using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class Crypt
{
    [XmlAttribute("HAVEREC")] public bool HaveRec { get; set; }

    [XmlAttribute("HAVEOVE")] public bool HaveOve { get; set; }

    [XmlElement("CHECKREC")] public string CheckRecHex { get; set; }

    [XmlElement("CHECKOVE")] public string CheckOveHex { get; set; }

    [XmlElement("PASSREC")] public string PassRecHex { get; set; }

    [XmlElement("PASSOVE")] public string PassOveHex { get; set; }
}