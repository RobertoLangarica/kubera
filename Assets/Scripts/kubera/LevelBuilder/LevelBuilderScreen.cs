﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LevelBuilder
{
	public class LevelBuilderScreen : MonoBehaviour {

		public const int BOMB_POWERUP		= 0;
		public const int BLOCK_POWERUP		= 1;
		public const int ROTATE_POWERUP 	= 2;
		public const int DESTROY_POWERUP 	= 3;
		public const int WILDCARD_POWERUP 	= 4;

		public const string ABC_NORMAL_TYPE	 = "1";
		public const string ABC_OBSTACLE_TYPE= "0";

		public InputField inputStar1;
		public InputField inputStar2;
		public InputField inputStar3;
		public InputField inputMovements;
		public Text lblScore;
		public Text lblTitle;
		public Text lblName;
		public Dropdown languageSelector;
		public Dropdown lvlSelector;
		public ABCPoolSelector abcSelector;
		public ABCPoolSelector abcObstacleSelector;
		public ABCPoolSelector abcGoalSelector;
		public LevelGoalSelector levelGoalSelector;
		public PiecesSelector piecesSelector;
		public TileGridEditor gridEditor;
		public Toggle[] powerupToggles;
		public LoadingIndicator loadingIndicator;

		public GameObject saveAsPopUp;
		private InputField saveAsInput;
		private Text saveAsWarning;
		private Button saveAsAccept;

		private string abcDataBeforeOpen;
		private string piecesDataBeforeOpen;
		private string gridDataBeforeOpen;
	
		//Nombre para el nivel que se esta modificando (este nivel puede no existir en el xml de niveles)
		private string currentEditingLevelName;

		public void Start()
		{
			//Esperamos 1 frame para evitar los problemas de sincronia entre 
			//quien manda llamar primero su Start o su Awake
			StartCoroutine("Initialization");

			//Mientras se procesa el diccionario por primera vez
			showLoadingIndicator();
			PersistentData.instance.onDictionaryFinished += hideLoadingIndicatorandRemoveCallback;
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
			hideLoadingIndicator();

			levelGoalSelector.isValidWord += wordExistInDictionary;
			levelGoalSelector.isPreviouslyUsedWord += wordExistInPreviousLevels;
			levelGoalSelector.onAddWordToDictionary += addWordToDictionary;

			piecesSelector.Initialize();
			gridEditor.Inititalize();

			//HUD por default
			resetEditorToDefaultState(UserDataManager.instance.language);

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

			piecesSelector.resetDataItems();
			piecesSelector.updateShowedData();
			levelGoalSelector.resetToDefault();

			gridEditor.resetDataItems();

			//Valores por default
			inputMovements.text = "10";
			inputStar1.text = "10";
			inputStar2.text = "20";
			inputStar3.text = "30";

			foreach(Toggle toggle in powerupToggles)
			{
				toggle.isOn = false;
			}

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

		/**
		 * Actualiza las opciones del selector de niveles
		 **/ 
		private void updateLevelSelectorOptions()
		{
			List<string> names = PersistentData.instance.levelsData.getAllLevelsNames();

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
			abcSelector.setAlfabet(PersistentData.instance.abcStructure.getAlfabet());
			abcGoalSelector.setAlfabet(PersistentData.instance.abcStructure.getAlfabet());
			abcObstacleSelector.setAlfabet(PersistentData.instance.abcStructure.getAlfabet());
		}

		public void writeLevelToXML()
		{
			Level lvlToSave;
			bool add = false;

			//Creamos un nuevo nivel o guardamos uno que ya existe
			if(PersistentData.instance.levelsData.existLevel(currentEditingLevelName))
			{
				//Ya existe el nivel asi que lo modificamos
				lvlToSave = PersistentData.instance.levelsData.getLevelByName(currentEditingLevelName);
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
			lvlToSave.pieces = piecesSelector.getCSVData();
			lvlToSave.grid = gridEditor.getCSVData();
			lvlToSave.winCondition = levelGoalSelector.getStringData();
			lvlToSave.unblockBomb = powerupToggles[BOMB_POWERUP].isOn;
			lvlToSave.unblockBlock = powerupToggles[BLOCK_POWERUP].isOn;
			lvlToSave.unblockRotate = powerupToggles[ROTATE_POWERUP].isOn;
			lvlToSave.unblockDestroy = powerupToggles[DESTROY_POWERUP].isOn;
			lvlToSave.unblockWildcard = powerupToggles[WILDCARD_POWERUP].isOn;
			lvlToSave.moves = int.Parse(inputMovements.text);
			lvlToSave.scoreToStar1 = int.Parse(inputStar1.text);
			lvlToSave.scoreToStar2 = int.Parse(inputStar2.text);
			lvlToSave.scoreToStar3 = int.Parse(inputStar3.text);

			if(add)
			{
				PersistentData.instance.levelsData.addLevel(lvlToSave);
			}

			//Aguardamos el archivo
			PersistentData.instance.levelsData.Save(Application.dataPath+"/Resources/levels_"+languageSelector.options[languageSelector.value].text+".xml");

			//Completamos la lista de niveles
			updateLevelSelectorOptions();
		}

		private void configureHUDFromLevel(string levelName)
		{
			//Actualizamos el nombre
			currentEditingLevelName = levelName;
			updateShowedName();

			Level level = PersistentData.instance.levelsData.getLevelByName(levelName);

			abcSelector.sincronizeDataWithCSV(level.lettersPool);
			abcObstacleSelector.sincronizeDataWithCSV(level.obstacleLettersPool);
			piecesSelector.sincronizeDataWithCSV(level.pieces);
			gridEditor.sincronizeDataWithCSV(level.grid);
			levelGoalSelector.sincronizeDataWithString(level.winCondition);
			powerupToggles[BOMB_POWERUP].isOn = level.unblockBomb;
			powerupToggles[BLOCK_POWERUP].isOn = level.unblockBlock;
			powerupToggles[ROTATE_POWERUP].isOn = level.unblockRotate;
			powerupToggles[DESTROY_POWERUP].isOn = level.unblockDestroy;
			powerupToggles[WILDCARD_POWERUP].isOn = level.unblockWildcard;
			inputMovements.text = level.moves.ToString();
			inputStar1.text = level.scoreToStar1.ToString();
			inputStar2.text = level.scoreToStar2.ToString();
			inputStar3.text = level.scoreToStar3.ToString();
		}
			
		public void OnLanguageSelected()
		{

			//Procesamos niveles y diccionario en el lenguaje indicado
			showLoadingIndicator();
			PersistentData.instance.onDictionaryFinished += onDiccionaryFinished;
			PersistentData.instance.configureGameForLanguage(languageSelector.options[languageSelector.value].text);
		}

		private void onDiccionaryFinished()
		{
			PersistentData.instance.onDictionaryFinished -= onDiccionaryFinished;
			resetEditorToDefaultState(languageSelector.options[languageSelector.value].text);
			Invoke("hideLoadingIndicator",0.5f);
		}

		public void OnLeveleSelectedToLoad()
		{
			if(lvlSelector.value != 0)
			{
				configureHUDFromLevel(lvlSelector.options[lvlSelector.value].text);
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
				saveAsWarning.gameObject.SetActive(PersistentData.instance.levelsData.existLevel(n));
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

		public void OnAcceptABCObstacleSelector()
		{
			//Se quedan los datos como estan
			abcObstacleSelector.gameObject.SetActive(false);
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
			word = word.ToLowerInvariant();
			word = word.Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u');
			return PersistentData.instance.abcStructure.isValidWord(word);
		}

		public bool wordExistInPreviousLevels(string word)
		{
			word = word.ToLowerInvariant();
			word = word.Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u');

			foreach(Level lvl in PersistentData.instance.levelsData.levels)
			{
				if(lvl.winCondition != null && lvl.winCondition.Length != 0)
				{
					string[] goal = lvl.winCondition.Split('-');

					if(goal[0] == "word")
					{
						string[] words = goal[1].Split('_');

						if(words[0] == word || words[1] == word)
						{
							return true;	
						}
					}
				}	
			}

			return false;
		}

		public void addWordToDictionary(string word)
		{
			word = word.ToLowerInvariant();
			word = word.Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u');
			PersistentData.instance.addWordToDictionary(word,languageSelector.options[languageSelector.value].text);
			PersistentData.instance.abcStructure.registerNewWord(word);
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

		public void OnPlay(){}

		private void hideLoadingIndicator()
		{
			loadingIndicator.gameObject.SetActive(false);
		}

		private void hideLoadingIndicatorandRemoveCallback()
		{
			hideLoadingIndicator();
			PersistentData.instance.onDictionaryFinished += hideLoadingIndicatorandRemoveCallback;
		}

		private void showLoadingIndicator()
		{
			loadingIndicator.gameObject.SetActive(true);
		}
	}
}