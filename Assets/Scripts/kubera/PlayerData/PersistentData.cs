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
	[HideInInspector]public Level currentLevel;
	[HideInInspector]public Levels levelsData;
	[HideInInspector]public ABCDataStructure abcStructure;

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

		abcStructure = FindObjectOfType<ABCDataStructure>();
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

		//Alfabeto
		TextAsset abc = Resources.Load("ABCData/ABC_"+language) as TextAsset;
		abcStructure.initializeAlfabet(abc.text);

		if(loadSerializedDictionary)
		{
			loadAndDeserializeDictionary(language);	
		}
		else
		{
			//Diccionario
			abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;
			abcStructure.onDictionaryFinished += onDictionaryFinishedCallback;	
			abcStructure.processDictionary(abc.text);	
		}

		//CurrentLevel
		currentLevel = levelsData.getLevelByNumber(levelNumber);
	}

	private void onDictionaryFinishedCallback()
	{
		abcStructure.onDictionaryFinished -= onDictionaryFinishedCallback;
		serializeAndSaveDictionary(currentLanguage);
		onDictionaryFinished();
	}


	private void serializeAndSaveDictionary(string language)
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		string path = Application.dataPath+"/Resources/"+"ABCData/DICTIONARY_"+language+".bytes";

		File.Delete(path);
		FileStream fs = new FileStream(path,FileMode.Create);
		ABCNodeSerializer serializer = new ABCNodeSerializer();

		serializer.Serialize(fs,abcStructure.tree.root);
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

		string path = Application.dataPath+"/Resources/"+"ABCData/DICTIONARY_"+language+".bytes";

		ABCNode data = null;
		FileStream fs = File.OpenRead(path);
		ABCNodeSerializer serializer = new ABCNodeSerializer();

		data = (ABCNode)serializer.Deserialize(fs, null, typeof(ABCNode));
		fs.Close();
		fs.Dispose();

		abcStructure.tree = new ABCTree();
		abcStructure.tree.root = data;

		Debug.Log("Diccionario deserializado con exito");
	}

	public void addWordToDictionary(string word, string language = "")
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		//Diccionario
		//TextAsset abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;
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