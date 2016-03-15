using UnityEngine;
using System.Collections;

/*
 * Clase que controla los textos del juego, para siempre seleccionar los del idioma adecuado
 */
public class GameTextManager
{
	protected static GameTextManager _instance;

	protected string gamelanguage;
	protected GameTextXML gameTextData;

	public static GameTextManager instance 
	{
		get {
			if(null == _instance)
			{
				_instance = new GameTextManager();
			}

			return _instance;
		}
	}

	GameTextManager()
	{
		//Se carga el XML de los textos del juego
		TextAsset tempTxt = (TextAsset)Resources.Load ("GameText");
		gameTextData = GameTextXML.LoadFromText(tempTxt.text);

		gamelanguage = UserDataManager.instance.language;
		//Debug.Log (gamelanguage);
	}

	/*
	 * Busca y regresa un texto que coincida con el ID que se le envie
	 * 
	 * @params idText{string}: El ID del texto que se quiere, este debe coincidir con el XML
	 * 
	 * @params language{string}: Este estring es para forzar un texto en un idioma en particular, ignorando el que se tenga guardado.
	 * 
	 * @params textToReplace{string[]}: Una lista de Textos o simbolos que se quieren remplazar. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a newText que sea del mismo tamaño.
	 * 
	 * @params newText{string[]}: Una lista de Textos o simbolos que reemplazaran los textos de textToReplace. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a textToReplace que sea del mismo tamaño.
	 */
	public string getTextByIDinLanguage(string idText,string language = "",string[] textToReplace = null,string[] newText = null)
	{
		TextXML text = gameTextData.getTextByID(idText);
		string resultText = "";

		if(language == "")
		{
			language = gamelanguage.ToString();
		}

		for(int i = 0;i < text.languages.Length;i++)
		{
			if(text.languages[i].id == language)
			{
				resultText = text.languages[i].text;
			}
		}

		if(resultText == "")
		{
			resultText = text.languages[text.languages.Length-1].text;
		}

		if(textToReplace != null && newText != null)
		{
			if(textToReplace.Length == newText.Length)
			{
				for(int i = 0;i < newText.Length;i++)
				{
					resultText = resultText.Replace(textToReplace[i],newText[i]);
				}
			}
		}

		return resultText;
	}
}