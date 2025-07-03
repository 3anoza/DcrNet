using System.Text;
using DcrNet.Extensions;
using DcrNet.Models;
using NSpeex;

namespace DcrNet.Services;


/// <summary>
/// <code>
/// Liberty format data represented as non-standard WAV+Speex‐ACM wrapper
/// around the 80-byte Speex header and its comment packet.
/// ------------------------------------------WAVE FORMAT Structure------------------------------------------------
/// |Offset | Length | Text or value             | Field native name      |                   Meaning             |
/// |0      | 8      | -                         | -                      | unknown fields in format structure    |
/// |8      | 4      | 1..n (for example 2100)   | nAvgBytesPerSec        | bytes per second                      |
/// |12     | 2      | 1..n (for example 42)     | nBlockAlign            | bytes per “block” (Speex frame size)  |
/// |14     | 1      | 0..10                     | nQuality               | encoder quality                       |
/// |15     | 5      | -                         | -                      | unknown fields in format structure    |
/// |20     | 8      | must be "Speex "          | speex_string           | reserved bytes header                 |
/// |28     | 20     | "speex-1.2beta3"          | speex_version          | encoder version string                |
/// |48     | 4      | 0..n                      | speex_version_id       | numeric version identifier            |
/// |52     | 4      | 80                        | header_size            | size of the Speex header              |
/// |56     | 4      | 16000, 32000, 48000, ...  | rate                   | sampling rate (Hz)                    |
/// |60     | 4      | 0=narrow,1=wide,2=ultra   | mode                   | band mode                             |
/// |64     | 4      | 0..n                      | mode_bitstream_version | bitstream version                     |
/// |68     | 4      | 1..n                      | nb_channels            | number of Speex channels              |
/// |72     | 4      | 1..n, or -1 for VBR       | bitrate                | target bitrate                        |
/// |76     | 4      | 160, 320, 380, ...        | frame_size             | samples per decoded frame             |
/// |80     | 4      | 1=VBR enabled, 0=disabled | vbr                    | vbr future state                      |
/// |84     | 4      | 1..n                      | frames_per_packet      | number of Speex frames per packet     |
/// |88     | 4      | usually 0                 | extra_headers          | number of extra header packets        |
/// |92     | 4      | 0                         | reserved1              | -                                     |
/// |96     | 4      | 0                         | reserved2              | -                                     |
/// |100    | 20+    | -                         | -                      | vorbis‐style comment header           |
/// </code>
/// </summary>
public class WaveFormatParser
{
    private const int CODEC_NAME_LENGTH = 8;
    private const int VERSION_LENGTH = 20;
    private const int FORMAT_FIELD_LENGTH = 4;
    private const int FRAME_SIZE_FIELD_LENGTH = 2;
    private const int QUALITY_FIELD_LENGTH = 1;
    private const int SALT_LENGTH = 5;

    public static WaveFormat ParseFormatData(string formatData)
    {
        var dataBytes = HexStringToByteArray(formatData);

        return ParseFormatData(dataBytes);
    }

    /// <summary>
    ///     Parse WaveFormat structure from sequence of bytes
    /// </summary>
    /// <param name="dataBytes">bytes array with WaveFormat data</param>
    /// <param name="isLegacyVer">is this array from legacy file version?</param>
    /// <returns>parsed WaveFormat</returns>
    public static WaveFormat ParseFormatData(byte[] dataBytes, bool isLegacyVer = false)
    {
        var fieldPos = 0;
        var format = new WaveFormat();
        fieldPos += FORMAT_FIELD_LENGTH * (isLegacyVer ? 3 : 2); // skip 2 params
        format.BytesPerSecond = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.FrameSize = dataBytes.ParseShort(ref fieldPos, FRAME_SIZE_FIELD_LENGTH);
        format.Quality = dataBytes.ParseShort(ref fieldPos, QUALITY_FIELD_LENGTH);
        fieldPos += SALT_LENGTH;
        format.CodecName = dataBytes.ParseString(ref fieldPos, CODEC_NAME_LENGTH);
        var speexVersionBytes = dataBytes.ParseSubArray(ref fieldPos, VERSION_LENGTH);
        format.Version = Encoding.UTF8.GetString(speexVersionBytes);
        fieldPos += FORMAT_FIELD_LENGTH * 2; // skip 2 params
        format.SampleRate = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.ChannelMode = GetBandMode(dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH));
        format.ModeBitStreamVersion = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.NbChannels = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.BitRate = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.PcmFrameSize = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        format.IsVBR = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH) != 0;
        format.FramesPerPacket = dataBytes.ParseInt(ref fieldPos, FORMAT_FIELD_LENGTH);
        return format;
    }

    private static BandMode GetBandMode(int mode)
    {
        switch (mode)
        {
            case 0:
                return BandMode.Narrow;

            case 1:
                return BandMode.Wide;

            case 2:
                return BandMode.UltraWide;

            default:
                throw new Exception("Invalid band mode, header data is damaged.");
        }
    }

    private static byte[] HexStringToByteArray(string hex)
    {
        hex = hex.Replace(" ", "");

        if (hex.Length % 2 != 0) throw new ArgumentException("Hex string must be in pairs by 2 values");

        var bytes = new byte[hex.Length / 2];

        for (var i = 0; i < hex.Length; i += 2) bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

        return bytes;
    }
}