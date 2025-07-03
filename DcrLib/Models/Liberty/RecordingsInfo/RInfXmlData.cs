using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.RecordingsInfo;

[XmlRoot("Root")]
public class RecordingsInfo
{
    [XmlElement("ISUBFOLD")] public string ISUBFOLD { get; set; }

    [XmlArray("LISTSESS")]
    [XmlArrayItem("RECSESS")]
    public List<RecordingSession> Sessions { get; set; } = new();
}