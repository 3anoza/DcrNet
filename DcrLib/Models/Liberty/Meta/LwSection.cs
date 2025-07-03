using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class LwSection
{
    [XmlElement("OF")] public string Of { get; set; }

    [XmlElement("DP")] public string Dp { get; set; }
}