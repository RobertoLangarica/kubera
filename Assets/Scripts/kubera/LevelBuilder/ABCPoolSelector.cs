using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ABC;

namespace LevelBuilder
{
	public class ABCPoolSelector : MonoBehaviour 
	{
		public GridSelector selector;
		public GameObject LetterPrefab;
		public string title;

		private List<AlfabetUnit> alfabet;
		private List<GridDataItem> items;

		void Start () 
		{
			selector.setTitle(title);
		}

		public void setAlfabet(List<AlfabetUnit> newAlfabet)
		{
			alfabet = newAlfabet;

			createDataItems();
			setSelectorData();
		}

		private void createDataItems()
		{
			items = new List<GridDataItem>();

			foreach(AlfabetUnit a in alfabet)
			{
				items.Add(new GridDataItem());
				items[items.Count-1].data = a;
				items[items.Count-1].objectToShow = Instantiate(LetterPrefab) as GameObject;
				items[items.Count-1].objectToShow.GetComponent<Text>().text = a.cvalue.ToString();
			}
		}

		private void setSelectorData()
		{
			selector.setData(items);
		}

		public void getData()
		{
			
		}
	}
}