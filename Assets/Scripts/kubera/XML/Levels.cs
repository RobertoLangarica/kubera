using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

[XmlRoot("scrabtris")]
public class Levels
{
	protected List<Level> _levels = new List<Level>();
	
	public Levels(){}
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(Levels));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static Levels LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(Levels));
		return serializer.Deserialize(new StringReader(text)) as Levels;
	}

	[XmlArray("levels"),XmlArrayItem("level")]
	public Level[] levels
	{
		set{_levels = new List<Level>(value);}
		get{return _levels.ToArray();}
	}
}
