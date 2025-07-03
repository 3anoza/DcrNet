using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Info;

public class FlSection
{
    [XmlElement("NOTE")] public string Note { get; set; }
}