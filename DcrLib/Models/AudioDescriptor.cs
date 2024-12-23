namespace DcrLib.Models;

public class AudioDescriptor
{
    public byte[] DataBegin { get; set; }
    public byte[] DataEnd { get; set; }
    public byte[] AttxBegin { get; set; }
    public byte[] AttxEnd { get; set; }
    public byte[] VideoBegin { get; set; }
    public byte[] VideoEnd { get; set; }
    public byte[] AudioMarker { get; set; }
    public byte[] AudioPointer { get; set; }
    public int ChunkLength { get; set; }
    public int PacketLength { get; set; }
    public int ChannelsToDecode { get; set; } = 14;
}