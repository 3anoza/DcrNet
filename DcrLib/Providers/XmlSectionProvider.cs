using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace DcrNet.Providers;

public class XmlSectionProvider : IXmlSectionProvider
{
    public TResult GetSectionData<TResult>(string xmlContent)
    {
        var xmlSerializer = new XmlSerializer(typeof(TResult));

        var fixedXml = Regex.Replace(xmlContent, @"[\x00-\x1F]", string.Empty);
        var xmlReader = XmlReader.Create(new StringReader(fixedXml));
        return (TResult)xmlSerializer.Deserialize(xmlReader)!;
    }
}