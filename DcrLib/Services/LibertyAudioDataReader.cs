using System.Text;
using DcrNet.Extensions;
using DcrNet.Models;

namespace DcrNet.Services;

/// <summary>
///     Represents audio data reader class for Liberty files
/// </summary>
public class LibertyAudioDataReader
{
    private readonly BinaryReader _fileReader;
    private readonly WaveFormat _waveFormat;

    /// <summary>
    ///     Creates new instance of reader
    /// </summary>
    /// <param name="fileStream">file stream with liberty data</param>
    /// <param name="waveFormat">audio information</param>
    public LibertyAudioDataReader(FileStream fileStream, WaveFormat waveFormat)
    {
        _waveFormat = waveFormat;
        _fileReader = new BinaryReader(fileStream);
    }

    /// <summary>
    ///     Reads all speex data into single file
    /// </summary>
    public string ReadFramesToFile()
    {
        _fileReader.BaseStream.Seek(0, SeekOrigin.Begin);
        var mergedFrameSize = _waveFormat.FrameSize * _waveFormat.Channels;
        var dataStartPosition = _fileReader.FindPatternPosition(Constants.Data.Begin, 4096 * 8);
        var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_allchannels");

        Console.WriteLine($"[DCR_LIB]: Started frames reading into separate file: {tempFilePath}");
        _fileReader.BaseStream.Position = dataStartPosition + Constants.FILE_SECTION_SIZE + 4;

        using (var framesFileStream =
               new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            while (_fileReader.BaseStream.Position < _fileReader.BaseStream.Length)
            {
                if (PeekMatch(_fileReader, Constants.Data.End))
                    break;

                if (PeekMatch(_fileReader, Constants.Cadr.Begin))
                {
                    SkipSection(_fileReader, Constants.Cadr.Begin, Constants.Cadr.End);
                    continue;
                }

                if (PeekMatch(_fileReader, Constants.AudiMrk.Begin) || PeekMatch(_fileReader, Constants.AudiPtr.Begin))
                {
                    _fileReader.BaseStream.Seek(Constants.FILE_SECTION_SIZE, SeekOrigin.Current);
                    continue;
                }

                var audioBuf = new byte[mergedFrameSize];
                var bytesRead = _fileReader.Read(audioBuf, 0, mergedFrameSize);
                if (bytesRead <= 0) break;
                framesFileStream.Write(audioBuf, 0, bytesRead);
            }


            Console.WriteLine($"[DCR_LIB]: Frames reading finished. Total size: {framesFileStream.Length} bytes");
        }

        return tempFilePath;
    }

    private bool PeekMatch(BinaryReader reader, byte[] tag)
    {
        var buf = new byte[tag.Length];
        var read = reader.Read(buf, 0, buf.Length);
        reader.BaseStream.Position -= read;
        if (read < buf.Length) return false;
        return !buf.Where((t, i) => t != tag[i]).Any();
    }

    private void SkipSection(BinaryReader reader, byte[] openTag, byte[] closeTag)
    {
        reader.BaseStream.Position += openTag.Length;

        var matchPos = 0;
        while (true)
        {
            int b = reader.ReadByte();
            if (b < 0)
                throw new InvalidDataException("Closing tag missed: " + Encoding.ASCII.GetString(closeTag));
            if (b == closeTag[matchPos])
            {
                matchPos++;
                if (matchPos == closeTag.Length)
                    return;
            }
            else
            {
                if (matchPos > 0)
                {
                    reader.BaseStream.Position -= matchPos;
                    matchPos = 0;
                }
            }
        }
    }

    public List<byte[]> GetSpeexData(byte[] fileBytes)
    {
        var output = new List<byte[]>();

        var currentPosition = fileBytes.FindPatternPosition(Constants.Data.Begin, 0) + Constants.FILE_SECTION_SIZE + 4;
        var dataEndPos = fileBytes.FindPatternPosition(Constants.Data.End, currentPosition);

        var completeFrameSize = _waveFormat.FrameSize * _waveFormat.Channels;

        while (currentPosition < dataEndPos)
        {
            if (fileBytes.StartsWithAt(Constants.Cadr.Begin, currentPosition))
            {
                var videoEndPosition =
                    fileBytes.FindPatternPosition(Constants.Cadr.End, currentPosition + Constants.FILE_SECTION_SIZE);
                if (videoEndPosition == -1)
                    break;

                currentPosition = videoEndPosition + Constants.FILE_SECTION_SIZE;
                continue;
            }

            if (fileBytes.StartsWithAt(Constants.AudiMrk.Begin, currentPosition))
            {
                currentPosition += Constants.FILE_SECTION_SIZE;
                continue;
            }

            if (fileBytes.StartsWithAt(Constants.AudiPtr.Begin, currentPosition))
            {
                currentPosition += Constants.FILE_SECTION_SIZE;
                continue;
            }

            if (currentPosition + completeFrameSize > dataEndPos)
                break;

            var buffer = new byte[completeFrameSize];
            Array.Copy(fileBytes, currentPosition, buffer, 0, completeFrameSize);
            output.Add(buffer);

            currentPosition += completeFrameSize;
        }

        return output;
    }
}