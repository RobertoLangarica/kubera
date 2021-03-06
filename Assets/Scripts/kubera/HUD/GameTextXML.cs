﻿using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

[XmlRoot("gameText")]
public class MultiLanguageTextXML
{
	protected List<TextXML> _texts = new List<TextXML>();
	
	[XmlArray("texts"),XmlArrayItem("text")]
	public TextXML[] texts
	{
		set{_texts = new List<TextXML>(value);}
		get{return _texts.ToArray();}
	}
	
	public static MultiLanguageTextXML LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(MultiLanguageTextXML));
		return serializer.Deserialize(new StringReader(text)) as MultiLanguageTextXML;
	}

	public TextXML getTextByID(string id)
	{
		for(int i = 0;i < texts.Length;i++)
		{
			if(texts[i].id == id)
			{
				return texts[i];
			}
		}
		return null;
	}
}