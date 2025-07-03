namespace DcrNet.Services;

public class AudioMixer
{
    /// <summary>
    ///     Mixes samples sequence (each sample for channel) into single normalized mono sample
    /// </summary>
    /// <param name="samples">samples from each channel</param>
    /// <returns>mixed sample</returns>
    public byte[] MixSamplesToMono(List<byte[]> samples)
    {
        var mixedSample = samples.Aggregate(0, (current, sample)
            => current + BitConverter.ToInt16(sample));

        mixedSample /= samples.Count; //Math.Clamp(mixedSample, short.MinValue, short.MaxValue);
        var mixedSampleBytes = BitConverter.GetBytes((short)mixedSample);

        return mixedSampleBytes;
    }
}