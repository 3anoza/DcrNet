using DcrLib.Models;

namespace DcrLib.Services;

public class AudioMixer
{
    private readonly List<byte[]> _pcmData;
    private readonly DcrAudioFormat _audioFormat;
    public const int BYTES_PER_SAMPLE = 2;
    public const int BITS_PER_SAMPLE = 16;

    public AudioMixer(List<byte[]> pcmData, DcrAudioFormat format)
    {
        _pcmData = pcmData;
        _audioFormat = format;
    }


    public List<byte> MixChannels()
    {
        int channelsAmount = _audioFormat.Channels;

        int maxChannelLength = 0;

        foreach (var channel in _pcmData)
        {
            if (channel.Length > maxChannelLength)
                maxChannelLength = channel.Length;
        }

        int totalSamples = maxChannelLength / BYTES_PER_SAMPLE;

        List<byte> mixedPcmData = new List<byte>();

        for (int i = 0; i < totalSamples; i++)
        {
            for (int ch = 0; ch < channelsAmount; ch++)
            {
                byte[] channelData = _pcmData[ch];
                if (i * BYTES_PER_SAMPLE < channelData.Length)
                {
                    for (int b = 0; b < BYTES_PER_SAMPLE; b++)
                    {
                        mixedPcmData.Add(channelData[i * BYTES_PER_SAMPLE + b]);
                    }
                }
                else
                {
                    for (int b = 0; b < BYTES_PER_SAMPLE; b++)
                    {
                        mixedPcmData.Add(0);
                    }
                }
            }
        }

        return mixedPcmData;
    }

    public byte[] MixChannelsToMono()
    {
        int bytesPerSample = BITS_PER_SAMPLE / 8;
        int minLength = int.MaxValue;

        foreach (var channel in _pcmData)
        {
            if (channel.Length < minLength)
                minLength = channel.Length;
        }

        int totalSamples = minLength / bytesPerSample;
        byte[] mixedPcmData = new byte[totalSamples * bytesPerSample];

        for (int i = 0; i < totalSamples; i++)
        {
            int sum = 0;

            for (int ch = 0; ch < _pcmData.Count; ch++)
            {
                byte[] channelData = _pcmData[ch];
                int sampleValue = BitConverter.ToInt16(channelData, i * bytesPerSample);

                sum += sampleValue;
            }

            short avgSample = (short)(sum / _pcmData.Count);
            byte[] avgBytes = BitConverter.GetBytes(avgSample);
            Buffer.BlockCopy(avgBytes, 0, mixedPcmData, i * bytesPerSample, bytesPerSample);
        }

        return mixedPcmData;
    }
}