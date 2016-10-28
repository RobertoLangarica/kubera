using UnityEngine;
using System.Collections;

/*
 * Clase que controla los textos del juego, para siempre seleccionar los del idioma adecuado
 */
public class MultiLanguageTextManager
{
	//HOME
	public const string START_GAME						= "startGame";
	public const string CONNECT_SHOPIKA_POPUP			= "connectShopikaPopUp";
	public const string INVITE_FRIENDS_SHOPIKA_POPUP	= "inviteFriendsShopikaPopUp";

	public const string CONNECTING_SHOPIKA_TITLE 			= "connecting_shopika_title";
	public const string CONNECTING_SHOPIKA_INFO 	   		= "connecting_shopika_info";
	public const string CONNECTING_FAILURE_SHOPIKA_TITLE   	= "connecting_failure_shopika_title";
	public const string CONNECTING_FAILURE_SHOPIKA_INFO		= "connecting_failure_shopika_info";
	public const string CONNECTING_FACEBOOK_TITLE 	    	= "connecting_facebook_title";
	public const string CONNECTING_FACEBOOK_INFO 	    	= "connecting_facebook_info";
	public const string CONNECTING_FAILURE_FACEBOOK_TITLE 	= "connecting_failure_facebook_title";
	public const string CONNECTING_FAILURE_FACEBOOK_INFO 	= "connecting_failure_facebook_info";


	//GAME
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
	public const string SCORE_HUD_TITLE_ID 					= "hudScore";
	public const string LVL_HUD_TITLE_ID 					= "hudLevel";
	public const string SECONDCHANCE_GIVEUP 				= "secondChanceGiveUp";
	public const string SECONDCHANCE_RETRY					= "secondChanceRetry";
	public const string SECONDCHANCE_TITLE 					= "secondChanceTitle";
	public const string GAME_NO_OPTION_CONTINUE 			= "noOptionsGameContinue";
	public const string GAME_NO_OPTION_GIVEUP				= "noOptionsGameGiveUp";
	public const string GAME_NO_OPTION_TITLE				= "noOptionGameTitle";

	public const string START_GAME_OBJECTIVE_POINTS_TEXT	= "startGameObjectivePointsText";
	public const string START_GAME_OBJECTIVE_LETTERS_TEXT	= "startGameObjectiveLettersText";
	public const string START_GAME_OBJECTIVE_BLACK_TEXT		= "startGameObjectiveBlackText";
	public const string START_GAME_OBJECTIVE_WORD_TEXT		= "startGameObjectiveWordText";
	public const string START_GAME_OBJECTIVE_WORDS_TEXT		= "startGameObjectiveWordsText";
	public const string START_GAME_OBJECTIVE_SYNONYM_TEXT	= "startGameObjectiveSynonymText";
	public const string START_GAME_OBJECTIVE_ANTONYM_TEXT	= "startGameObjectiveAntonymText";

	//MAP
	public const string OBJECTIVES_NAME_TEXT_ID 			= "levlTextName";

	public const string OBJECTIVE_POPUP_LVL_TEXT_ID 		= "lvlTextObjectivePopUp";
	public const string OBJECTIVE_POPUP_GOAL_TEXT_ID 		= "goalTextObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_POINTS_ID_A 		= "goalByPointsObjectivePopUpA";
	public const string OBJECTIVE_POPUP_BY_POINTS_ID_B 		= "goalByPointsObjectivePopUpB";
	public const string OBJECTIVE_POPUP_BY_WORDS_ID_A 		= "goalByWordsObjectivePopUpA";
	public const string OBJECTIVE_POPUP_BY_WORDS_ID_B 		= "goalByWordsObjectivePopUpB";
	public const string OBJECTIVE_POPUP_BY_LETTERS_ID 		= "goalByLettersObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_OBSTACLES_ID_A 	= "goalByObstaclesObjectivePopUpA";
	public const string OBJECTIVE_POPUP_BY_OBSTACLES_ID_B 	= "goalByObstaclesObjectivePopUpB";
	public const string OBJECTIVE_POPUP_BY_1_WORD_ID 		= "goalBy1WordObjectivePopUp";
	public const string OBJECTIVE_POPUP_BY_SYNONYMOUS_ID_A 	= "goalBySynonymousObjectivePopUpA";
	public const string OBJECTIVE_POPUP_BY_SYNONYMOUS_ID_B 	= "goalBySynonymousObjectivePopUpB";
	public const string OBJECTIVE_POPUP_BY_ANTONYM_ID_A 	= "goalByAntonymObjectivePopUpA";
	public const string OBJECTIVE_POPUP_BY_ANTONYM_ID_B		= "goalByAntonymObjectivePopUpB";
	public const string OBJECTIVE_POPUP_PLAY		 		= "goalPlay";
	public const string OBJECTIVE_POPUP_FACEBOOK	 		= "goalFacebook";

	public const string AFTERGAME_POPUP_POINTS		 		= "afterGamePoints";
	public const string AFTERGAME_POPUP_NEXT		 		= "afterGameNext";
	public const string AFTERGAME_POPUP_RETRY		 		= "afterGameRetry";
	public const string AFTERGAME_POPUP_FACEBOOK	 		= "afterGameFacebook";

	public const string LOOSEGAME_POPUP_TEXT		 		= "looseGameText";
	public const string LOOSEGAME_POPUP_NEXT		 		= "looseGameNext";
	public const string LOOSEGAME_POPUP_FACEBOOK	 		= "looseGameFacebook";

	public const string NO_GEMS_POPUP_TITLE					= "NoGemsPopUpTitle";
	public const string NO_GEMS_POPUP_INFO					= "NoGemsPopUpInfo";
	public const string NO_GEMS_POPUP_INFO_NO_LOGIN			= "NoGemsPopUpInfoNoShopikaLogin";
	public const string NO_GEMS_POPUP_BUTTON				= "NoGemsPopUpButton";

	public const string BOSS_LOCKED_UNLOCK_TEXT 			= "BossLockedUnlockText";
	public const string BOSS_LOCKED_OPTION_TEXT 			= "BossLockedOptionText";
	public const string BOSS_LOCKED_KEY_TEXT 				= "BossLockedKeyText";
	public const string BOSS_LOCKED_GEM_TEXT 				= "BossLockedGemText";
	public const string BOSS_LOCKED_STAR_TEXT 				= "BossLockedStarText";

	public const string FULL_LIFES_POPUP_TITLE				= "fullLifesTitle";
	public const string FULL_LIFES_POPUP_INFO				= "fullLifesInfo";
	public const string FULL_LIFES_POPUP_BUTTON				= "fullLifesButton";

	public const string MISSING_LIFES_POPUP_TITLE			= "missingLifesTitle";
	public const string MISSING_LIFES_POPUP_INFO1			= "missingLifesInfo1";
	public const string MISSING_LIFES_POPUP_INFO2			= "missingLifesInfo2";
	public const string MISSING_LIFES_POPUP_BUTTON			= "missingLifesButton";

	public const string NO_LIFES_POPUP_TITLE				= "noLifesTitle";
	public const string NO_LIFES_POPUP_INFO1				= "noLifesInfo1";
	public const string NO_LIFES_POPUP_INFO2				= "noLifesInfo2";
	public const string NO_LIFES_POPUP_BUTTON1				= "noLifesButton1";
	public const string NO_LIFES_POPUP_BUTTON2				= "noLifesButton2";

	public const string FULL_LIFES_TEXT						= "fullLifesText";

	public const string FB_CONNECT_POPUP_TITLE				= "fbConectTitle";
	public const string FB_CONNECT_POPUP_INFO1				= "fbConectInfo1";
	public const string FB_CONNECT_POPUP_BUTTON				= "fbConectButton";

	public const string FB_LOG_IN_TEXT						= "fbLogInText";
	public const string FB_LOG_OUT_TEXT						= "fbLogOutText";

	public const string FB_REQUEST_ASK_TEXT					= "fbRequestAskText";

	public const string FB_SUCCESS_TEXT 					= "fbSuccessText";
	public const string FB_SUCCESS_LIFES_TEXT 				= "fbSuccessLifesText";
	public const string FB_SUCCESS_KEYS_TEXT 				= "fbSuccessKeysText";

	//GAME
	public const string FLAVOR_TEXT							= "ticketFlavorText";
	public const string EXIT_POPUP_ID 						= "exitText";
	public const string WIN_TEXT_POPUP_ID					= "WinTextPopUpID";
	public const string STARTGAME_TEXT_POPUP_ID				= "StartTextPopUpID";
	public const string NO_MOVEMENTS_POPUP_ID				= "noMovementsPopUpID";
	public const string NO_PIECES_POPUP_ID					= "noPiecesPopUpID";
	public const string NO_OPTIONS_FIRST					= "noOptionsFirst";
	public const string NO_OPTIONS_GIVEUP					= "noOptionsGiveUp";
	public const string NO_OPTIONS_USEPOWER					= "noOptionsUsePower";
	public const string FREE_POWERUP_PRICE 					= "powerUpFreePrice";

	public const string TUTORIAL_LV1_PHASE1					= "Tutorial_lvl1_phase1";

	public const string TUTORIAL_LV2_PHASE1					= "Tutorial_lvl2_phase1";
	public const string TUTORIAL_LV2_PHASE2					= "Tutorial_lvl2_phase2";
	public const string TUTORIAL_LV2_PHASE3					= "Tutorial_lvl2_phase3";
	public const string TUTORIAL_LV2_PHASE4					= "Tutorial_lvl2_phase4";

	public const string TUTORIAL_LV3_PHASE1					= "Tutorial_lvl3_phase1";
	public const string TUTORIAL_LV3_PHASE2					= "Tutorial_lvl3_phase2";
	public const string TUTORIAL_LV3_PHASE3					= "Tutorial_lvl3_phase3";
	public const string TUTORIAL_LV3_PHASE4					= "Tutorial_lvl3_phase4";

	public const string TUTORIAL_LV4_PHASE1					= "Tutorial_lvl4_phase1";
	public const string TUTORIAL_LV4_PHASE2					= "Tutorial_lvl4_phase2";
	public const string TUTORIAL_LV4_PHASE3					= "Tutorial_lvl4_phase3";
	public const string TUTORIAL_LV4_PHASE4					= "Tutorial_lvl4_phase4";

	public const string TUTORIAL_LV5_PHASE1					= "Tutorial_lvl5_phase1";
	public const string TUTORIAL_LV5_PHASE2					= "Tutorial_lvl5_phase2";
	public const string TUTORIAL_LV5_PHASE3					= "Tutorial_lvl5_phase3";
	public const string TUTORIAL_LV5_WORD					= "Tutorial_lvl5_word";

	public const string TUTORIAL_LV6_PHASE1					= "Tutorial_lvl6_phase1";
	public const string TUTORIAL_LV6_PHASE2					= "Tutorial_lvl6_phase2";

	public const string TUTORIAL_LV7_PHASE1					= "Tutorial_lvl7_phase1";
	public const string TUTORIAL_LV7_PHASE2					= "Tutorial_lvl7_phase2";
	public const string TUTORIAL_LV7_PHASE3					= "Tutorial_lvl7_phase3";
	public const string TUTORIAL_LV7_PHASE4					= "Tutorial_lvl7_phase4";
	public const string TUTORIAL_LV7_WORD					= "Tutorial_lvl7_word";

	public const string TUTORIAL_LV8_PHASE1					= "Tutorial_lvl8_phase1";
	public const string TUTORIAL_LV8_PHASE2					= "Tutorial_lvl8_phase2";
	public const string TUTORIAL_LV8_PHASE3					= "Tutorial_lvl8_phase3";

	public const string TUTORIAL_LV13_PHASE1				= "Tutorial_lvl13_phase1";
	public const string TUTORIAL_LV13_PHASE2				= "Tutorial_lvl13_phase2";
	public const string TUTORIAL_LV13_PHASE3				= "Tutorial_lvl13_phase3";

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


	//Invitation to review
	public const string INVITATION_TITLE_TEXT1 				="Invitation_title_text1";
	public const string INVITATION_11_TEXT1 				="Invitation_11_text1";

	public const string INVITATION_21_TEXT1 				="Invitation_21_text1";
	public const string INVITATION_21_ANSWER1 				="Invitation_21_Answer1";
	public const string INVITATION_21_OPTION1 				="Invitation_21_option1";
	public const string INVITATION_21_OPTION2 				="Invitation_21_option2";

	public const string INVITATION_28_TEXT1 				="Invitation_28_text1";
	public const string INVITATION_28_ANSWER1 				="Invitation_28_Answer1";
	public const string INVITATION_28_ANSWER2 				="Invitation_28_Answer2";
	public const string INVITATION_28_OPTION1 				="Invitation_28_option1";
	public const string INVITATION_28_OPTION2 				="Invitation_28_option2";

	public const string INVITATION_32_TEXT1 				="Invitation_32_text1";
	public const string INVITATION_32_OPTION1 				="Invitation_32_option1";

	public const string INVITATION_40_TEXT1 				="Invitation_40_text1";
	public const string INVITATION_40_OPTION1 				="Invitation_40_option1";

	public const string INVITATION_44_TEXT1 				="Invitation_44_text1";
	public const string INVITATION_44_TEXT2 				="Invitation_44_text2";
	public const string INVITATION_44_OPTION1 				="Invitation_44_option1";
	public const string INVITATION_44_OPTION2 				="Invitation_44_option2";

	//WorldsName
	public const string WORLD1_NAME 						="World1_name";
	public const string WORLD2_NAME 						="World2_name";
	public const string WORLD3_NAME 						="World3_name";
	public const string WORLD4_NAME 						="World4_name";
	public const string WORLD5_NAME 						="World5_name";
	public const string WORLD6_NAME 						="World6_name";
	public const string WORLD7_NAME 						="World7_name";
	public const string WORLD8_NAME 						="World8_name";
	public const string WORLD9_NAME 						="World9_name";

	//Notifications
	public const string NOTIFICATION_LIFE1					="Life_1_Notification";
	public const string NOTIFICATION_FULL_LIFES				="FullLifes_Notification";
	public const string NOTIFICATION_3_DAYS 				= "3_Days_Without_Playing";
	public const string NOTIFICATION_7_DAYS 				= "7_Days_Without_Playing";
	public const string NOTIFICATION_14_DAYS 				= "14_Days_Without_Playing";
	public const string NOTIFICATION_30_DAYS 				= "30_Days_Without_Playing";

	//Share
	public const string SHARE_DEFAULT_TEXT					="shareDefaultText";

	//VisitShopika
	public const string VISIT_SHOPIKA_TITLE					="visitShopikaTitle";
	public const string VISIT_SHOPIKA_INFO					="visitShopikaDescription";
	public const string VISIT_SHOPIKA_BUTTON				="visitShopikaButton";

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