using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.RecordingsInfo;

public class RecordingMirror
{
    [XmlAttribute("MMODE")] public int Mode { get; set; }

    [XmlAttribute("MDSTTYPE")] public int DstType { get; set; }

    [XmlElement("MFILE")] public string MFile { get; set; }

    [XmlElement("MSUBFOLD")] public string MSubfold { get; set; }
}