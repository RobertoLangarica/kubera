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
	
	[XmlAttribute("destroyPoweUpProbability")]
	public int destroyPoweUpProbability;
	
	[XmlAttribute("rotatePoweUpProbability")]
	public int rotatePoweUpProbability;
	
	[XmlAttribute("wildPoweUpProbability")]
	public int wildPoweUpProbability;
	
	[XmlAttribute("blockPoweUpProbability")]
	public int blockPoweUpProbability;
	
	[XmlAttribute("grid")]
	public string grid;
}
