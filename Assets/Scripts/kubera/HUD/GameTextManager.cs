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

		//TODO: No se puede leer gameLanguage solo una vez ya que se puede modificar
		gamelanguage = UserDataManager.instance.language;
		//Debug.Log (gamelanguage);
	}

	/*
	 * @params idText{string}: El ID del texto que se quiere, este debe coincidir con el XML
	 * 
	 * @params language{string}: Idioma en particular o una cadena vacia para el lenguaje default
	 * 
	 */
	public string getTextByID(string idText,string language = string.Empty)
	{
		TextXML text = gameTextData.getTextByID(idText);
		string resultText = string.Empty;

		if(language == string.Empty)
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

		//TODO: En el ciclo de arriba hay que guardar el default y aqui asignarse. No volver a repetir el proceso
		if(resultText == string.Empty)
		{
			resultText = getTextByID(idText,"default");
		}

		return resultText;
	}

	/**
	 * * @params textToReplace{string[]}: Una lista de Textos o simbolos que se quieren remplazar. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a newText que sea del mismo tamaño.
	 * 
	 * @params newText{string[]}: Una lista de Textos o simbolos que reemplazaran los textos de textToReplace. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a textToReplace que sea del mismo tamaño.
	 **/ 
	//TODO: Renombrar parametros y funcion por algo mas claro( multipleReplace(target, textsToReplace, replacements)
	public string changeTextWords(string original,string[] textToReplace,string[] newText)
	{
		string resultText = original;

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
	}
}