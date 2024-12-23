using NSpeex;

namespace DcrLib.Models;

public class DcrAudioFormat
{
    public string CodecName { get; set; }
    public string Version { get; set; }
    public int SampleRate { get; set; }
    public BandMode ChannelMode { get; set; }
    public int ModeBitStreamVersion { get; set; }
    public int NbChannels { get; set; }
    public int BitRate { get; set; }
    public int PcmFrameSize { get; set; }
    public bool IsVBR { get; set; }
    public int FramesPerPacket {get; set; }
    public int Channels { get; set; }
}