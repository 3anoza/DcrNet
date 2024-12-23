using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace DcrLib.Services;

public class XmlEjector
{
    private XElement? _xmlRoot;
    public XmlEjector(string xmlContent)
    {
        if (string.IsNullOrEmpty(xmlContent)) 
            throw new ArgumentNullException(nameof(xmlContent));
        
        var fixedXml = Regex.Replace(xmlContent, @"[\x00-\x1F]", string.Empty);
        _xmlRoot = XDocument.Parse(fixedXml).Root;
    }

    public string GetFormatData()
    {
        var element = _xmlRoot.Element(Constants.META_SECTION)
            .Element(Constants.AUDIO_TAG)
            .Element(Constants.FORMAT_DATA_TAG);
        return element.Value;
    }

    public int GetChannelsAmount()
    {
        var element = _xmlRoot.Element(Constants.META_SECTION)
            .Element(Constants.AUDIO_TAG)
            .Element(Constants.CHANNELS_LIST_TAG);
        return element.Elements().Count();
    }
}