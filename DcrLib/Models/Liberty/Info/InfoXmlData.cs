using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Info;

[XmlRoot("Root")]
public class InfoXmlData
{
    [XmlElement("fl")] public FlSection Fl { get; set; }
}