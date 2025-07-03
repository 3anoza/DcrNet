using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

[XmlRoot("Root")]
public class LegacyMetaXmlData
{
    [XmlElement("mt")] public Prop Prop { get; set; }
}

[XmlRoot("Root")]
public class LegacyDkNota : DockNota
{
}