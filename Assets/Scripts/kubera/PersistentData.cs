using UnityEngine;
using System.Collections;

/**
 * clase que ponemos en un prefab que va existir 
 * desde el inicio del juego con informacion que debe ser permanente o compartida sin 
 * necesidad de ir directamente dirigida a alguien.
 * 
 * */
public class PersistentData : MonoBehaviour 
{
	//Solo existe una sola instancia en todo el juego de este objeto
	public static PersistentData instance;

	[HideInInspector]public string currentLevelName;
	[HideInInspector]public Level currentLevel;
	[HideInInspector]public Levels levelsData;
	[HideInInspector]public ABCDataStructure abcStructure;

	private int m_currentLevelIndex;

	//El idioma en que se encuentra configurado actualmente el juego
	private string currentLanguage;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	void Awake() 
	{
		if(instance)
		{
			//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
			destroyed = true;
			Destroy(instance);
			return;
		}

		instance = this;

		DontDestroyOnLoad(this);

		currentLevelIndex = 1;
	}

	void Start()
	{
		if(destroyed)
		{
			return;
		}

		abcStructure = FindObjectOfType<ABCDataStructure>();
		
		configureGameForLanguage();

		print(currentLevelName);
	}

	/**
	 * Configura el juego para el lenguaje que tiene UserDataManager
	 **/ 
	public void configureGameForLanguage()
	{
		//Niveles
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels_"+UserDataManager.instance.language);
		levelsData = Levels.LoadFromText(tempTxt.text);

		//Alfabeto
		TextAsset abc = Resources.Load("ABCData/ABC_"+UserDataManager.instance.language) as TextAsset;
		abcStructure.initializeAlfabet(abc.text);

		//Diccionario
		abc = Resources.Load("ABCData/WORDS_"+UserDataManager.instance.language) as TextAsset;
		abcStructure.processDictionary(abc.text);


		//CurrentLevel
		currentLevel = levelsData.getLevelByName(currentLevelName);
	}

	/**
	 * Indica el indice de nivel que el usuario va jugar
	 **/ 
	public int currentLevelIndex
	{
		set
		{
			m_currentLevelIndex = value;
			currentLevelName = m_currentLevelIndex.ToString("0000");

			if(levelsData != null)
			{
				currentLevel = levelsData.getLevelByName(currentLevelName);
			}
		}

		get
		{
			return m_currentLevelIndex;
		}
	}
}