using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.RecordingsInfo;

public class RecordingChannel
{
    [XmlAttribute("NUMBCH")] public int Number { get; set; }

    [XmlAttribute("TYPEDV")] public int DeviceType { get; set; }

    [XmlAttribute("LINEDV")] public int DeviceLine { get; set; }

    [XmlAttribute("SYNCH")] public int SyncGroup { get; set; }

    [XmlElement("NAMEDV")] public string Name { get; set; }

    [XmlElement("EXTDATA")] public string ExtData { get; set; }
}