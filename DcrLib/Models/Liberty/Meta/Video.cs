using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class Video
{
    [XmlArray("LISTSTRM")]
    [XmlArrayItem("ITEM")]
    public List<VideoStream> Streams { get; set; }
}