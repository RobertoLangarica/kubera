using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GoalWordPopup : MonoBehaviour {

		public Text txtWarning;
		public InputField inputWord;
		public Button btnAdd;

		public delegate void DNotification();
		public DNotification onCancel;
		public DNotification onAccept;
		public DNotification onAdd;
		public DNotification onChange;

		void Start()
		{
			reset();
		}

		public void reset()
		{
			btnAdd.interactable = false;
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
	}
}