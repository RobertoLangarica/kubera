using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GoalWordPopup : MonoBehaviour {

		public Text txtWarning;
		public InputField inputWord;
		public Button btnAdd;
		public Button btnAddWord;
		public Button btnDeleteWord;
		public Text	txtWords;
	
		protected List<string> words = new List<string>();

		public delegate void DNotification();
		public DNotification onCancel;
		public DNotification onAccept;
		public DNotification onAdd;
		public DNotification onChange;
		public DNotification onAddWord;
		public DNotification onDeleteWord;

		public void reset()
		{
			btnAdd.interactable = false;
			btnAddWord.interactable = false;
			btnDeleteWord.interactable = false;
			txtWarning.text = "";
			inputWord.text = "";
		}

		public void activateAdd(bool active)
		{
			btnAdd.interactable = active;
		}

		public void cancel()
		{
			onCancel();
		}

		public void accept()
		{
			onAccept();
		}

		public void add()
		{
			onAdd();
		}

		public void change()
		{
			onChange();
		}

		public void delete()
		{
			onDeleteWord ();
		}

		public void addWordToList()
		{
			onAddWord ();
		}

		public void deleteWordFromList()
		{
			words.RemoveAt (words.Count-1);
		}

		public void showWarning(string text)
		{
			txtWarning.text = text;
		}

		public void setInputValue(string text)
		{
			inputWord.text = text;
		}

		public string getInputValue()
		{
			return inputWord.text;
		}

		public void setWords(string[] text)
		{
			for(int i=0; i<text.Length; i++)
			{
				words.Add (text[i].Split('_')[0]);
			}
		}

		public void actualizeInputValues()
		{
			txtWords.text = "";

			for (int i = 0; i < words.Count; i++) 
			{
				txtWords.text += words [i];

				if (i + 1 < words.Count) 
				{
					txtWords.text += ",\n";
				}			
			}
		}

		public void setNewWord(string newWord)
		{
			words.Add(newWord);
		}

		public string[] getInputValues()
		{
			return words.ToArray();
		}

		public void activateAddWord(bool active)
		{
			btnAddWord.interactable = active;
		}

		public void activateDeleteWord(bool active)
		{
			btnDeleteWord.interactable = active;
		}
	}
}