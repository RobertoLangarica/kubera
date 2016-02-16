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

	/**
	 * Devuelve el nivel indicado en lvlName
	 **/ 
	public Level getLevelByName(string lvlName)
	{
		foreach(Level l in _levels)
		{
			if(l.name.Equals(lvlName))
			{
				return l;
			}
		}

		return null;
	}

	public Level getLevelByNumber(int levelNumber)
	{
		return getLevelByName(levelNumber.ToString("0000"));
	}

	/**
	 * Devuelve una lista con los nombres de los niveles cargados en el xml
	 **/ 
	public List<string> getAllLevelsNames()
	{
		List<string> result = new List<string>();

		foreach(Level l in _levels)
		{
			result.Add(l.name);
		}

		return result;
	}

	/**
	 * Indica si el nivel especificado existe
	 **/ 
	public bool existLevel(int levelNumber)
	{
		return existLevel(levelNumber.ToString("0000"));
	}

	/**
	 * Indica si el nivel especificado existe
	 **/ 
	public bool existLevel(string levelName)
	{
		foreach(Level l in _levels)
		{
			if(l.name.Equals(levelName))
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Agrega un nuevo nivel a la lista de niveles
	 * */
	public void addLevel(Level level)
	{
		_levels.Add(level);
	}
}
