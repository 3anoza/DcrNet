using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class Meta
{
    [XmlElement("AUDIO")] public Audio Audio { get; set; }

    [XmlElement("VIDEO")] public Video Video { get; set; }

    [XmlElement("PROP")] public Prop Prop { get; set; }

    [XmlElement("DOCKNOTA")] public DockNota DockNota { get; set; }

    [XmlElement("CRYPT")] public Crypt Crypt { get; set; }
}