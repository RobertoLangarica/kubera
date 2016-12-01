﻿using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LevelBuilder
{
	public class LevelBuilderScreen : MonoBehaviour {

		public const int WORD_HINT_POWERUP		= 0;
		public const int BOMB_POWERUP		= 1;
		public const int BLOCK_POWERUP		= 2;
		public const int ROTATE_POWERUP 	= 3;
		public const int WILDCARD_POWERUP 	= 4;
		public const int DESTROY_POWERUP 	= 5;

		public const string ABC_NORMAL_TYPE	 = "1";
		public const string ABC_OBSTACLE_TYPE= "0";

		protected string defaultLetters   ="12_A_1_1,02_B_3_1,05_C_3_1,05_D_2_1,12_E_1_1,01_F_4_1,02_G_2_1,03_H_4_1,06_I_1_1,01_J_8_1,00_K_0_1,05_L_1_1,02_M_3_1,05_N_1_1,01_Ñ_8_1,09_O_1_1,02_P_3_1,01_Q_5_1,07_R_1_1,06_S_1_1,04_T_1_1,05_U_1_1,01_V_4_1,00_W_0_1,01_X_8_1,01_Y_4_1,01_Z_10_1" ;
		protected string defaultLetters2  ="12_A_1_1,00_B_3_1,05_C_3_1,05_D_2_1,12_E_1_1,00_F_4_1,00_G_2_1,00_H_4_1,06_I_1_1,00_J_8_1,00_K_0_1,05_L_1_1,00_M_3_1,05_N_1_1,00_Ñ_8_1,09_O_1_1,00_P_3_1,00_Q_5_1,07_R_1_1,06_S_1_1,04_T_1_1,05_U_1_1,00_V_4_1,00_W_0_1,00_X_8_1,00_Y_4_1,00_Z_10_1" ;
		protected string defaultObstacles ="0_A_1_0,02_B_3_0,00_C_3_0,00_D_2_0,00_E_1_0,01_F_4_0,02_G_2_0,03_H_4_0,00_I_1_0,01_J_8_0,00_K_0_0,00_L_1_0,02_M_3_0,00_N_1_0,01_Ñ_8_0,00_O_1_0,02_P_3_0,01_Q_5_0,00_R_1_0,00_S_1_0,00_T_1_0,00_U_1_0,01_V_4_0,00_W_0_0,01_X_8_0,01_Y_4_0,01_Z_10_1" ;

		protected string defaultLettersEnglish   ="09_A_1_1,02_B_3_1,02_C_3_1,04_D_2_1,12_E_1_1,02_F_4_1,03_G_2_1,02_H_4_1,09_I_1_1,01_J_8_1,01_K_5_1,04_L_1_1,02_M_3_1,06_N_1_1,00_Ñ_8_1,08_O_1_1,02_P_3_1,01_Q_5_1,05_R_1_1,04_S_1_1,06_T_1_1,04_U_1_1,02_V_4_1,02_W_4_1,01_X_8_1,02_Y_4_1,01_Z_10_1" ;


		public InputField inputStar1;
		public InputField inputStar2;
		public InputField inputStar3;
		public InputField inputMovements;
		public InputField tutorialInput;
		public InputField quickLvlAccessInput;
		public Text lblScore;
		public Text lblTitle;
		public Text lblName;
		public Dropdown languageSelector;
		public Dropdown lvlSelector;
		public ABCPoolSelector abcSelector;
		public ABCPoolSelector abcObstacleSelector;
		public ABCPoolSelector abcGoalSelector;
		public ABCPoolSelector abcTutorialSelector;
		public LevelGoalSelector levelGoalSelector;
		public PiecesSelector piecesSelector;
		public TileGridEditor gridEditor;
		public Toggle[] powerupToggles;
		public InputField[] inputPowerup;


		public LoadingIndicator loadingIndicator;

		public InputField inputWorld;
		public InputField inputFriendsNeeded;
		public InputField inputStarsNeeded;
		public InputField inputGemsNeeded;

		public GameObject saveAsPopUp;
		private InputField saveAsInput;
		private Text saveAsWarning;
		private Button saveAsAccept;

		public GameObject insertPopUp;
		private InputField insertInput;
		private Button insertAccept;

		public GameObject deletePopUp;
		private InputField deleteInput;
		private Button deleteAccept;

		private string abcDataBeforeOpen;
		private string piecesDataBeforeOpen;
		private string gridDataBeforeOpen;

		protected string tutorialSelection;
	
		//Nombre para el nivel que se esta modificando (este nivel puede no existir en el xml de niveles)
		private string currentEditingLevelName;

		public void Start()
		{
			//Esperamos 1 frame para evitar los problemas de sincronia entre 
			//quien manda llamar primero su Start o su Awake
			StartCoroutine("Initialization");

			//Mientras se procesa el diccionario por primera vez
			showLoadingIndicator();
			PersistentData.GetInstance().onDictionaryFinished += hideLoadingIndicatorandRemoveCallback;

			StartCoroutine (initializeAfterGame());
		}

		private IEnumerator Initialization()
		{
			yield return null;
			saveAsPopUp.SetActive(true);//Activo para poderle extrar hijos
			saveAsInput = saveAsPopUp.GetComponentInChildren<InputField>();
			saveAsWarning = saveAsPopUp.GetComponentsInChildren<Text>(true)[1];
			saveAsWarning.gameObject.SetActive(false);
			saveAsAccept = saveAsPopUp.GetComponentsInChildren<Button>(true)[1];
			saveAsAccept.interactable = false;
			saveAsPopUp.SetActive(false);

			insertPopUp.SetActive(true);//Activo para poderle extrar hijos
			insertInput = insertPopUp.GetComponentInChildren<InputField>();
			insertAccept = insertPopUp.GetComponentsInChildren<Button>(true)[1];
			insertAccept.interactable = false;
			insertPopUp.SetActive(false);

			deletePopUp.SetActive(true);//Activo para poderle extrar hijos
			deleteInput = deletePopUp.GetComponentInChildren<InputField>();
			deleteAccept = deletePopUp.GetComponentsInChildren<Button>(true)[1];
			deleteAccept.interactable = false;
			deletePopUp.SetActive(false);
			hideLoadingIndicator();

			levelGoalSelector.isValidWord += wordExistInDictionary;
			levelGoalSelector.isPreviouslyUsedWord += wordExistInPreviousLevels;
			levelGoalSelector.onAddWordToDictionary += addWordToDictionary;

			piecesSelector.Initialize();
			gridEditor.Inititalize();

			//HUD por default
			resetEditorToDefaultState(UserDataManager.instance.language);
			//resetEditorToDefaultState("english");
			OnLanguageSelected();

			//Nombre del siguiente nivel (el inmediato siguiente)
			setcurrentEditingNameToTheLast();
		}
			
		/**
		 * Pone el editor en estado default para comenzar a editar un nuevo nivel
		 **/ 
		private void resetEditorToDefaultState(string language)
		{
			//opciones de niveles en el dropdown de cargar
			updateLevelSelectorOptions();

			setAlfabetToABCSelectors();
			setABCSelectorsDefaultData();

			piecesSelector.resetDataItems();
			piecesSelector.updateShowedData();
			levelGoalSelector.resetToDefault();

			gridEditor.resetDataItems();

			//Valores por default
			inputMovements.text = "10";
			inputStar1.text = "10";
			inputStar2.text = "20";
			inputStar3.text = "30";

			inputWorld.text = "0";
			inputFriendsNeeded.text = "1";
			inputStarsNeeded.text = "20";
			inputGemsNeeded.text = "10";

			foreach(Toggle toggle in powerupToggles)
			{
				toggle.isOn = false;
			}

			foreach(InputField inputField in inputPowerup)
			{
				inputField.text = "";
			}
				
			if(languageSelector.options[languageSelector.value].text != language)
			{
				//Actualizamos el lenguaje al que este configurado 
				for(int i = 0; i < languageSelector.options.Count; i++)
				{
					if(languageSelector.options[i].text == language)
					{
						languageSelector.value = i;
						break;
					}
				}	
			}
		}

		/**
		 * Actualiza las opciones del selector de niveles
		 **/ 
		private void updateLevelSelectorOptions()
		{
			List<string> names = PersistentData.GetInstance().levelsData.getAllLevelsNames();
			names.Sort();

			List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

			options.Add(new Dropdown.OptionData("Cargar"));

			foreach(string name in names)
			{
				options.Add(new Dropdown.OptionData(name));
			}

			lvlSelector.options = options;
			lvlSelector.value = 0;//Seleccionamos el item 0 que es el default que dice Cargar
		}

		/**
		 * Pone el nombre del nivel en edicion al ultimo existente + 1
		 **/ 
		private void setcurrentEditingNameToTheLast()
		{
			if(lvlSelector.options.Count == 1)
			{
				currentEditingLevelName = "0001";
			}
			else
			{
				currentEditingLevelName = (int.Parse(lvlSelector.options[lvlSelector.options.Count-1].text)+1).ToString("0000");
			}

			//Mostrando el nombre correcto
			updateShowedName();
		}

		/*+
		 * Actualiza los nombres mostrados en la HUD
		 **/ 
		private void updateShowedName()
		{
			lblTitle.text = currentEditingLevelName;
		}

		private void setAlfabetToABCSelectors()
		{
			abcSelector.setAlfabet(PersistentData.GetInstance().abcDictionary.getAlfabet());
			abcGoalSelector.setAlfabet(PersistentData.GetInstance().abcDictionary.getAlfabet());
			abcObstacleSelector.setAlfabet(PersistentData.GetInstance().abcDictionary.getAlfabet());
			abcTutorialSelector.setAlfabet(PersistentData.GetInstance().abcDictionary.getAlfabet());
		}

		private void setABCSelectorsDefaultData()
		{
			abcSelector.createEmptyData();
			abcGoalSelector.createEmptyData();
			abcObstacleSelector.createEmptyData();
			abcTutorialSelector.createEmptyData();

			//Default data
			abcSelector.sincronizeDataWithCSV (defaultLetters);
			//abcObstacleSelector.sincronizeDataWithCSV(defaultObstacles);

			//levelGoalSelector.sincronizeDataWithString(level.winCondition);
		}

		public void writeLevelToXML()
		{
			Level lvlToSave;
			bool add = false;

			//Creamos un nuevo nivel o guardamos uno que ya existe
			if(PersistentData.GetInstance().levelsData.existLevel(currentEditingLevelName))
			{
				//Ya existe el nivel asi que lo modificamos
				lvlToSave = PersistentData.GetInstance().levelsData.getLevelByName(currentEditingLevelName);
			}
			else
			{
				//Creamos un nuevo nivel e indicamos que se tiene que agregar a ls lista
				add = true;

				lvlToSave = new Level();
			}

			lvlToSave.name = currentEditingLevelName;
			lvlToSave.difficulty = 0;
			lvlToSave.lettersPool = abcSelector.getCSVData(ABC_NORMAL_TYPE);
			lvlToSave.obstacleLettersPool = abcObstacleSelector.getCSVData(ABC_OBSTACLE_TYPE);
			lvlToSave.tutorialLettersPool = abcTutorialSelector.getCSVData(ABC_NORMAL_TYPE) + '-' + tutorialSelection;
			lvlToSave.pieces = piecesSelector.getCSVData();
			lvlToSave.grid = gridEditor.getCSVData();
			lvlToSave.goal = levelGoalSelector.getStringData();
			lvlToSave.unblockWordHint = powerupToggles[WORD_HINT_POWERUP].isOn;
			lvlToSave.unblockBomb = powerupToggles[BOMB_POWERUP].isOn;
			lvlToSave.unblockBlock = powerupToggles[BLOCK_POWERUP].isOn;
			lvlToSave.unblockRotate = powerupToggles[ROTATE_POWERUP].isOn;
			lvlToSave.unblockWildcard = powerupToggles[WILDCARD_POWERUP].isOn;
			lvlToSave.unblockDestroy = powerupToggles[DESTROY_POWERUP].isOn;

			lvlToSave.amountWordHint = int.Parse(inputPowerup [WORD_HINT_POWERUP].text);
			lvlToSave.amountBomb = int.Parse(inputPowerup [BOMB_POWERUP].text);
			lvlToSave.amountBlock = int.Parse(inputPowerup [BLOCK_POWERUP].text);
			lvlToSave.amountRotate = int.Parse(inputPowerup [ROTATE_POWERUP].text);
			lvlToSave.amountWildcard = int.Parse(inputPowerup [WILDCARD_POWERUP].text);
			lvlToSave.amountDestroy = int.Parse(inputPowerup [DESTROY_POWERUP].text);

			lvlToSave.moves = int.Parse(inputMovements.text);
			lvlToSave.scoreToStar1 = int.Parse(inputStar1.text);
			lvlToSave.scoreToStar2 = int.Parse(inputStar2.text);
			lvlToSave.scoreToStar3 = int.Parse(inputStar3.text);
			lvlToSave.world = int.Parse (inputWorld.text);
			lvlToSave.friendsNeeded = int.Parse(inputFriendsNeeded.text);
			lvlToSave.gemsNeeded = int.Parse(inputGemsNeeded.text);
			lvlToSave.starsNeeded = int.Parse(inputStarsNeeded.text);

			//Agregamos el conteo de obstaculos para esa meta
			if(levelGoalSelector.isObstacles())
			{
				lvlToSave.goal = levelGoalSelector.getStringData() + gridEditor.getDataCountOfType(8);
			}

			if(add)
			{
				PersistentData.GetInstance().levelsData.addLevel(lvlToSave);
			}
				
			saveLocalData();

			#if UNITY_EDITOR
			AssetDatabase.Refresh();
			#endif

			//Completamos la lista de niveles
			updateLevelSelectorOptions();
		}

		protected void saveLocalData()
		{
			processLevelsBeforeSave();

			//Guardamos el archivo
			PersistentData.GetInstance().levelsData.Save(Application.dataPath+"/Resources/levels_"+languageSelector.options[languageSelector.value].text+".xml");	
		}

		protected void processLevelsBeforeSave()
		{
			//Ordenamos de mayor a menor segun su nombre
			PersistentData.GetInstance ().levelsData.sortLevels();

			//Configuracion de los bosses
			bool bossValue = true;
			Level[] levels = PersistentData.GetInstance ().levelsData.levels;

			for(int i = 0; i < levels.Length; i++)
			{
				if(i+1 < levels.Length)
				{
					if(levels[i].world == levels[i+1].world)
					{
						//No es boss
						levels[i].isBoss = !bossValue;
						levels[i].friendsNeeded = 0;
						levels[i].starsNeeded = 0;
					}
					else
					{
						//Es boss
						levels[i].isBoss = bossValue;
						levels[i].friendsNeeded = 1;
						levels[i].starsNeeded = Mathf.RoundToInt((levels[i].index-1)*3*.8f);
					}
				}
				else
				{
					//Es boss
					levels[i].isBoss = bossValue;
					levels[i].friendsNeeded = 1;
					levels[i].starsNeeded = Mathf.RoundToInt((levels[i].index-1)*3*.8f);
				}
			}
		}

		public void InsertLevel(int index)
		{
			string levelName;
			Level level;

			//Renombramos los niveles (evitamos duplicados recorriendo hacia atras)
			for(int i = PersistentData.GetInstance().levelsData.levels.Length; i >= index ; i--)
			{
				levelName = i.ToString("0000");

				if(PersistentData.GetInstance().levelsData.existLevel(levelName))
				{
					level = PersistentData.GetInstance().levelsData.getLevelByName(levelName);
					level.name = (i+1).ToString("0000");
				}
			} 

			//Insertamos uno vacio
			resetEditorToDefaultState(languageSelector.options[languageSelector.value].text);	
			currentEditingLevelName = (index).ToString("0000");
			writeLevelToXML();

			//Mostrando el nombre correcto
			updateShowedName();
		}

		public void DeleteLevel(int index)
		{
			string levelName = index.ToString("0000");
			Level level;
			int length = PersistentData.GetInstance().levelsData.levels.Length;
			//Lo eliminamos
			if(PersistentData.GetInstance().levelsData.existLevel(levelName))
			{
				PersistentData.GetInstance().levelsData.removeLevel(PersistentData.GetInstance().levelsData.getLevelByName(levelName));
			}
				
			//Renombramos los niveles (evitamos duplicados recorriendo hacia atras)
			for(int i = index+1; i <= length; i++)
			{
				levelName = i.ToString("0000");

				if(PersistentData.GetInstance().levelsData.existLevel(levelName))
				{
					level = PersistentData.GetInstance().levelsData.getLevelByName(levelName);
					level.name = (i-1).ToString("0000");
				}
			} 

			saveLocalData();

			#if UNITY_EDITOR
			AssetDatabase.Refresh();
			#endif

			//Completamos la lista de niveles
			updateLevelSelectorOptions();

			//HUD por default
			resetEditorToDefaultState(UserDataManager.instance.language);

			//Nombre del siguiente nivel (el inmediato siguiente)
			setcurrentEditingNameToTheLast();
		}

		private void configureHUDFromLevel(string levelName)
		{
			//Actualizamos el nombre
			currentEditingLevelName = levelName;
			updateShowedName();

			Level level = PersistentData.GetInstance().levelsData.getLevelByName(levelName);

			abcSelector.sincronizeDataWithCSV(level.lettersPool);
			abcObstacleSelector.sincronizeDataWithCSV(level.obstacleLettersPool);
			levelGoalSelector.sincronizeDataWithString(level.goal);
			piecesSelector.sincronizeDataWithCSV(level.pieces);
			gridEditor.sincronizeDataWithCSV(level.grid);

			powerupToggles[WORD_HINT_POWERUP].isOn = level.unblockWordHint;
			powerupToggles[BOMB_POWERUP].isOn = level.unblockBomb;
			powerupToggles[BLOCK_POWERUP].isOn = level.unblockBlock;
			powerupToggles[ROTATE_POWERUP].isOn = level.unblockRotate;
			powerupToggles[DESTROY_POWERUP].isOn = level.unblockDestroy;
			powerupToggles[WILDCARD_POWERUP].isOn = level.unblockWildcard;

			inputPowerup [WORD_HINT_POWERUP].text = level.amountWordHint.ToString();
			inputPowerup [BOMB_POWERUP].text = level.amountBomb.ToString();
			inputPowerup [BLOCK_POWERUP].text = level.amountBlock.ToString();
			inputPowerup [ROTATE_POWERUP].text = level.amountRotate.ToString();
			inputPowerup [WILDCARD_POWERUP].text = level.amountWildcard.ToString();
			inputPowerup [DESTROY_POWERUP].text = level.amountDestroy.ToString();

			inputMovements.text = level.moves.ToString();
			inputStar1.text = level.scoreToStar1.ToString();
			inputStar2.text = level.scoreToStar2.ToString();
			inputStar3.text = level.scoreToStar3.ToString();
			inputWorld.text = level.world.ToString ();
			inputFriendsNeeded.text = level.friendsNeeded.ToString ();
			inputGemsNeeded.text = level.gemsNeeded.ToString ();
			inputStarsNeeded.text = level.starsNeeded.ToString ();


			if(level.tutorialLettersPool != null)
			{
				if(level.tutorialLettersPool.Length > 0)
				{
					abcTutorialSelector.sincronizeDataWithCSV(level.tutorialLettersPool.Split('-')[0]);
					tutorialInput.text = level.tutorialLettersPool.Split('-')[1];
				}
			}
		}
			
		public void OnLanguageSelected()
		{

			//Procesamos niveles y diccionario en el lenguaje indicado
			showLoadingIndicator();
			PersistentData.GetInstance().onDictionaryFinished += onDiccionaryFinished;
			PersistentData.GetInstance().configureGameForLanguage(languageSelector.options[languageSelector.value].text);
		}

		private void onDiccionaryFinished()
		{
			PersistentData.GetInstance().onDictionaryFinished -= onDiccionaryFinished;
			resetEditorToDefaultState(languageSelector.options[languageSelector.value].text);
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnLevelSelectedToLoad()
		{
			if(lvlSelector.value != 0)
			{
				configureHUDFromLevel(lvlSelector.options[lvlSelector.value].text);
				showLoadingIndicator();
				Invoke("hideLoadingIndicator",0.5f);
			}
		}

		public void OnLevelSelectedToLoadFromQuickAcces()
		{
			if(quickLvlAccessInput.text != "" && int.Parse(quickLvlAccessInput.text) != 0)
			{

				//Que el lvl selector tenga el valor adecuado
				string value = int.Parse(quickLvlAccessInput.text).ToString("0000");

				for(int i = 0; i < lvlSelector.options.Count; i++)
				{
					if(lvlSelector.options[i].text == value)
					{
						lvlSelector.value = i;
						break;
					}
				}


				//cargamos el nivel
				configureHUDFromLevel(int.Parse(quickLvlAccessInput.text).ToString("0000"));
				showLoadingIndicator();
				Invoke("hideLoadingIndicator",0.5f);
			}
		}
			
		public void onStarPointsChange()
		{
			lblScore.text = (int.Parse(inputStar1.text) + int.Parse(inputStar2.text) + int.Parse(inputStar3.text)).ToString();
		}
			
		public void OnShowSaveAsPopup()
		{
			//Estado default del saveas popup
			saveAsInput.text = "";
			saveAsAccept.interactable = false;
			saveAsWarning.gameObject.SetActive(false);

			saveAsPopUp.SetActive(true);
		}
			
		public void OnSaveAsPopupNameChange()
		{
			if(saveAsInput.text != "")
			{
				saveAsAccept.interactable = true;

				//Warning si existe el nivel 
				int n = int.Parse(saveAsInput.text);
				saveAsWarning.gameObject.SetActive(PersistentData.GetInstance().levelsData.existLevel(n));
			}
			else
			{
				saveAsAccept.interactable = false;
				saveAsWarning.gameObject.SetActive(false);
			}
		}
			
		public void OnSaveAsPopupCancel()
		{
			saveAsPopUp.SetActive(false);
		}
			
		public void OnSaveAsPopupAccept()
		{
			saveAsPopUp.SetActive(false);

			currentEditingLevelName = int.Parse(saveAsInput.text).ToString("0000");
			updateShowedName();

			writeLevelToXML();
			showLoadingIndicator();
			Invoke("hideLoadingIndicator",0.5f);

		}

		public void OnShowInsertPopup()
		{
			//Estado default
			insertInput.text = "";
			insertAccept.interactable = false;

			insertPopUp.SetActive(true);
		}

		public void OnInsertPopupNameChange()
		{
			if(insertInput.text != "")
			{
				insertAccept.interactable = true;
			}
			else
			{
				insertAccept.interactable = false;
			}
		}

		public void OnInsertPopupCancel()
		{
			insertPopUp.SetActive(false);
		}

		public void OnInsertPopupAccept()
		{
			insertPopUp.SetActive(false);
			showLoadingIndicator();
			InsertLevel(int.Parse(insertInput.text));
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnShowDeletePopup()
		{
			//Estado default
			deleteInput.text = "";
			deleteAccept.interactable = false;

			deletePopUp.SetActive(true);
		}

		public void OnDeletePopupNameChange()
		{
			if(deleteInput.text != "")
			{
				deleteAccept.interactable = true;
			}
			else
			{
				deleteAccept.interactable = false;
			}
		}

		public void OnDeletePopupCancel()
		{
			deletePopUp.SetActive(false);
		}

		public void OnDeletePopupAccept()
		{
			deletePopUp.SetActive(false);
			showLoadingIndicator();
			DeleteLevel(int.Parse(deleteInput.text));
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnSave()
		{
			writeLevelToXML();
			showLoadingIndicator();
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnShowABCSelector()
		{
			abcDataBeforeOpen = abcSelector.getCSVData(ABC_NORMAL_TYPE);
			abcSelector.gameObject.SetActive(true);
		}

		public void OnCancelABCSelector()
		{
			abcSelector.sincronizeDataWithCSV(abcDataBeforeOpen);
			abcSelector.updateShowedData();
			//Reset de datos a los del XML
			abcSelector.gameObject.SetActive(false);
		}

		public void OnResetABCSelector()
		{
			abcSelector.sincronizeDataWithCSV (defaultLetters);
		}

		public void OnResetABCEnglishSelector()
		{
			abcSelector.sincronizeDataWithCSV (defaultLettersEnglish);
		}

		public void OnResetABCSelector2()
		{
			abcSelector.sincronizeDataWithCSV (defaultLetters2);
		}

		public void OnEreaseABCSelector()
		{
			abcSelector.createEmptyData();
		}
			
		public void OnAcceptABCSelector()
		{
			//Se quedan los datos como estan
			abcSelector.gameObject.SetActive(false);
		}

		public void OnShowABCObstacleSelector()
		{
			abcDataBeforeOpen = abcObstacleSelector.getCSVData(ABC_OBSTACLE_TYPE);
			abcObstacleSelector.gameObject.SetActive(true);
		}

		public void OnCancelABCObstacleSelector()
		{
			abcObstacleSelector.sincronizeDataWithCSV(abcDataBeforeOpen);
			abcObstacleSelector.updateShowedData();
			abcObstacleSelector.gameObject.SetActive(false);
		}

		public void OnResetABCObstacleSelector()
		{
			abcObstacleSelector.sincronizeDataWithCSV(defaultObstacles);
		}

		public void OnEreaseABCObstacleSelector()
		{
			abcObstacleSelector.createEmptyData();
		}

		public void OnAcceptABCObstacleSelector()
		{
			//Se quedan los datos como estan
			abcObstacleSelector.gameObject.SetActive(false);
		}

		public void OnShowABCTutorialSelector()
		{
			abcDataBeforeOpen = abcTutorialSelector.getCSVData(ABC_NORMAL_TYPE);
			abcTutorialSelector.gameObject.SetActive(true);
		}

		public void OnCancelABCTutorialSelector()
		{
			abcTutorialSelector.sincronizeDataWithCSV(abcDataBeforeOpen);
			abcTutorialSelector.updateShowedData();
			abcTutorialSelector.gameObject.SetActive(false);
		}

		public void OnResetABCTutorialSelector()
		{
			abcTutorialSelector.sincronizeDataWithCSV(defaultLetters);
		}

		public void OnEreaseABCTutorialSelector()
		{
			abcTutorialSelector.createEmptyData();
		}

		public void OnAcceptABCTutorialSelector()
		{
			//Se quedan los datos como estan
			abcTutorialSelector.gameObject.SetActive(false);
		}

		public void onSelectionTextChanged(string val)
		{
			tutorialSelection = val;
		}

		public void OnShowPiecesSelector()
		{
			piecesDataBeforeOpen = piecesSelector.getCSVData();
			//Si no hay datos nos quedamos con los del XML
			piecesSelector.gameObject.SetActive(true);
		}

		public void OnCancelPiecesSelector()
		{
			//Reset de datos a los del XML
			piecesSelector.sincronizeDataWithCSV(piecesDataBeforeOpen);
			piecesSelector.updateShowedData();
			piecesSelector.gameObject.SetActive(false);
		}

		public void OnResetPieceSelector()
		{
			piecesSelector.sincronizeDataWithCSV("");
		}

		public void OnAcceptPiecesSelector()
		{
			//Se quedan los datos como estan
			piecesSelector.gameObject.SetActive(false);
		}

		public void OnShowGridEditor()
		{
			gridDataBeforeOpen = gridEditor.getCSVData();
			//Si no hay datos nos quedamos con los del XML
			gridEditor.gameObject.SetActive(true);
		}

		public void OnCancelGridEditor()
		{
			//Reset de datos a los del XML
			gridEditor.sincronizeDataWithCSV(gridDataBeforeOpen);
			gridEditor.gameObject.SetActive(false);
		}

		public void OnAcceptGridEditor()
		{
			//Se quedan los datos como estan
			gridEditor.gameObject.SetActive(false);
		}

		public bool wordExistInDictionary(string word)
		{
			word = word.ToUpperInvariant();
			word = word.Replace('Á','A').Replace('É','E').Replace('Í','I').Replace('Ó','Ó').Replace('Ú','U').Replace('Ü','U');
			return PersistentData.GetInstance().abcDictionary.isValidWord(word);
		}

		public bool wordExistInPreviousLevels(string word)
		{
			word = word.ToUpperInvariant();
			word = word.Replace('Á','A').Replace('É','E').Replace('Í','I').Replace('Ó','Ó').Replace('Ú','U').Replace('Ü','U');

			foreach(Level lvl in PersistentData.GetInstance().levelsData.levels)
			{
				if(lvl.goal != null && lvl.goal.Length != 0)
				{
					string[] goal = lvl.goal.Split('-');

					if(goal[0] == "word")
					{
						string[] words = goal[1].Split(',');

						for(int i= 0; i<words.Length; i++)
						{
							if(words[i].Split('_')[0] == word || words[i].Split('_')[1] == word)
							{
								return true;	
							}
						}
					}
				}	
			}

			return false;
		}

		public void addWordToDictionary(string word)
		{
			word = word.ToUpperInvariant();
			word = word.Replace('Á','A').Replace('É','E').Replace('Í','I').Replace('Ó','Ó').Replace('Ú','U').Replace('Ü','U');
			PersistentData.GetInstance().addWordToDictionary(word,languageSelector.options[languageSelector.value].text);
			PersistentData.GetInstance().abcDictionary.registerNewWord(word);
			PersistentData.GetInstance().serializeAndSaveDictionary(languageSelector.options[languageSelector.value].text);
		}

		public void OnReset()
		{
			resetEditorToDefaultState(languageSelector.options[languageSelector.value].text);
			showLoadingIndicator();
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnNew()
		{
			resetEditorToDefaultState(languageSelector.options[languageSelector.value].text);
			setcurrentEditingNameToTheLast();
			showLoadingIndicator();
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnPlay()
		{
			writeLevelToXML();
			PersistentData.GetInstance().setLevelNumber(int.Parse(currentEditingLevelName),true);
			ScreenManager.GetInstance().GoToSceneAsync("Game");
		}

		private void hideLoadingIndicator()
		{
			loadingIndicator.gameObject.SetActive(false);
		}

		private void hideLoadingIndicatorandRemoveCallback()
		{
			//hideLoadingIndicator();
			PersistentData.GetInstance().onDictionaryFinished += hideLoadingIndicatorandRemoveCallback;
		}

		IEnumerator initializeAfterGame()
		{
			yield return new WaitForSeconds (.5f);
			if(PersistentData.GetInstance().fromGameToEdit)
			{
				PersistentData.GetInstance().fromGameToEdit = false;

				configureHUDFromLevel (PersistentData.GetInstance().currentLevel.name);

			}
		}

		private void showLoadingIndicator()
		{
			loadingIndicator.gameObject.SetActive(true);
		}
	}
}