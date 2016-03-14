using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class TextXML
{
	protected List<LanguageXML> _languages = new List<LanguageXML>();

	[XmlAttribute("id")]
	public string id;
	
	[XmlArray("languages"),XmlArrayItem("language")]
	public LanguageXML[] languages
	{
		set{_languages = new List<LanguageXML>(value);}
		get{return _languages.ToArray();}
	}
}