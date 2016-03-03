﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class LevelGoalSelector : MonoBehaviour 
	{
		public const string ABC_NORMAL_TYPE	 = "1";

		public delegate bool DValidateString(string s);
		public delegate void DProcessString(string s);
		public DValidateString isValidWord;
		public DValidateString isPreviouslyUsedWord;
		public DProcessString onAddWordToDictionary;

		public Toggle togglePoints;
		public Toggle toggleLetters;
		public Toggle toggleWordsCount;
		public Toggle toggleWord;
		public Toggle toggleObstacles;

		public Selectable selectablePoints;
		public Selectable selectableLetters;
		public Selectable selectableWordsCount;
		public Selectable selectableWord;

		public InputField inputPoints;
		public InputField inputWords;

		public GoalWordPopup wordPopup;
		public ABCPoolSelector abcSelector;
		private string abcDataBeforeOpen;
		private string wordBeforeOpen;

		void Start()
		{
			togglePoints.onValueChanged.AddListener(onToggleValueChanged);
			toggleLetters.onValueChanged.AddListener(onToggleValueChanged);
			toggleWordsCount.onValueChanged.AddListener(onToggleValueChanged);
			toggleWord.onValueChanged.AddListener(onToggleValueChanged);
			toggleObstacles.onValueChanged.AddListener(onToggleValueChanged);

			wordPopup.onAccept += onAcceptWordPopUp;
			wordPopup.onCancel += onCancelWordPopUp;
			wordPopup.onAdd += AddWordToDictionary;
			wordPopup.onChange += onWordChange;
			wordPopup.onAddWord += onAddWordPopUp;
			wordPopup.onDeleteWord += onDeleteWordPopUp;

			showWordPopup(false);
		}

		private void onToggleValueChanged(bool value)
		{
			selectablePoints.interactable = togglePoints.isOn;
			selectableLetters.interactable = toggleLetters.isOn;
			selectableWordsCount.interactable = toggleWordsCount.isOn;
			selectableWord.interactable = toggleWord.isOn;
		}

		public void showWordPopup(bool show)
		{
			wordPopup.gameObject.SetActive(show);
		}

		public void onShowWordPopUp()
		{
			wordBeforeOpen = wordPopup.getInputValue();
			showWordPopup(true);
		}

		public void onCancelWordPopUp()
		{
			wordPopup.reset();
			wordPopup.setInputValue(wordBeforeOpen);
			showWordPopup(false);
		}

		public void onAcceptWordPopUp()
		{
			wordBeforeOpen = wordPopup.getInputValue();
			wordPopup.reset();
			wordPopup.setInputValue(wordBeforeOpen);
			showWordPopup(false);
		}

		public void onAddWordPopUp()
		{
			wordBeforeOpen = wordPopup.getInputValue();
			wordPopup.reset();
			wordPopup.setNewWord(wordBeforeOpen);
			wordPopup.actualizeInputValues ();
			wordPopup.activateDeleteWord (true);
		}

		public void onDeleteWordPopUp()
		{
			wordPopup.deleteWordFromList ();
			activateDeleteWord ();
			wordPopup.actualizeInputValues ();
		}

		public void activateDeleteWord()
		{
			wordPopup.activateDeleteWord (checkIfExistWords());
		}

		public void AddWordToDictionary()
		{
			onAddWordToDictionary(wordPopup.getInputValue());
			wordPopup.activateAdd(false);
			wordPopup.showWarning("Palabra válida");
		}

		public void onWordChange()
		{
			wordPopup.showWarning("");
			wordPopup.activateAdd(false);
			wordPopup.activateAddWord (false);

			if(wordPopup.getInputValue().Length < 3)
			{
				wordPopup.showWarning("Se requieren minimo 3 caracteres.");
			}
			else if(!isValidWord(wordPopup.getInputValue()))
			{
				wordPopup.showWarning("Palabra no válida para el diccionario.");
				wordPopup.activateAdd(true);
			}
			else if(isPreviouslyUsedWord(wordPopup.getInputValue()))
			{
				wordPopup.showWarning("Ya se utiliza en niveles creados anteriormente");
			}
			else
			{
				wordPopup.showWarning("Palabra válida");
				wordPopup.activateAddWord (true);
			}
		}

		public bool checkIfExistWords()
		{
			if(wordPopup.getInputValues().Length >0)
			{
				return true;
			}
			return false;
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

		public void resetToDefault()
		{
			togglePoints.isOn = true;
			toggleLetters.isOn = toggleWordsCount.isOn = toggleWord.isOn = toggleObstacles.isOn = false;

			selectablePoints.interactable		= true;
			selectableLetters.interactable		= false;
			selectableWordsCount.interactable	= false;
			selectableWord.interactable			= false;

			abcSelector.sincronizeDataWithCSV("");
			wordPopup.reset();
			inputPoints.text = "00";
			inputWords.text = "00";
		}

		public void sincronizeDataWithString(string data)
		{
			resetToDefault();
			if(data == null || data.Length == 0){return;}

			string[] goal = data.Split('-');

			if(goal[0] == "points")
			{
				inputPoints.text = goal[1];
				togglePoints.isOn = true;
			}
			else if(goal[0] == "letters")
			{
				abcSelector.sincronizeDataWithCSV(goal[1]);
				toggleLetters.isOn = true;
			}
			else if(goal[0] == "words")
			{
				inputWords.text = goal[1];
				toggleWordsCount.isOn = true;
			}
			else if(goal[0] == "obstacles")
			{
				toggleObstacles.isOn = true;
			}
			else if(goal[0] == "word")
			{
				//wordPopup.setInputValue(goal[1].Split('_')[0]);
				wordPopup.setWords (goal[1].Split(','));
				wordPopup.actualizeInputValues();
				activateDeleteWord ();
				toggleWord.isOn = true;
			}
		}
			
		public string getStringData()
		{
			string result = "";

			if(togglePoints.isOn)
			{
				result =  "points-"+inputPoints.text;
			}
			else if(toggleLetters.isOn)
			{
				result = "letters-"+abcSelector.getCSVData(ABC_NORMAL_TYPE);
			}
			else if(toggleWordsCount.isOn)
			{
				result = "words-"+inputWords.text;
			}
			else if(toggleObstacles.isOn)
			{
				result = "obstacles-";
			}
			else if(toggleWord.isOn)
			{
				//Primero se guarda con acentos luego sin acentos
				string[] words = wordPopup.getInputValues();
				string word;
				result = "word-";
				for(int i=0; i<words.Length; i++)
				{
					word = words[i];
					result += word+"_";
					word = words[i].Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u');
					result += word;
					if(i+1 < words.Length)
					{
						result += ",";
					}
				}
			}				
			return result;
		}
	}
}