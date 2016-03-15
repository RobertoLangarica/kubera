using System.Xml;
using System.Xml.Serialization;

public class LanguageXML
{
	[XmlAttribute("id")]
	public string id;
	
	[XmlAttribute("text")]
	public string text;
}