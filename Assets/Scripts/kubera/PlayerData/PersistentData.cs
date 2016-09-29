using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ABC;
using ABC.Tree;
using ABCSerializer;
using Data;

/**
 * clase que ponemos en un prefab que va existir
 * desde el inicio del juego con informacion que debe ser permanente o compartida sin
 * necesidad de ir directamente dirigida a alguien.
 *
 * */
public class PersistentData : Manager<PersistentData>
{
	public delegate void DNotify();
	public DNotify onDictionaryFinished;

	public bool loadSerializedDictionary = true;
	public int maxWordLength = 10;
	[HideInInspector]public int levelNumber = -1;
	[HideInInspector]public Level currentLevel;
	[HideInInspector]public string lastLevelReachedName;
	[HideInInspector]public int lastLevelStars = -1;
	[HideInInspector]public int lastLevelPoints = -1;
	[HideInInspector]public bool nextLevelIsReached;
	[HideInInspector]public Levels levelsData;
	[HideInInspector]public ABCDictionary abcDictionary;

	//El idioma en que se encuentra configurado actualmente el juego
	private string currentLanguage;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	[HideInInspector]public int currentWorld  =-1;

	[HideInInspector]
	public bool fromLevelBuilder;
	[HideInInspector]
	public bool fromGameToEdit;
	[HideInInspector]
	public bool fromGameToLevels = false;

	[HideInInspector]
	public bool fromLevelsToGame = false;

	[HideInInspector]
	public int startLevel = 1;

	[HideInInspector]
	public string lastLevelPlayedName = "";
	[HideInInspector]
	public bool fromLoose;

	protected override void Awake()
	{
		base.Awake ();
		setLevelNumber(levelNumber);
	}

	void Start()
	{
		if(destroyed)
		{
			return;
		}

		abcDictionary = FindObjectOfType<ABCDictionary>();
		onDictionaryFinished += foo;
		//HARDCODING
		configureGameForLanguage("spanish");//english,spanish

		/*currentWorld = */
		//print((LevelsDataManager.GetInstance () as LevelsDataManager).getCurrentData ().levels [((LevelsDataManager.GetInstance () as LevelsDataManager).getCurrentData ().levels.Count - 1)]);
	}

	private void foo(){}

	/**
	 * Configura el juego para el lenguaje que tiene UserDataManager
	 * @param language el lenguaje para configurar, si se deja vacio se utiliza el de UserDataManager.
	 **/
	public void configureGameForLanguage(string language = "")
	{
		if(language == currentLanguage)
		{
			onDictionaryFinished();
			return;
		}
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}
		else
		{
			UserDataManager.instance.language = language;
		}

		currentLanguage = language;

		//Niveles
		TextAsset levels = (TextAsset)Resources.Load ("levels_"+language);
		levelsData = Levels.LoadFromText(levels.text);
		levelsData.fillWorlds();
		Resources.UnloadAsset(levels);

		//CurrentLevel
		currentLevel = levelsData.getLevelByNumber(levelNumber);

		//Alfabeto
		TextAsset abc = Resources.Load("ABCData/ABC_"+language) as TextAsset;
		abcDictionary.initializeAlfabet(abc.text);
		Resources.UnloadAsset(abc);

		//Diccionario
		#if UNITY_EDITOR
		if(loadSerializedDictionary)
		{
			loadAndDeserializeDictionary(language);
			onDictionaryFinished();
		}
		else
		{
			//Diccionario
			//abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;

			StreamReader stream = new StreamReader(Application.dataPath+"/ABCData/WORDS_"+language+".txt");
			abcDictionary.onDictionaryFinished += onDictionaryFinishedCallback;
			//abcDictionary.processDictionary(abc.text, maxWordLength);
			abcDictionary.processDictionary(stream.ReadToEnd(), maxWordLength);
			stream.Close();
			stream.Dispose();
		}
		#else
			loadAndDeserializeDictionary(language);
			onDictionaryFinished();
		#endif
	}

	private void onDictionaryFinishedCallback()
	{
		abcDictionary.onDictionaryFinished -= onDictionaryFinishedCallback;
		serializeAndSaveDictionary(currentLanguage);
		onDictionaryFinished();
	}


	public void serializeAndSaveDictionary(string language)
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		string path = Application.dataPath+"/Resources/"+"ABCData/DICTIONARY_"+language+".bytes";

		File.Delete(path);
		FileStream fs = new FileStream(path,FileMode.Create);
		ABCNodeSerializer serializer = new ABCNodeSerializer();

		serializer.Serialize(fs,abcDictionary.getTreeRoot());
		fs.Close();
		fs.Dispose();

		if(_mustShowDebugInfo)
		{
			Debug.Log("Diccionario guardado y serializado con exito");
		}
	}

	private void loadAndDeserializeDictionary(string language)
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		ABCNode data = null;
		ABCNodeSerializer serializer = new ABCNodeSerializer();
		TextAsset resource = Resources.Load("ABCData/DICTIONARY_"+language) as TextAsset;
		Stream source = new MemoryStream(resource.bytes);
		data = (ABCNode)serializer.Deserialize(source, null, typeof(ABCNode));

		abcDictionary.setTreeRoot(data);
		Resources.UnloadAsset(resource);

		if(_mustShowDebugInfo)
		{
			Debug.Log("Diccionario deserializado con exito");
		}
	}

	public void addWordToDictionary(string word, string language = "")
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		StreamWriter writer = new StreamWriter(Application.dataPath+"/Resources/"+"ABCData/WORDS_"+language+".txt",true);
		writer.Write("\n"+word);
		writer.Close();
		writer.Dispose();


	}

	/**
	 * Indica el indice de nivel que el usuario va jugar
	 **/
	public void setLevelNumber(int value,bool fromBuilder = false)
	{
		levelNumber = value;

		if(levelsData != null)
		{
			currentLevel = levelsData.getLevelByNumber(levelNumber);

			if(fromBuilder)
			{
				fromLevelBuilder = true;
			}
		}
	}

	//HACK para pruebas
	public Level getRandomLevel()
	{
		return levelsData.levels[Random.Range(0,levelsData.levels.Length-1)];
	}

	public Level getLevelByIndex(int index)
	{
		return levelsData.levels[index];
	}

	public Level getFirstLevel()
	{
		return levelsData.levels[0];
	}

	public Level getNextLevel()
	{
		setLevelNumber (startLevel);

		startLevel++;

		if (currentLevel == null)
		{
			startLevel = 1;
			setLevelNumber (startLevel);
		}

		return currentLevel;
	}
}
