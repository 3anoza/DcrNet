using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.RecordingsInfo;

public class RecordingSession
{
    [XmlAttribute("CTRLSUM")] public long CtrlSum { get; set; }

    [XmlElement("STARTREC")] public FileTime StartRec { get; set; }

    [XmlElement("STOPREC")] public FileTime StopRec { get; set; }

    [XmlElement("POSXBGN")] public Position PosXBegin { get; set; }

    [XmlElement("POSXEND")] public Position PosXEnd { get; set; }

    /// <summary>
    ///     Legacy property, that presents in some version
    /// </summary>
    [XmlElement("RECMIRR")]
    public RecordingMirror Mirror { get; set; }

    [XmlArray("LISTCHA")]
    [XmlArrayItem("RECCHA")]
    public List<RecordingChannel> AudioChannels { get; set; } = new();

    [XmlArray("LISTCHV")]
    [XmlArrayItem("RECCHV")]
    public List<RecordingChannel> VideoChannels { get; set; } = new();
}