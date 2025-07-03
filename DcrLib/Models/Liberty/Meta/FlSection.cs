using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class FlSection
{
    [XmlElement("FN")] public List<FieldNota> Fields { get; set; }
}