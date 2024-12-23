using DcrLib.Models;
using NAudio.Wave;
using NSpeex;
using System.Text;

namespace DcrLib.Services;

public class SpeexConverter
{
    public static List<byte[]> GetPcmAudioForAllChannels(List<byte[]>? speexData, DcrAudioFormat audioFormat, int packetSize)
    {
        List<byte[]> output = new List<byte[]>();

        if (speexData == null || speexData.Count == 0)
        {
            Console.WriteLine("Audio data is empty!");
        }

        List<byte> channelDataBuffer = new List<byte>();

        for (int i = 0; i < audioFormat.Channels; i++)
        {
            SpeexDecoder decoder = new SpeexDecoder(audioFormat.ChannelMode);

            foreach (var speexFrame in speexData)
            {
                var chunkToDecode = speexFrame.Skip(i * packetSize)
                                                    .Take(packetSize)
                                                    .ToArray();
                short[] decodedSamples = new short[audioFormat.PcmFrameSize * audioFormat.FramesPerPacket];

                decoder.Decode(chunkToDecode, 0, chunkToDecode.Length, decodedSamples, 0, false);

                byte[] pcmBytes = new byte[decodedSamples.Length * 2];
                Buffer.BlockCopy(decodedSamples, 0, pcmBytes, 0, decodedSamples.Length * 2);

                channelDataBuffer.AddRange(pcmBytes);
            }
            output.Add(channelDataBuffer.ToArray());
            channelDataBuffer.Clear();
        }

        //List<SpeexDecoder> channelDecoders = new List<SpeexDecoder>();
        //for (int i = 0; i < audioFormat.Channels; i++)
        //{
        //    SpeexDecoder decoder = new SpeexDecoder(audioFormat.ChannelMode);
        //    channelDecoders.Add(decoder);
        //}

        //List<byte[]> currentPcmChunk = new List<byte[]>();



        //foreach (var speexFrame in speexData)
        //{
        //    var channelsFrames = speexFrame.Chunk(packetSize).ToList();
        //    for (int i = 0; i < channelDecoders.Count; i++)
        //    {
        //        short[] decoded = new short[audioFormat.PcmFrameSize * audioFormat.FramesPerPacket];
        //        channelDecoders[i].Decode(channelsFrames[i], 0, packetSize, decoded, 0, false);

        //        byte[] decodedFramesBytes = new byte[decoded.Length * 2];
        //        Buffer.BlockCopy(decoded, 0, decodedFramesBytes, 0, decoded.Length * 2);
        //        currentPcmChunk.Add(decodedFramesBytes);
        //    }
        //    output.Add(currentPcmChunk.SelectMany(chunk => chunk).ToArray());
        //    currentPcmChunk.Clear();
        //}

        //List<byte[]> currentFrameBlock = new List<byte[]>();
        //int FrameIndex = 0;
        //bool[] channelSilence = Enumerable.Repeat(true, audioFormat.Channels).ToArray();
        //foreach (byte[] frame in speexData)
        //{
        //    short[] decoded = new short[audioFormat.PcmFrameSize * audioFormat.FramesPerPacket];
        //    ChannelDecoders.ElementAt(FrameIndex % audioFormat.Channels).Decode(frame, 0, packetSize, decoded, 0, false);

        //    byte[] decodedFramesBytes = new byte[decoded.Length * 2];
        //    Buffer.BlockCopy(decoded, 0, decodedFramesBytes, 0, decoded.Length * 2);

        //    currentFrameBlock.Add(decodedFramesBytes);

        //    if (currentFrameBlock.Count == audioFormat.Channels)
        //    {
        //        byte[] InterleavedAndDecodedBlock;

        //        if (audioFormat.Channels == 1)
        //            InterleavedAndDecodedBlock = currentFrameBlock.ElementAt(0);
        //        else
        //            InterleavedAndDecodedBlock = InterleaveFrames(currentFrameBlock);

        //        output.Add(InterleavedAndDecodedBlock);

        //        currentFrameBlock = new List<byte[]>();
        //    }
        //    FrameIndex++;
        //}

        return output;
    }

    private static byte[] InterleaveFrames(List<byte[]> frames)
    {
        int outputSize = frames.First().Length * frames.Count;
        int numberOfSamplesInFrame = frames.First().Length / 2;
        byte[] output = new byte[outputSize];

        int sampleCounter = 0;

        for (int seampleInFrame = 0; seampleInFrame < numberOfSamplesInFrame; seampleInFrame++)
        {
            foreach (byte[] frame in frames)
            {
                for (int byteNumberInSample = 0; byteNumberInSample < 2; byteNumberInSample++)
                {
                    int indexOfStartingByteInCurrentFrame = seampleInFrame * 2;
                    int indexOfCurrentByteToCopy = indexOfStartingByteInCurrentFrame + byteNumberInSample;

                    output[sampleCounter++] = frame[indexOfCurrentByteToCopy];
                }
            }
        }

        return output;
    }

    public static List<byte[]> GetPcmAudio(List<byte[]>? speexData, DcrAudioFormat audioFormat, int channels, int packetSize)
    {
        List<byte[]> output = new List<byte[]>();

        if (speexData == null || speexData.Count == 0)
        {
            Console.WriteLine("Audio data is empty!");
        }

        SpeexDecoder decoder = new SpeexDecoder(audioFormat.ChannelMode);

        foreach (var speexFrame in speexData)
        {
            var firstChannelFrame = speexFrame.Chunk(packetSize).Take(1);
            short[] pcmFrame = new short[audioFormat.PcmFrameSize * audioFormat.FramesPerPacket];

            foreach (var firstFrame in firstChannelFrame)
            {
                var status = decoder.Decode(firstFrame, 0, firstFrame.Length, pcmFrame, 0, false);

                Console.WriteLine($"Samples read: {status}");

                byte[] pcmFrameBytes = new byte[pcmFrame.Length * 2];
                Buffer.BlockCopy(pcmFrame, 0, pcmFrameBytes, 0, pcmFrameBytes.Length);

                output.Add(pcmFrameBytes);
                break;
            }
        }

        //SpeexDecoder decoder = new SpeexDecoder(speexMode);

        //foreach (var speexFrame in speexData)
        //{
        //    var actualFrames = speexFrame.Chunk(119).Take(1);

        //    foreach (var actualFrame in actualFrames)
        //    {
        //        short[] decodedBuffer = new short[decoder.FrameSize];

        //        Console.WriteLine($"ActFrameSize={actualFrame.Length}, BufferSize={decodedBuffer.Length}");

        //        //var status = decoder.Decode(actualFrame, 0, actualFrame.Length - 1, decodedBuffer);

        //        var status = decoder.DecodeInt(actualFrame, decodedBuffer);

        //        Console.WriteLine($"Decode status {status}");

        //        if (status < 0)
        //            continue;

        //        output.AddRange(decodedBuffer);
        //    }

        //foreach (var actualFrame in actualFrames)
        //{


        //}


        //if (speexFrame.Length > 1666)
        //{
        //    var chunks = speexFrame.Chunk(1666).ToList();
        //    foreach (var chunk in chunks)
        //    {
        //        Console.WriteLine($"FrameSize={speexFrame.Length}, BufferSize={decodedBuffer.Length}");

        //        var status = decoder.Decode(chunk, 0, chunk.Length, decodedBuffer);

        //        Console.WriteLine($"Decode status {status}");

        //        if (status < 0)
        //            continue;


        //        output.AddRange(decodedBuffer);
        //    }
        //}
        //else
        //{
        //    Console.WriteLine($"FrameSize={speexFrame.Length}, BufferSize={decodedBuffer.Length}");

        //    var status = decoder.Decode(speexFrame, 0, speexFrame.Length, decodedBuffer);

        //    Console.WriteLine($"Decode status {status}");

        //    if (status < 0)
        //        continue;

        //    output.AddRange(decodedBuffer);
        //}
    //}

        return output;
    }
}