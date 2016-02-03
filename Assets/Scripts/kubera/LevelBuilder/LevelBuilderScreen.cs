using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LevelBuilder
{
	public class LevelBuilderScreen : MonoBehaviour {


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
		public Toggle[] powerupToggles;

		public GameObject saveAsPopUp;
		private InputField saveAsInput;
		private Text saveAsWarning;
		private Button saveAsAccept;

		//Nombre para el nivel que se esta modificando (este nivel puede no existir en el xml de niveles)
		private string currentEditingLevelName;

		public void Start()
		{
			saveAsPopUp.SetActive(true);//Activo para poderle extrar hijos
			saveAsInput = saveAsPopUp.GetComponentInChildren<InputField>();
			saveAsWarning = saveAsPopUp.GetComponentsInChildren<Text>(true)[1];
			saveAsWarning.gameObject.SetActive(false);
			saveAsAccept = saveAsPopUp.GetComponentsInChildren<Button>(true)[1];
			saveAsAccept.interactable = false;
			saveAsPopUp.SetActive(false);


			//HUD por default
			resetEditorToDefaultState();
		}

		/**
		 * Pone el editor en estado default para comenzar a editar un nuevo nivel
		 **/ 
		private void resetEditorToDefaultState()
		{
			//opciones de niveles en el dropdown de cargar
			updateLevelSelectorOptions();
			//Nombre del siguiente nivel (el inmediato siguiente)
			setcurrentEditingNameToTheLast();
			//Mostrando el nombre correcto
			updateShowedName();

			setAlfabetToABCSelector();

			//Valores por default
			inputMovements.text = "10";
			inputStar1.text = "10";
			inputStar2.text = "20";
			inputStar3.text = "30";
			hideABCSelector();

			foreach(Toggle toggle in powerupToggles)
			{
				toggle.isOn = false;
			}

			//Actualizamos el lenguaje al que este configurado 
			for(int i = 0; i < languageSelector.options.Count; i++)
			{
				if(languageSelector.options[i].text == UserDataManager.instance.language)
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
		}

		/*+
		 * Actualiza los nombres mostrados en la HUD
		 **/ 
		private void updateShowedName()
		{
			lblName.text = currentEditingLevelName;
			lblTitle.text = currentEditingLevelName;
		}

		private void setAlfabetToABCSelector()
		{
			abcSelector.setAlfabet(PersistentData.instance.abcStructure.getAlfabet());
		}

		public void onStarValueChange()
		{
			lblScore.text = (int.Parse(inputStar1.text) + int.Parse(inputStar2.text) + int.Parse(inputStar3.text)).ToString();
		}

		/**
		 * Carga el nivel seleccionado para su configuracion
		 **/
		public void OnLeveleSelectedToLoad()
		{
			if(lvlSelector.value != 0)
			{
				loadHUDFromLevel(lvlSelector.options[lvlSelector.value].text);
			}
		}

		/**
		 * Cambia al lenguaje seleccionado
		 **/ 
		public void OnLanguageSelected()
		{
			//Que se carguen los niveles adecuados
			PersistentData.instance.configureGameForLanguage(languageSelector.options[languageSelector.value].text);

			//Las nuevas opciones de niveles con ese lenguaje
			updateLevelSelectorOptions();

			//Estado default del editor
			setcurrentEditingNameToTheLast();
			updateShowedName();
		}
			
		/**
		 * Guardamos el currentLevel dentro del XML
		 **/ 
		public void save()
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
			lvlToSave.lettersPool = "";
			lvlToSave.obstacleLettersPool = "";
			lvlToSave.pieces = "";
			lvlToSave.grid = "";
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

		/**
		 * Lee los datos de un nivel y configura la HUD para que los muestre correctamente
		 **/ 
		private void loadHUDFromLevel(string levelName)
		{
			//Actualizamos el nombre
			currentEditingLevelName = levelName;
			updateShowedName();

			Level level = PersistentData.instance.levelsData.getLevelByName(levelName);


			//level.lettersPool = "";
			//level.obstacleLettersPool = "";
			//level.pieces = "";
			//level.grid = "";
			inputMovements.text = level.moves.ToString();
			inputStar1.text = level.scoreToStar1.ToString();
			inputStar2.text = level.scoreToStar2.ToString();
			inputStar3.text = level.scoreToStar3.ToString();
		}

		/**
		 * Mostramos el popUp de guardar como para seleccionar nuevo nombre
		 **/ 
		public void saveAs()
		{
			//Estado default del saveas popup
			saveAsInput.text = "";
			saveAsAccept.interactable = false;
			saveAsWarning.gameObject.SetActive(false);

			saveAsPopUp.SetActive(true);
		}

		/**
		 * Reaccion al cambio de nombre en el popup saveAs
		 **/ 
		public void onSaveAsNameChange()
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

		/**
		 * Se cancela el guardar como
		 **/ 
		public void OnSaveAsCancel()
		{
			saveAsPopUp.SetActive(false);
		}

		/**
		 * Se acepta y se aguarda el nivel con el nombre que se indica
		 **/ 
		public void OnSaveAsAccept()
		{
			saveAsPopUp.SetActive(false);

			currentEditingLevelName = int.Parse(saveAsInput.text).ToString("0000");
			updateShowedName();

			save();
		}

		public void showABCSelector()
		{
			abcSelector.gameObject.SetActive(true);
		}

		public void hideABCSelector()
		{
			abcSelector.gameObject.SetActive(false);
		}
	}
}