using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

[XmlRoot("scrabtris")]
public class Levels
{
	protected List<Level> _levels = new List<Level>();

	[XmlIgnoreAttribute]
	public Dictionary<int,List<Level>> worlds;

	public Levels(){}
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(Levels));
		using(var stream = new StreamWriter(new FileStream(path, FileMode.Create),Encoding.UTF8))
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

	public void fillWorlds()
	{
		worlds = new Dictionary<int, List<Level>>();

		foreach(Level level in _levels)
		{
			if(!worlds.ContainsKey(level.world))
			{
				worlds.Add(level.world, new List<Level>());
			}
				
			worlds[level.world].Add(level);
		}

		foreach(KeyValuePair<int,List<Level>> item in worlds)
		{
			item.Value.Sort((x,y)=>string.Compare(x.name,y.name));
		}
	}

	public List<Level> getWorldByIndex(int index)
	{
		if(worlds.ContainsKey(index))
		{
			return worlds[index];
		}

		return null;
	}

	public List<Level> getLevelWorld(Level level)
	{
		return getWorldByIndex(level.world);
	}

	public List<Level> getLevelWorld(string levelName)
	{
		return getLevelWorld(getLevelByName(levelName));
	}
}
