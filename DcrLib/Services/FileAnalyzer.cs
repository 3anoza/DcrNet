using System.Text;
using DcrLib.Extensions;
using DcrLib.Models;

namespace DcrLib.Services;

public class FileAnalyzer
{
    private readonly byte[] _fileBytes;

    public FileAnalyzer(byte[] fileBytes)
    {
        _fileBytes = fileBytes;
    }

    public DcrAudioFormat GetAudioFormat(byte[] chunkStart, byte[] chunkEnd, int chunkLength)
    {
        XmlEjector ejector;

        long startPosition = _fileBytes.FindPatternPosition(chunkStart, 0) + chunkLength;

        if (startPosition < 0) throw new Exception("Chunk with audio info not found");
        
        long endPosition = _fileBytes.FindPatternPosition(chunkEnd, startPosition);

        if (startPosition < 0) throw new Exception("Chunk with audio info is corrupted");

        byte[] buffer = new byte[endPosition - startPosition];
        Array.Copy(_fileBytes, startPosition, buffer, 0, buffer.Length);

        var xmlContent = Encoding.Default.GetString(buffer);
        
        ejector = new XmlEjector(xmlContent);
     
        var formatData = ejector.GetFormatData();
        var audioFormat = DcrParser.ParseFormatData(formatData);
        audioFormat.Channels = ejector.GetChannelsAmount();
        return audioFormat;
    }

    public List<byte[]> GetSpeexData(AudioDescriptor descriptor)
    {
        var output = new List<byte[]>();

        long currentPosition = _fileBytes.FindPatternPosition(descriptor.DataBegin, 0) + descriptor.ChunkLength + 4;
        long dataEndPos = _fileBytes.FindPatternPosition(descriptor.DataEnd, currentPosition);

        var completeFrameSize = descriptor.PacketLength * descriptor.ChannelsToDecode;

        while (currentPosition < dataEndPos)
        {
            if (StartsWithAt(descriptor.VideoBegin, currentPosition))
            {
                var videoEndPosition = _fileBytes.FindPatternPosition(descriptor.VideoEnd, currentPosition + descriptor.VideoBegin.Length);
                if (videoEndPosition == -1)
                    break;

                currentPosition = videoEndPosition + descriptor.ChunkLength;
                continue;
            }

            if (StartsWithAt(descriptor.AudioMarker, currentPosition))
            {
                currentPosition += descriptor.ChunkLength;
                continue;
            }

            if (StartsWithAt(descriptor.AudioPointer, currentPosition))
            {
                currentPosition += descriptor.ChunkLength;
                continue;
            }

            if (currentPosition + completeFrameSize > dataEndPos)
                break;

            var buffer = new byte[completeFrameSize];
            Array.Copy(_fileBytes, currentPosition, buffer, 0, completeFrameSize);
            output.Add(buffer);

            currentPosition += completeFrameSize;

            //var nextCadrPosition = _fileBytes.FindPatternPosition(descriptor.VideoBegin, currentPosition);

            //if (nextCadrPosition == -1)
            //{
            //    nextCadrPosition = dataEndPos;
            //}

            //if (nextCadrPosition > currentPosition)
            //{
            //    var mrkPos = _fileBytes.FindPatternPosition(descriptor.AudioMarker, currentPosition);

            //    if (mrkPos != -1 && mrkPos == currentPosition)
            //        currentPosition += descriptor.ChunkLength;

            //    var ptrPos = _fileBytes.FindPatternPosition(descriptor.AudioPointer, currentPosition);

            //    if (ptrPos != -1 && ptrPos == currentPosition)
            //        currentPosition += descriptor.ChunkLength;

            //    var buffer = new byte[completeFrameSize];
            //    Array.Copy(_fileBytes, currentPosition, buffer, 0, buffer.Length);
            //    //frameBuffer.AddRange(buffer);

            //    //Console.Write($"{Encoding.Default.GetString(buffer, 0, 1)}, ");

            //    //currentSize += completeFrameSize;
            //    currentPosition += completeFrameSize;
            //    output.Add(buffer);

            //    //if (currentSize == completeFrameSize)
            //    //{
            //    //    output.Add(frameBuffer.ToArray());
            //    //    frameBuffer.Clear();
            //    //    currentSize = 0;
            //    //}
            //    continue;
            //}

            //currentPosition = _fileBytes.FindPatternPosition(descriptor.VideoEnd, nextCadrPosition);

            //if (currentPosition == -1) break;

            //currentPosition += descriptor.ChunkLength;
        }

        return output;
    }

    private bool StartsWithAt(byte[] pattern, long position)
    {
        if (position + pattern.Length > _fileBytes.Length)
            return false;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (_fileBytes[position + i] != pattern[i])
                return false;
        }
        return true;
    }
}