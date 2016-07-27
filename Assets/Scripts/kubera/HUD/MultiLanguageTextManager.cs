using UnityEngine;
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
	public const string GOAL_CONDITION_BY_POINT_UP_ID		= "goalByPointsConditionUP";
	public const string GOAL_CONDITION_BY_POINT_ID 			= "goalByPointsCondition";
	public const string GOAL_CONDITION_BY_WORDS_UP_ID 		= "goalByWordsConditionUP";
	public const string GOAL_CONDITION_BY_WORDS_ID 			= "goalByWordsCondition";
	public const string GOAL_CONDITION_BY_LETTERS_ID 		= "goalByLettersCondition";
	public const string GOAL_CONDITION_BY_OBSTACLES_UP_ID 	= "goalByObstaclesConditionUP";
	public const string GOAL_CONDITION_BY_OBSTACLES_ID 		= "goalByObstaclesCondition";
	public const string GOAL_CONDITION_BY_1_WORD_ID 		= "goalBy1WordCondition";
	public const string GOAL_CONDITION_BY_SYNONYMOUS_ID 	= "goalBySynonymousCondition";
	public const string GOAL_CONDITION_BY_ANTONYM_ID 		= "goalByAntonymCondition";
	public const string EXIT_POPUP_ID 						= "exitText";
	public const string WIN_TEXT_POPUP_ID					= "WinTextPopUpID";
	public const string STARTGAME_TEXT_POPUP_ID				= "StartTextPopUpID";
	public const string NO_MOVEMENTS_POPUP_ID				= "noMovementsPopUpID";
	public const string NO_PIECES_POPUP_ID					= "noPiecesPopUpID";

	public const string TUTORIAL_LV1_PHASE1					= "Tutorial_lvl1_phase1";
	public const string TUTORIAL_LV1_PHASE2					= "Tutorial_lvl1_phase2";
	public const string TUTORIAL_LV1_PHASE3					= "Tutorial_lvl1_phase3";
	public const string TUTORIAL_LV1_PHASE4					= "Tutorial_lvl1_phase4";
	public const string TUTORIAL_LV1_PHASE5					= "Tutorial_lvl1_phase5";
	public const string TUTORIAL_LV1_PHASE6					= "Tutorial_lvl1_phase6";
	public const string TUTORIAL_LV1_PHASE7					= "Tutorial_lvl1_phase7";
	public const string TUTORIAL_LV1_PHASE8					= "Tutorial_lvl1_phase8";
	public const string TUTORIAL_LV1_PHASE9					= "Tutorial_lvl1_phase9";

	public const string TUTORIAL_LV2_PHASE1					= "Tutorial_lvl2_phase1";
	public const string TUTORIAL_LV2_PHASE2					= "Tutorial_lvl2_phase2";
	public const string TUTORIAL_LV2_PHASE3					= "Tutorial_lvl2_phase3";
	public const string TUTORIAL_LV2_PHASE4					= "Tutorial_lvl2_phase4";

	public const string TUTORIAL_LV3_PHASE1					= "Tutorial_lvl3_phase1";
	public const string TUTORIAL_LV3_PHASE2A				= "Tutorial_lvl3_phase2A";
	public const string TUTORIAL_LV3_PHASE2B				= "Tutorial_lvl3_phase2B";
	public const string TUTORIAL_LV3_PHASE3					= "Tutorial_lvl3_phase3";

	public const string TUTORIAL_LV4_PHASE1A				= "Tutorial_lvl4_phase1A";
	public const string TUTORIAL_LV4_PHASE1B				= "Tutorial_lvl4_phase1B";
	public const string TUTORIAL_LV4_PHASE2					= "Tutorial_lvl4_phase2";

	public const string TUTORIAL_LV8_PHASE1					= "Tutorial_lvl8_phase1";
	public const string TUTORIAL_LV8_PHASE2					= "Tutorial_lvl8_phase2";

	public const string TUTORIAL_LV22_PHASE1				= "Tutorial_lvl22_phase1";
	public const string TUTORIAL_LV22_PHASE2				= "Tutorial_lvl22_phase2";

	public const string TUTORIAL_LV37_PHASE1				= "Tutorial_lvl37_phase1";
	public const string TUTORIAL_LV37_PHASE2				= "Tutorial_lvl37_phase2";

	public const string TUTORIAL_LV52_PHASE1				= "Tutorial_lvl52_phase1";
	public const string TUTORIAL_LV52_PHASE2				= "Tutorial_lvl52_phase2";

	public const string TUTORIAL_LV64_PHASE1				= "Tutorial_lvl64_phase1";
	public const string TUTORIAL_LV64_PHASE2				= "Tutorial_lvl64_phase2";
	public const string TUTORIAL_LV64_PHASE3				= "Tutorial_lvl64_phase3";
	public const string TUTORIAL_LV64_PHASE4				= "Tutorial_lvl64_phase4";

	public const string NO_GEMS_POPUP_TITLE					= "NoGemsPopUpTitle";
	public const string NO_GEMS_POPUP_INFO					= "NoGemsPopUpInfo";
	public const string NO_GEMS_POPUP_BUTTON				= "NoGemsPopUpButton";



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
		
	/**
	 * @params target{string}: La cadena en la que se buscará el texto de textToReplace para poder sustituirlo
	 * 
	 * @params textToReplace{string[]}: Una lista de Textos o simbolos que se quieren remplazar. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a newText que sea del mismo tamaño.
	 * 
	 * @params replacements{string[]}: Una lista de Textos o simbolos que reemplazaran los textos de textToReplace. Usualmente para agregar texto dinamico.
	 * 									Este sera ignorado si no se pasa tambien valor a textToReplace que sea del mismo tamaño.
	 **/ 
	public string multipleReplace(string target,string[] textToReplace,string[] replacements)
	{
		string resultText = target;

		if(textToReplace != null && replacements != null)
		{
			if (textToReplace.Length == replacements.Length) 
			{
				for (int i = 0; i < replacements.Length; i++) 
				{
					resultText = resultText.Replace (textToReplace [i], replacements [i]);
				}
			} 
			else 
			{
				//DONE: Cambie el mensaje a ingles y agrege que el error es por diferentes lengths
				Debug.LogWarning("THE TEXT HAS NOT CHANGED, because the two lists have diferent legth");
			}
		}

		return resultText;
	}
}