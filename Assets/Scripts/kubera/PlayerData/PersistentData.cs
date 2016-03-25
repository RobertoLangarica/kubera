﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ABC;
using ABC.Tree;
using ABCSerializer;

/**
 * clase que ponemos en un prefab que va existir 
 * desde el inicio del juego con informacion que debe ser permanente o compartida sin 
 * necesidad de ir directamente dirigida a alguien.
 * 
 * */
public class PersistentData : MonoBehaviour 
{
	//Solo existe una sola instancia en todo el juego de este objeto
	public static PersistentData instance = null;

	public delegate void DNotify();
	public DNotify onDictionaryFinished;

	public int levelNumber = 1;
	public bool loadSerializedDictionary = true;
	public int maxWordLength = 10;
	[HideInInspector]public Level currentLevel;
	[HideInInspector]public Levels levelsData;
	[HideInInspector]public ABCDictionary abcDictionary;

	//El idioma en que se encuentra configurado actualmente el juego
	private string currentLanguage;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	[HideInInspector]
	public bool fromLevelBuilder;
	[HideInInspector]
	public bool fromGameToEdit;

	void Awake() 
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag ("persistentData");
		for(int i=1; i< go.Length; i++)
		{
			DestroyImmediate (go [i]);
		}

		//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
		if (go.Length > 1) 
		{
			destroyed = true;
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);
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
		configureGameForLanguage();
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

		currentLanguage = language;

		//Niveles
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels_"+language);
		levelsData = Levels.LoadFromText(tempTxt.text);

		//CurrentLevel
		currentLevel = levelsData.getLevelByNumber(levelNumber);

		//Alfabeto
		TextAsset abc = Resources.Load("ABCData/ABC_"+language) as TextAsset;
		abcDictionary.initializeAlfabet(abc.text);

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
			abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;
			abcDictionary.onDictionaryFinished += onDictionaryFinishedCallback;	
			abcDictionary.processDictionary(abc.text, maxWordLength);	
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

		Debug.Log("Diccionario guardado y serializado con exito");
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

		Debug.Log("Diccionario deserializado con exito");
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

	public Level getRandomLevel()
	{
		return levelsData.levels[Random.Range(0,levelsData.levels.Length)];
	}
}