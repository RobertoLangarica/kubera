﻿using UnityEngine;
using System.Collections;

/*
 * Clase que controla los textos del juego, para siempre seleccionar los del idioma adecuado
 */
public class MultiLanguageTextManager
{
	public const string SCORE_HUD_TITLE_ID 					= "hudScore";
	public const string OBJECTIVE_POPUP_BY_POINTS_ID 		= "goalByPointsObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_WORDS_ID 		= "goalByWordsObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_LETTERS_ID 		= "goalByLettersObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_OBSTACLES_ID 	= "goalByObstaclesObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_1_WORD_ID 		= "goalBy1WordObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_SYNONYMOUS_ID 	= "goalBySynonymousObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_ANTONYM_ID 		= "goalByAntonymObjectivePopUp";
	public const string GOAL_CONDITION_BY_POINT_ID 			= "goalByPointsCondition";
	public const string GOAL_CONDITION_BY_WORDS_ID 			= "goalByWordsCondition";
	public const string GOAL_CONDITION_BY_LETTERS_ID 		= "goalByLettersCondition";
	public const string GOAL_CONDITION_BY_OBSTACLES_ID 		= "goalByObstaclesCondition";
	public const string GOAL_CONDITION_BY_1_WORD_ID 		= "goalBy1WordCondition";
	public const string GOAL_CONDITION_BY_SYNONYMOUS_ID 	= "goalBySynonymousCondition";
	public const string GOAL_CONDITION_BY_ANTONYM_ID 		= "goalByAntonymCondition";
	public const string EXIT_POPUP_ID 						= "exitText";


	protected static MultiLanguageTextManager _instance;

	protected string _gameLanguage;
	protected MultiLanguageTextXML gameTextData;

	public string gameLanguage
	{
		get
		{
			return UserDataManager.instance.language;
		}
	}

	public static MultiLanguageTextManager instance 
	{
		get {
			if(null == _instance)
			{
				_instance = new MultiLanguageTextManager();
			}

			return _instance;
		}
	}

	MultiLanguageTextManager()
	{
		//Se carga el XML de los textos del juego
		TextAsset tempTxt = (TextAsset)Resources.Load ("GameText");
		gameTextData = MultiLanguageTextXML.LoadFromText(tempTxt.text);
	}

	/*
	 * @params idText{string}: El ID del texto que se quiere, este debe coincidir con el XML
	 * 
	 * @params language{string}: Idioma en particular o una cadena vacia para el lenguaje default
	 * 
	 */
	public string getTextByID(string idText,string language = "")
	{
		TextXML text = gameTextData.getTextByID(idText);
		string resultText = string.Empty;
		string defaultText = string.Empty;

		if(language == string.Empty)
		{
			language = gameLanguage;
		}

		for(int i = 0;i < text.languages.Length;i++)
		{
			if(text.languages[i].id == language)
			{
				resultText = text.languages[i].text;
			}
			else if (text.languages[i].id == "default")
			{
				defaultText = text.languages[i].text;
			}
		}

		if(resultText == string.Empty)
		{
			resultText = defaultText;
		}

		return resultText;
	}


	//TODO: Comentarios obsoletos
	/**
	 * * @params textToReplace{string[]}: Una lista de Textos o simbolos que se quieren remplazar. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a newText que sea del mismo tamaño.
	 * 
	 * @params newText{string[]}: Una lista de Textos o simbolos que reemplazaran los textos de textToReplace. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a textToReplace que sea del mismo tamaño.
	 **/ 
	public string multipleReplace(string target,string[] textToReplace,string[] replacements)
	{
		string resultText = target;

		if(textToReplace != null && replacements != null)
		{
			if(textToReplace.Length == replacements.Length)
			{
				for(int i = 0;i < replacements.Length;i++)
				{
					resultText = resultText.Replace(textToReplace[i],replacements[i]);
				}
			}
			//DONE: mandar logError
			else
			{
				//TODO: estos mensajes son para alguein que no sabe nada del codigo que manda llamar
				//TODO: Los arreglos son obviamente diferentes y el error aqui es un tema de diferentes largos
				Debug.LogWarning("EL TEXTO NO SE MODIFICO, los arreglos para modificar el texto son diferentes");
			}
		}

		return resultText;
	}
}