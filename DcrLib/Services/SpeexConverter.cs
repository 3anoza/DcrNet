using NSpeex;
using WaveFormat = DcrNet.Models.WaveFormat;

namespace DcrNet.Services;

public class SpeexConverter
{
    /// <summary>
    ///     Decodes speex frames into pcm frames and store each channel into separate file
    /// </summary>
    /// <param name="speexDataFile">file with only speex-related data</param>
    /// <param name="audioFormat">information for related speex data</param>
    /// <returns>paths to pcm data files for each channel</returns>
    public List<string> ExtractPcmAudioForEachChannel(string speexDataFile, WaveFormat audioFormat)
    {
        Console.WriteLine("[DCR_LIB]: Started PCM decoding for speex frames");
        var output = new List<string>();

        var chunkSize = audioFormat.FrameSize * audioFormat.Channels;
        using var speexStream = new FileStream(speexDataFile, FileMode.Open, FileAccess.Read);

        var channelStreams = InitializeStreamsWithDecoders(audioFormat.Channels, audioFormat.ChannelMode, output);

        var quarterPartOfChunks = GetChunksMultiplier(chunkSize, speexStream.Length);

        var buffer = new byte[chunkSize * quarterPartOfChunks];
        var frame = new byte[audioFormat.FrameSize];
        var bytesRead = 0;
        Console.WriteLine("[DCR_LIB]: Started speex stream reading.");
        while ((bytesRead = speexStream.Read(buffer, 0, buffer.Length)) > 0)
            for (var channelIndex = 0; channelIndex < channelStreams.Count; channelIndex++)
            for (var frameIndex = 0; frameIndex < quarterPartOfChunks; frameIndex++)
                DecodeAndStoreFrames(
                    channelStreams[channelIndex],
                    buffer,
                    frame,
                    channelIndex,
                    frameIndex,
                    chunkSize,
                    audioFormat.PcmFrameSize,
                    audioFormat.FramesPerPacket);

        foreach (var channelStream in channelStreams)
        {
            channelStream.stream.Close();
            channelStream.stream.Dispose();
        }

        Console.WriteLine("[DCR_LIB]: PCM decoding finished");
        return output;
    }

    private List<(FileStream stream, SpeexDecoder decoder)> InitializeStreamsWithDecoders(int channels,
        BandMode channelMode, List<string> streamPaths)
    {
        Console.WriteLine($"[DCR_LIB]: Initializing {channels} streams with decoders.");

        var channelStreams = new List<(FileStream stream, SpeexDecoder decoder)>();

        for (var ch = 0; ch < channels; ch++)
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{ch + 1}.pcm");
            channelStreams.Add(new ValueTuple<FileStream, SpeexDecoder>(
                new FileStream(path, FileMode.Create, FileAccess.Write),
                new SpeexDecoder(channelMode))
            );
            streamPaths.Add(path);
        }

        Console.WriteLine("[DCR_LIB]: Streams and decoders are created.");

        return channelStreams;
    }

    private long GetChunksMultiplier(int chunkSize, long speexStreamLength)
    {
        var chunksAmount = speexStreamLength / chunkSize;
        var chunksMultiplier = (long)(chunksAmount * 0.25f);
        Console.WriteLine(
            $"[DCR_LIB]: Approximate chunks amount: {chunksAmount}. Speeding up the process by taking 25% ({chunksMultiplier}) of chunks/reading.");
        return chunksMultiplier;
    }

    private void DecodeAndStoreFrames((FileStream stream, SpeexDecoder decoder) channel, byte[] buffer,
        byte[] frameBuffer, int channelIndex, int frameIndex, int chunkSize, int pcmFrameSize, int framesPerPacket)
    {
        var frameOffset = channelIndex * frameBuffer.Length + frameIndex * chunkSize;
        // Take frame from buffer for specified channel
        Array.Copy(buffer, frameOffset, frameBuffer, 0, frameBuffer.Length);
        var pcmFrame = DecodeSpeexFrame(channel.decoder, pcmFrameSize, framesPerPacket, frameBuffer);

        channel.stream.Write(pcmFrame, 0, pcmFrame.Length);
    }

    private byte[] DecodeSpeexFrame(SpeexDecoder decoder, int pcmFrameSize, int framesPerPacket,
        byte[] speexFrame)
    {
        var decodedFrame = new short[pcmFrameSize * framesPerPacket];
        decoder.Decode(speexFrame, 0, speexFrame.Length, decodedFrame, 0, false);
        var pcmBytes = new byte[decodedFrame.Length * 2];
        //Array.Copy(decodedFrame, 0, pcmBytes, 0,decodedFrame.Length * 2);
        Buffer.BlockCopy(decodedFrame, 0, pcmBytes, 0, decodedFrame.Length * 2);

        return pcmBytes;
    }
}