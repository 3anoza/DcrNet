using DcrNet.Services;
using NAudio.Lame;
using NAudio.Wave;

namespace DcrNet.Files;

public class Mp3FileBuilder
{
    public const int BITS_PER_SAMPLE = 16;
    public const int BYTES_PER_SAMPLE = BITS_PER_SAMPLE / 8;
    private readonly List<string> _channelFiles;
    private readonly AudioMixer _mixer;
    private readonly int _nbChannels;

    private readonly int _sampleRate;

    private List<FileStream>? _channelStreams;

    /// <summary>
    ///     Creates mp3 files builder instance
    /// </summary>
    /// <param name="file">instance of mp3 file</param>
    public Mp3FileBuilder(LibertyFile file)
    {
        _channelFiles = file.GetPcmChannelFiles();
        _sampleRate = file.FileDescriptor.WaveFormat.SampleRate;
        _nbChannels = file.FileDescriptor.WaveFormat.NbChannels;
        _mixer = new AudioMixer();
    }

    /// <summary>
    ///     Builds mono channeled mp3 file
    /// </summary>
    public void BuildMonoMp3(string outputMp3Path)
    {
        Console.WriteLine("[DCR_LIB]: Building MONO mp3 file from PCM data.");
        var channelStreams = _channelStreams ??= OpenChannelStreams(_channelFiles);

        var totalSamples = GetTotalSamples(channelStreams);
        var waveFormat = new WaveFormat(_sampleRate, BITS_PER_SAMPLE, _nbChannels);

        Console.WriteLine($"[DCR_LIB]: MP3 file created and located at path: {outputMp3Path}");
        using var fileStream = File.Create(outputMp3Path);
        using var mp3Writer = new LameMP3FileWriter(fileStream, waveFormat, LAMEPreset.STANDARD);

        Console.WriteLine("[DCR_LIB]: Mixing the channels to mono.");
        for (long i = 0; i < totalSamples; i++)
        {
            var sampleFromAllChannels = ReadSampleFromAllChannels(channelStreams);
            var mixedSample = _mixer.MixSamplesToMono(sampleFromAllChannels);
            mp3Writer.Write(mixedSample, 0, mixedSample.Length);
        }

        Console.WriteLine("[DCR_LIB]: PCM data merged to single channel and inserted into file.");
        Console.WriteLine("[DCR_LIB]: MP3 file creation finished.");
    }

    /// <summary>
    ///     Builds mp3 files. Separate file for each channel
    /// </summary>
    /// <param name="channelsToSkip">1-based sequence of channels to skip</param>
    public List<string> BuildMp3(string folderPath, params int[] channelsToSkip)
    {
        var channelMp3Files = new List<string>();

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        Console.WriteLine(
            $"[DCR_LIB]: Started MP3 files building. Channels to skip: {string.Join(",", channelsToSkip)}");

        var channelStreams = _channelStreams ??= OpenChannelStreams(_channelFiles);
        channelStreams = channelStreams.Where(stream =>
                !channelsToSkip.Contains(_channelStreams.IndexOf(stream) + 1))
            .ToList();

        var waveFormat = new WaveFormat(_sampleRate, BITS_PER_SAMPLE, 1);

        foreach (var channelStream in channelStreams)
        {
            var outputMp3Path = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(channelStream.Name) + ".mp3");
            using var rawStream = new RawSourceWaveStream(channelStream, waveFormat);
            using var mp3Writer = new LameMP3FileWriter(File.Create(outputMp3Path), waveFormat, LAMEPreset.STANDARD);
            rawStream.CopyTo(mp3Writer);

            channelMp3Files.Add(outputMp3Path);
            Console.WriteLine($"[DCR_LIB]: Created complete file for channel by path: {outputMp3Path}");
        }

        Console.WriteLine($"[DCR_LIB]: All MP3 files are located in folder: {folderPath}");

        return channelMp3Files;
    }

    /// <summary>
    ///     Removes all reserved resources and files
    /// </summary>
    public void Dispose()
    {
        if (_channelStreams != null)
        {
            Console.WriteLine($"[DCR_LIB]: Clearing {_channelStreams.Count} streams and {_channelFiles.Count} files");

            foreach (var channelStream in _channelStreams)
            {
                channelStream.Dispose();
                channelStream.Close();
            }

            Console.WriteLine("[DCR_LIB]: Streams cleared");
            _channelStreams = null;
        }

        foreach (var channelFile in _channelFiles.Where(File.Exists)) File.Delete(channelFile);

        Console.WriteLine("[DCR_LIB]: Files cleared");
    }

    private List<FileStream> OpenChannelStreams(List<string> channelFiles)
    {
        return channelFiles.Select(channel => new FileStream(channel, FileMode.Open, FileAccess.Read))
            .ToList();
    }

    private long GetTotalSamples(List<FileStream> channels)
    {
        var totalSamples = channels.Select(channel => channel.Length)
            .Prepend(int.MaxValue)
            .Min();

        totalSamples /= BYTES_PER_SAMPLE;

        return totalSamples;
    }

    private List<byte[]> ReadSampleFromAllChannels(List<FileStream> channels)
    {
        var samples = new List<byte[]>();

        foreach (var channel in channels)
        {
            var buffer = new byte[BYTES_PER_SAMPLE];
            var bytesRead = channel.Read(buffer, 0, buffer.Length);
            if (bytesRead < 0)
                buffer = BitConverter.GetBytes((short)0);

            samples.Add(buffer);
        }

        return samples;
    }
}