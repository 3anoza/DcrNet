using System.Xml.Serialization;

namespace DcrNet.Models.Liberty.Meta;

public class Prop
{
    [XmlAttribute("VERAPP")] public int AppVer { get; set; }

    [XmlAttribute("BUILDAPP")] public int AppBuild { get; set; }

    [XmlAttribute("CNTEXP")] public int CntExp { get; set; }

    [XmlAttribute("SPLITNUM")] public int SplitNum { get; set; }

    [XmlElement("NAMEAPP")] public string AppName { get; set; }

    [XmlElement("USERLOG")] public string UserLog { get; set; }

    [XmlElement("USERREG")] public string UserRegHex { get; set; }

    [XmlElement("FILEID")] public string FileId { get; set; }

    [XmlElement("TIMECR")] public string TimeCreated { get; set; }

    [XmlElement("MACHINE")] public string Machine { get; set; }

    [XmlElement("WFMTNAME")] public string WFmtName { get; set; }
}