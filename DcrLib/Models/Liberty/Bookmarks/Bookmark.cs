using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Bookmarks;

public class Bookmark
{
    [XmlAttribute("IDMARK")] public long IdMark { get; set; }

    [XmlElement("POSMARK")] public Position? PosMark { get; set; }

    [XmlElement("CRTIMEUTC")] public FileTime? CrTimeUtc { get; set; }

    [XmlElement("CRTIMELOC")] public FileTime? CrTimeLoc { get; set; }

    [XmlElement("PUBLNOTE")] public string PublNote { get; set; }

    [XmlElement("SPEAKER")] public string Speaker { get; set; }
}