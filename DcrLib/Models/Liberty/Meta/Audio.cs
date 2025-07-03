using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class Audio
{
    [XmlAttribute("USECHNL")] public int UseChnl { get; set; }

    [XmlElement("FMTNAME")] public string FmtName { get; set; }

    [XmlElement("FMTDATA")] public string FmtDataHex { get; set; }

    [XmlArray("LISTCHNL")]
    [XmlArrayItem("ITEM")]
    public List<ChannelItem> Channels { get; set; }
}