using DcrLib.Services;
using NAudio.Wave;

namespace DcrLib.Files;

public class WavFileBuilder
{
    public const int BITS_PER_SAMPLE = 16;
    private readonly DcrFile _file;

    public WavFileBuilder(DcrFile file)
    {
        _file = file;
    }

    public void BuildWav(string path)
    {
        var pcmChannels = _file.GetPCM();
        var audioFormatData = _file.AudioFormatData;
        var audioMixer = new AudioMixer(pcmChannels, audioFormatData);
        var mixedChannels = audioMixer.MixChannelsToMono();

        SavePcmToWave(mixedChannels, path, audioFormatData.SampleRate, BITS_PER_SAMPLE, audioFormatData.NbChannels);
    }

    private void SavePcmToWave(byte[] pcmData, string outputFilePath, int sampleRate, int bitsPerSample, int numChannels)
    {
        using var wavWriter = new WaveFileWriter(outputFilePath, new WaveFormat(sampleRate, bitsPerSample, numChannels));
        wavWriter.Write(pcmData, 0, pcmData.Length);
    }
}