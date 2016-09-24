using UnityEngine;
using System.Collections;

public class UserDataManager 
{
	protected static UserDataManager _instance;

	//Version para controlar cambios a los datos en el futuro
	public const int version = 1;

	public static UserDataManager instance 
	{ 
		get {
			if(null == _instance)
			{
				_instance = new UserDataManager();
			}

			return _instance;
		}
	}

	UserDataManager()
	{
		if(PlayerPrefs.HasKey("version"))
		{
			if(PlayerPrefs.GetInt("version") != version)
			{
				//Resolvemos la version
				resolveVersion();
			}
		}
		else
		{
			createDefaultData();
		}

		PlayerPrefs.SetInt("startGame",1);
	}

	/**
	 * Resuelve los problemas entre versiones
	 **/ 
	protected void resolveVersion()
	{
		PlayerPrefs.SetInt("version",version);
	}

	/**
	 * Crea los datos por primera vez y con los valores por default
	 **/
	protected void createDefaultData()
	{
		PlayerPrefs.SetInt("version",version);
		PlayerPrefs.SetString("language",getOSLanguage());
		PlayerPrefs.SetInt("musicSetting",1);
		PlayerPrefs.SetInt("soundEffectsSetting",1);
	}

	/**
	 * Lenguaje en el que esta configurado el juego.
	 * @warning El valor esta en minusculas
	 **/ 
	public string language
	{
		get{return PlayerPrefs.GetString("language");}

		set
		{
			if(value.Equals(PlayerPrefs.GetString("language")))
			{
				return;
			}

			if(isLanguageSupported(value))
			{
				Debug.Log (value);
				PlayerPrefs.SetString("language",value);
			}
			else
			{
				//Lenguaje del sistema operativo
				//PlayerPrefs.SetString("language",getOSLanguage());
				PlayerPrefs.SetString("language","spanish");
			}

			//Que se configure el juego para el lenguaje (en este caso diccionarios)
			PersistentData.GetInstance().configureGameForLanguage();
		}
	}

	/**
	 * Elimina todos los datos del usuairo para el juego
	 **/ 
	public void cleanData()
	{
		Debug.Log("UM->Cleaning previous data.");
		PlayerPrefs.DeleteAll();
		createDefaultData();
	}

	/**
	 * Devuelve el idioma del OS si es soportado o deveulve un idioma default en caso de no serlo
	 * @warning Siempre se devuelve en minusculas
	 **/ 
	protected string getOSLanguage()
	{
		switch(Application.systemLanguage)
		{
		case SystemLanguage.Spanish:
		case SystemLanguage.English:
			return Application.systemLanguage.ToString().ToLowerInvariant();
		}

		return SystemLanguage.English.ToString().ToLowerInvariant();
	}

	/**
	 * Indica si un lenguaje es soportado por el juego
	 **/ 
	protected bool isLanguageSupported(string language)
	{
		//[HARDCODING] no pude poner el enum convertido a cadena en minusculas porque se queja el switch
		switch(language)
		{
		case "spanish":
		case "english":
			return true;
		}

		return false;
	}

	public void foo(){}

	public bool isRotatePowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("rotatePowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("rotatePowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isOnePiecePowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("onePiecePowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("onePiecePowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isWildCardPowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("wildCardPowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("wildCardPowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isWordHintPowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("wordHintPowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("wordHintPowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isDestroyPowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("destroyPowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("destroyPowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isDestroyNeighborsPowerUpUnlocked
	{
		get
		{
			return PlayerPrefs.GetInt("destroyNeighborsPowerUpUses") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("destroyNeighborsPowerUpUses",value == false ? 0 : 1);
		}
	}

	public bool isSoundEffectsActive
	{
		get
		{
			return PlayerPrefs.GetInt("soundEffectsSetting") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("soundEffectsSetting",value == false ? 0 : 1);
		}
	}

	public bool isMusicActive
	{
		get
		{
			return PlayerPrefs.GetInt("musicSetting") == 0 ? false : true;
		}		
		set
		{
			PlayerPrefs.SetInt("musicSetting",value == false ? 0 : 1);
		}
	}
}

