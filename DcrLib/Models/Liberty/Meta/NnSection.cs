using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class NnSection
{
    [XmlAttribute("NU")] public int Nu { get; set; }

    [XmlElement("NT")] public string Title { get; set; }

    [XmlElement("ND")] public string Notes { get; set; }
}