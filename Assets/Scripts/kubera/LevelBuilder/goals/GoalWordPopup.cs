using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GoalWordPopup : MonoBehaviour {

		public Text txtWarning;
		public InputField inputWord;
		public Button btnAdd;
		public Button btnAddToList;
		public Button btnDeleteFromList;
		public Text	wordsList;

		public delegate void DNotification();
		public DNotification onCancel;
		public DNotification onAccept;
		public DNotification onAdd;
		public DNotification onChange;
		public DNotification onAddWordToList;
		public DNotification onDeleteWordFromList;

		public void reset()
		{
			btnAdd.interactable = false;
			btnAddToList.interactable = false;
			btnDeleteFromList.interactable = false;
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

		public void addWordToList()
		{
			//onAddWordToList ();
			getInputValues ();
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

		public void setInputValues(string[] text)
		{
			for(int i=0; i<text.Length; i++)
			{
				wordsList.text += text[i].Split('_')[0];
				if(i+1 < text.Length)
				{
					wordsList.text += "\n";
				}
			}
		}

		public string[] getInputValues()
		{
			string[] words = new string[wordsList.text.Length];

			for(int i=0; i<wordsList.text.Length; i++)
			{
				if(wordsList.text[i] == '\n')
				{
					print (wordsList.text.Substring(i,1));
				}
			}
			return words;
		}

		public void activateAddWordToList(bool active)
		{
			btnAddToList.interactable = active;
		}
	}
}