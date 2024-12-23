using DcrLib.Models;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using DcrLib.Extensions;
using NSpeex;

namespace DcrLib.Services;

public class DcrParser
{
    private const int CODEC_NAME_LENGTH = 8;
    private const int VERSION_LENGTH = 20;
    private const int FORMAT_FIELD_LENGTH = 4;

    public static DcrAudioFormat ParseFormatData(string formatData)
    {
        var dataBytes = HexStringToByteArray(formatData);

        int fieldPos = 20;
        DcrAudioFormat format = new DcrAudioFormat();

        format.CodecName = dataBytes.ParseString(ref fieldPos, CODEC_NAME_LENGTH);
        byte[] speexVersionBytes = dataBytes.ParseSubArray(ref fieldPos, VERSION_LENGTH);
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

        if (hex.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string must be in pairs by 2 values");
        }

        byte[] bytes = new byte[hex.Length / 2];

        for (int i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }
}