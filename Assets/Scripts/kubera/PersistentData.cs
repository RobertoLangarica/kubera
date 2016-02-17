using UnityEngine;
using System.Collections;
using System.IO;
using ABC;

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
	[HideInInspector]public Level currentLevel;
	[HideInInspector]public Levels levelsData;
	[HideInInspector]public ABCDataStructure abcStructure;

	//El idioma en que se encuentra configurado actualmente el juego
	private string currentLanguage;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	void Awake() 
	{
		if(instance != null)
		{
			//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
			destroyed = true;
			DestroyImmediate(instance);
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
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		//Niveles
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels_"+language);
		levelsData = Levels.LoadFromText(tempTxt.text);

		//Alfabeto
		TextAsset abc = Resources.Load("ABCData/ABC_"+language) as TextAsset;
		abcStructure.initializeAlfabet(abc.text);

		//Diccionario
		abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;
		abcStructure.onDictionaryFinished += onDictionaryFinishedCallback;	
		abcStructure.processDictionary(abc.text);


		//CurrentLevel
		currentLevel = levelsData.getLevelByNumber(levelNumber);
	}

	private void onDictionaryFinishedCallback()
	{
		abcStructure.onDictionaryFinished -= onDictionaryFinishedCallback;	
		onDictionaryFinished();
	}

	public void addWordToDictionary(string word, string language = "")
	{
		if(language == "")
		{
			language = UserDataManager.instance.language;
		}

		//Diccionario
		TextAsset abc = Resources.Load("ABCData/WORDS_"+language) as TextAsset;
		StreamWriter writer = new StreamWriter(Application.dataPath+"/Resources/"+"ABCData/WORDS_"+language+".txt",true);
		writer.Write("\n"+word);
		writer.Close();
		writer.Dispose();
	}

	/**
	 * Indica el indice de nivel que el usuario va jugar
	 **/ 
	public void setLevelNumber(int value)
	{
		levelNumber = value;

		if(levelsData != null)
		{
			currentLevel = levelsData.getLevelByNumber(levelNumber);
		}
	}
}