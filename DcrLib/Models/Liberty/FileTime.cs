using System.Xml.Serialization;

namespace DcrNet.Models.Liberty;

public class FileTime
{
    [XmlAttribute("FTLow")] public long Low { get; set; }

    [XmlAttribute("FTHigh")] public long High { get; set; }

    public long GetTimestamp()
    {
        return (High << 32) | (Low & 0xFFFFFFFF);
    }
}