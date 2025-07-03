using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class FieldNota
{
    [XmlAttribute("UF")] public int Uf { get; set; }

    [XmlAttribute("RF")] public int Rf { get; set; }

    [XmlAttribute("IF")] public int If { get; set; }

    [XmlAttribute("SH")] public int Sh { get; set; }

    [XmlAttribute("DL")] public int Dl { get; set; }

    [XmlAttribute("TF")] public int Tf { get; set; }

    [XmlAttribute("NV")] public int Nv { get; set; }

    [XmlElement("TL")] public string Label { get; set; }

    [XmlElement("TP")] public string ValuePlain { get; set; }

    [XmlElement("DF")] public string Default { get; set; }

    [XmlElement("XL")] public object? ExtraEmpty { get; set; }
}