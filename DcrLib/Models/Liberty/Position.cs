using System.Xml.Serialization;

namespace DcrNet.Models.Liberty;

public class Position
{
    [XmlAttribute("N64Low")] public long Low { get; set; }

    [XmlAttribute("N64High")] public long High { get; set; }

    public long GetPosition()
    {
        return (High << 32) | Low;
    }
}