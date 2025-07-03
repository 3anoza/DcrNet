using NSpeex;

namespace DcrNet.Models;

/// <summary>
///     For more info see  <seealso cref="Services.WaveFormatParser"/>
/// </summary>
public class WaveFormat
{
    /// <summary>
    ///     Parsed audio data codec name. (Usually Speex)
    /// </summary>
    public string CodecName { get; set; }
    /// <summary>
    ///     Audio encoder version
    /// </summary>
    public string Version { get; set; }
    public int BytesPerSecond { get; set; }
    /// <summary>
    ///     Size of single speex frame
    /// </summary>
    public short FrameSize { get; set; }
    /// <summary>
    ///     Speex audio data quality. (Q1-Q10)
    /// </summary>
    public short Quality { get; set; }
    public int SampleRate { get; set; }
    /// <summary>
    ///     Speex audio band mode. See <see cref="NSpeex.BandMode"/>
    /// </summary>
    public BandMode ChannelMode { get; set; }
    public int ModeBitStreamVersion { get; set; }
    /// <summary>
    ///     Amount of native speex channels
    /// </summary>
    public int NbChannels { get; set; }
    public int BitRate { get; set; }
    public int PcmFrameSize { get; set; }
    public bool IsVBR { get; set; }
    /// <summary>
    ///     Amount of PCM frames in single Speex packet (frame). Use along with <see cref="PcmFrameSize"/>
    /// </summary>
    public int FramesPerPacket { get; set; }
    /// <summary>
    ///     Amount of audio channels. Not to be confused with <see cref="NbChannels"/>
    /// </summary>
    public int Channels { get; set; }
}