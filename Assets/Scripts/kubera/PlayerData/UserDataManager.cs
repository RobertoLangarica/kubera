using UnityEngine;
using System.Collections;

public class UserDataManager 
{
	protected static UserDataManager _instance;

	//Version para controlar cambios a los datos en el futuro
	public const int version = 0;

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
	 * Crea los datos por primera vez y con los valores por default
	 * 
	 **/
	protected void createDefaultData()
	{
		//se inicializa a la version actual
		PlayerPrefs.SetInt("version",version);

		//Lenguaje del sistema operativo
		PlayerPrefs.SetString("language",getOSLanguage());
	}

	/**
	 * Lenguaje en el que esta configurado el juego.
	 * @warning El valor esta en minusculas
	 **/ 
	public string language
	{
		get{return PlayerPrefs.GetString("language");}
		set{PlayerPrefs.SetString("language",value);}
	}

	/**
	 * Resuelve los problemas entre versiones
	 **/ 
	protected void resolveVersion()
	{
		PlayerPrefs.SetInt("version",version);
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

	public void foo(){}
}

