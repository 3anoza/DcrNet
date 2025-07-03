using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

[XmlRoot("Root")]
public class MetaXmlData
{
    [XmlElement("META")] public Meta Meta { get; set; }
}