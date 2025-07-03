using System.Numerics;
using System.Text;
using DcrNet.Extensions;
using DcrNet.Models;
using DcrNet.Models.Liberty.Bookmarks;
using DcrNet.Models.Liberty.Meta;
using DcrNet.Providers;
using WaveFormat = DcrNet.Models.WaveFormat;

namespace DcrNet.Services;

/// <summary>
///     Representation of Liberty file sections data ejector
/// </summary>
public class LibertyFileDescriptorEjector
{
    private readonly BinaryReader _fileReader;
    private readonly bool _isLegacyVersion;
    private readonly IXmlSectionProvider _sectionProvider;

    /// <summary>
    ///    Creates an instance of ejector
    /// </summary>
    /// <param name="fileStream">stream with liberty file data</param>
    /// <param name="sectionProvider">xml sections provider</param>
    /// <exception cref="Exception">validations fail reason</exception>
    public LibertyFileDescriptorEjector(FileStream fileStream, IXmlSectionProvider sectionProvider)
    {
        _sectionProvider = sectionProvider;
        fileStream.Position = 0;
        _fileReader = new BinaryReader(fileStream);

        var header = string.Join("", _fileReader.ReadChars(Constants.FILE_HEADER.Length));

        if (!header.Equals(Constants.FILE_HEADER))
            throw new Exception($"Invalid file type. Expecting header: {Constants.FILE_HEADER}");

        Console.WriteLine("[DCR_LIB]: File header is valid. Verifying version.");

        var firstSectionWithJunk = _fileReader.ReadBytes(Constants.FILE_SECTION_SIZE * 2);
        var index = firstSectionWithJunk.FindPatternPosition(Constants.Meta.Begin, 0);
        if (index < 0)
        {
            _isLegacyVersion = true;
            Console.WriteLine("[DCR_LIB]: Legacy version detected.");
        }
        else
        {
            Console.WriteLine("[DCR_LIB]: Latest version detected.");
        }
    }

    /// <summary>
    ///     Returns file information
    /// </summary>
    public LibertyFileDescriptor GetFileDescriptor()
    {
        LibertyFileDescriptor descriptor = new();

        Console.WriteLine("[DCR_LIB]: Starting file meta sections parsing.");
        _fileReader.BaseStream.Seek(-Constants.FILE_SECTION_SIZE, SeekOrigin.Current);

        if (!_isLegacyVersion)
        {
            var metaXmlData = ReadMetaSection();

            descriptor.WaveFormat =
                CreateWaveFormat(metaXmlData.Meta.Audio.FmtDataHex, metaXmlData.Meta.Audio.Channels.Count);
        }
        else
        {
            Console.WriteLine("[DCR_LIB]: Extracting legacy data.");

            var wfmtSectionBytes = ReadSectionBytes(Constants.Wfmt.Begin, Constants.Wfmt.End, 128);

            Console.WriteLine($"[DCR_LIB]: WaveFormat section read finished. Total Bytes: {wfmtSectionBytes.Length}.");

            var chnlSectionBytes = ReadSectionBytes(Constants.Channel.Begin, Constants.Channel.End, 16);

            Console.WriteLine($"[DCR_LIB]: Channel section read finished. Total Bytes: {chnlSectionBytes.Length}.");

            var channels = ReadChannelsData(chnlSectionBytes);

            Console.WriteLine(
                $"[DCR_LIB]: Channel section parsed. ChannelsAmount={channels.Item1}, ActiveChannels={string.Join(",", channels.Item2)}.");

            var legacyMetaData = ReadLegacyMetaSection();

            descriptor.WaveFormat = CreateWaveFormat(wfmtSectionBytes, channels.Item1);
        }

        var bookmarksData = ReadBookmarksSection();

        descriptor.Bookmarks = bookmarksData.Bookmarks.Select(bookmark =>
        {
            var parsedIdAndType = BookmarksParser.GetBookmarkType(bookmark.IdMark);
            return new BookmarkData
            {
                Id = parsedIdAndType.index,
                Type = parsedIdAndType.type,
                FilePosition = bookmark.PosMark?.GetPosition() ?? 0,
                CreatedAtUtc = bookmark.CrTimeUtc != null
                    ? DateTime.FromFileTimeUtc(bookmark.CrTimeUtc.GetTimestamp())
                    : default,
                CreatedAtLocal = bookmark.CrTimeLoc != null
                    ? DateTime.FromFileTime(bookmark.CrTimeLoc.GetTimestamp())
                    : default,
                NoteText = bookmark.PublNote
            };
        }).ToList();

        return descriptor;
    }


    private MarkXmlData ReadBookmarksSection()
    {
        var sectionBytes = ReadSectionBytes(Constants.Mark.Begin, Constants.Mark.End, 1024 * 100);
        Console.WriteLine($"[DCR_LIB]: Bookmarks section read finished. Total bytes: {sectionBytes.Length}");

        var xmlContent = Encoding.UTF8.GetString(sectionBytes, 0, sectionBytes.Length);
        Console.WriteLine("[DCR_LIB]: Bookmarks section converted to xml string using UTF8 encoding.");

        return _sectionProvider.GetSectionData<MarkXmlData>(xmlContent);
    }

    /// <summary>
    ///     Reads meta section from legacy file and returns extracted information
    /// </summary>
    private LegacyMetaXmlData ReadLegacyMetaSection()
    {
        var metaSectionBytes = ReadSectionBytes(Constants.Meta.Begin, Constants.Meta.End, 1024);
        Console.WriteLine($"[DCR_LIB]: Meta section read finished. Total bytes: {metaSectionBytes.Length}");

        var metaXmlContent = Encoding.UTF8.GetString(metaSectionBytes, 0, metaSectionBytes.Length);

        metaXmlContent = metaXmlContent.Substring(0, metaXmlContent.IndexOf("##"));

        Console.WriteLine("[DCR_LIB]: Meta section converted to xml string using UTF8 encoding.");

        var legacyMetadata = _sectionProvider.GetSectionData<LegacyMetaXmlData>(metaXmlContent);

        Console.WriteLine(
            $"[DCR_LIB]: Meta section parsed. App={legacyMetadata.Prop.AppName}, Ver={legacyMetadata.Prop.AppVer}, Build={legacyMetadata.Prop.AppBuild}.");

        return legacyMetadata;
    }

    /// <summary>
    ///     Reads meta section for dcr file and returns extracted information
    /// </summary>
    private MetaXmlData ReadMetaSection()
    {
        var metaSectionBytes = ReadSectionBytes(Constants.Meta.Begin, Constants.Meta.End, 1024);
        Console.WriteLine($"[DCR_LIB]: Meta section read finished. Total bytes: {metaSectionBytes.Length}.");

        var metaXmlContent = Encoding.UTF8.GetString(metaSectionBytes, 0, metaSectionBytes.Length);

        Console.WriteLine("[DCR_LIB]: Meta section converted to xml string using UTF8 encoding.");

        var metaXmlData = _sectionProvider.GetSectionData<MetaXmlData>(metaXmlContent);

        Console.WriteLine(
            $"[DCR_LIB]: Meta section parsed. App={metaXmlData.Meta.Prop.AppName}, Ver={metaXmlData.Meta.Prop.AppVer}, Build={metaXmlData.Meta.Prop.AppBuild}.");

        return metaXmlData;
    }

    /// <summary>
    ///     Reads bytes from dcr file stream for specified section patterns
    /// </summary>
    private byte[] ReadSectionBytes(byte[] patternBegin, byte[] patternEnd, int chunksSize)
    {
        var sectionStartIndex = _fileReader.FindPatternPosition(patternBegin, chunksSize) + Constants.FILE_SECTION_SIZE;
        _fileReader.BaseStream.Position = sectionStartIndex;

        var sectionEndIndex = _fileReader.FindPatternPosition(patternEnd, chunksSize);
        _fileReader.BaseStream.Position = sectionStartIndex;

        var buffer = _fileReader.ReadBytes((int)(sectionEndIndex - sectionStartIndex));

        _fileReader.BaseStream.Position = sectionEndIndex + Constants.FILE_SECTION_SIZE;

        return buffer;
    }

    private WaveFormat CreateWaveFormat(byte[] formatData, int channelsAmount)
    {
        var audioFormat = WaveFormatParser.ParseFormatData(formatData, true);
        audioFormat.Channels = channelsAmount;
        return audioFormat;
    }

    private WaveFormat CreateWaveFormat(string formatDataHex, int channelsAmount)
    {
        var audioFormat = WaveFormatParser.ParseFormatData(formatDataHex);
        audioFormat.Channels = channelsAmount;
        return audioFormat;
    }

    /// <summary>
    ///     Converts masked channels bytes to the list of enabled channels
    /// </summary>
    private (int, List<int>) ReadChannelsData(byte[] channelsBytes)
    {
        var maskIndex = channelsBytes.ToList().FindIndex(b => b > 0);
        var mask = BitConverter.ToUInt32(channelsBytes, maskIndex);
        var channelCount = BitOperations.PopCount(mask);
        var activeChannels = new List<int>();
        for (var i = 0; i < 32; i++)
            if (((mask >> i) & 1) != 0)
                activeChannels.Add(i + 1);

        return (channelCount, activeChannels);
    }

    /// <summary>
    ///     Frees memory used for reading the data from stream
    /// </summary>
    public void Dispose()
    {
        _fileReader.Close();
        _fileReader.Dispose();
    }
}