using System.Text;
using DcrLib.Models;
using DcrLib.Services;
// ReSharper disable InconsistentNaming

namespace DcrLib.Files;

public class DcrFile
{
    private static byte[] META_CHUNK_BEGIN = Encoding.UTF8.GetBytes("meta#bgn");
    private static byte[] META_CHUNK_END = Encoding.UTF8.GetBytes("meta#end");
    private static byte[] DATA_CHUNK_BEGIN = Encoding.UTF8.GetBytes("data#bgn");
    private static byte[] DATA_CHUNK_END = Encoding.UTF8.GetBytes("data#end");
    private static byte[] VIDEO_CHUNK_BEGIN = Encoding.UTF8.GetBytes("cadr#bgn");
    private static byte[] VIDEO_CHUNK_END = Encoding.UTF8.GetBytes("cadr#end");
    private static byte[] ATTX_CHUNK_BEGIN = Encoding.UTF8.GetBytes("attx#bgn");
    private static byte[] ATTX_CHUNK_END = Encoding.UTF8.GetBytes("attx#end");
    private static byte[] PITCH_CHUNK_START = Encoding.UTF8.GetBytes("ptch#str");
    private static byte[] PITCH_CHUNK_STOP = Encoding.UTF8.GetBytes("ptch#stp");

    private static byte[] AUDIO_MARKER = Encoding.UTF8.GetBytes("audi#mrk");
    private static byte[] AUDIO_POINTER = Encoding.UTF8.GetBytes("audi#ptr");

    private const int CHUNK_LENGTH = 8;
    private const int PACKET_LENGTH = 119;

    public DcrAudioFormat AudioFormatData { get; private set; }
    private List<byte[]>? _speexData;

    public DcrFile(byte[] data)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentNullException(nameof(data));

        var fileAnalyzer = new FileAnalyzer(data);
        AudioFormatData = fileAnalyzer.GetAudioFormat(META_CHUNK_BEGIN, META_CHUNK_END, CHUNK_LENGTH);

        var audioDesc = new AudioDescriptor
        {
            DataBegin = DATA_CHUNK_BEGIN,
            DataEnd = DATA_CHUNK_END,
            AttxBegin = ATTX_CHUNK_BEGIN,
            AttxEnd = ATTX_CHUNK_END,
            VideoBegin = VIDEO_CHUNK_BEGIN,
            VideoEnd = VIDEO_CHUNK_END,
            AudioMarker = AUDIO_MARKER,
            AudioPointer = AUDIO_POINTER,
            ChunkLength = CHUNK_LENGTH,
            PacketLength = PACKET_LENGTH,
            ChannelsToDecode = AudioFormatData.Channels
        };

        _speexData = fileAnalyzer.GetSpeexData(audioDesc);
    }

    public List<byte[]> GetPCM()
    {
        return SpeexConverter.GetPcmAudioForAllChannels(_speexData, AudioFormatData, PACKET_LENGTH);
    }
}