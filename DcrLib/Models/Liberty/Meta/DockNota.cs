using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class DockNota
{
    [XmlAttribute("DT")] public int Dt { get; set; }

    [XmlAttribute("VJ")] public int Vj { get; set; }

    [XmlAttribute("VN")] public int Vn { get; set; }

    [XmlElement("NN")] public NnSection Nn { get; set; }

    [XmlElement("LW")] public LwSection? Lw { get; set; }

    [XmlElement("FL")] public FlSection Fl { get; set; }
}