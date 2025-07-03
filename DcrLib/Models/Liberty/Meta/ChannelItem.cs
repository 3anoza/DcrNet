using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class ChannelItem
{
    [XmlElement("CHNAME")] public string Name { get; set; }

    [XmlElement("CHDESC")] public string Description { get; set; }
}