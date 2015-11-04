using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class Level
{
	[XmlAttribute("name")]
	public string name;
	
	[XmlAttribute("difficulty")]
	public int difficulty;
	
	[XmlAttribute("pool")]
	public string pool;
	
	[XmlAttribute("pieces")]
	public string pieces;
}
