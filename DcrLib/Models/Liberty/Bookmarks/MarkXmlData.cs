using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Bookmarks;

[XmlRoot("Root")]
public class MarkXmlData
{
    [XmlElement("bm")] public List<Bookmark> Bookmarks { get; set; }
}