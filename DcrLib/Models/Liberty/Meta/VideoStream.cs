using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class VideoStream
{
    [XmlAttribute("RESX")] public int ResX { get; set; }

    [XmlAttribute("RESY")] public int ResY { get; set; }

    [XmlAttribute("FOURCC")] public long FourCC { get; set; }

    [XmlAttribute("BITS")] public int Bits { get; set; }

    [XmlAttribute("QUAL")] public int Qual { get; set; }

    [XmlAttribute("RATE")] public int Rate { get; set; }

    [XmlAttribute("VPTC")] public int Vptc { get; set; }

    [XmlElement("BMIDATA")] public string BmiDataHex { get; set; }

    [XmlElement("CODEC")] public string Codec { get; set; }

    [XmlElement("DRVFILE")] public string DriverFile { get; set; }

    [XmlElement("VERFILE")] public string VerFile { get; set; }

    [XmlElement("VERPROD")] public string VerProd { get; set; }

    [XmlElement("SRCNAME")] public string SourceName { get; set; }
}