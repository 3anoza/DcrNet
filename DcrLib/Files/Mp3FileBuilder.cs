using DcrLib.Services;
using NAudio.Lame;
using NAudio.Wave;

namespace DcrLib.Files;

public class Mp3FileBuilder
{
    public const int BITS_PER_SAMPLE = 16;
    private readonly DcrFile _file;

    public Mp3FileBuilder(DcrFile file)
    {
        _file = file;
    }

    public void BuildMp3(string path)
    {
        var pcmChannels = _file.GetPCM();
        var audioMixer = new AudioMixer(pcmChannels, _file.AudioFormatData);
        var mixedChannels = audioMixer.MixChannelsToMono();

        SavePcmToMp3(mixedChannels, path, _file.AudioFormatData.SampleRate, BITS_PER_SAMPLE,
            _file.AudioFormatData.NbChannels);
    }

    private void SavePcmToMp3(byte[] pcmData, string outputFilePath, int sampleRate, int bitsPerSample, int numChannels)
    {
        using (var ms = new MemoryStream(pcmData))
        using (var rdr = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, bitsPerSample, numChannels)))
        using (var mp3Writer = new LameMP3FileWriter(outputFilePath, rdr.WaveFormat, LAMEPreset.STANDARD))
        {
            rdr.CopyTo(mp3Writer);
        }
    }
}