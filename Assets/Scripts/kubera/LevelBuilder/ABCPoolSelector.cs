using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ABC;

namespace LevelBuilder
{
	public class ABCPoolSelector : MonoBehaviour 
	{
		public GridSelector selector;
		public GameObject LetterPrefab;
		public Text txtInfo;
		public string title;

		private List<AlfabetUnit> alfabet;
		public	List<GridDataItem> items;

		void Start () 
		{
			selector.setTitle(title);
		}

		public void setAlfabet(List<AlfabetUnit> newAlfabet)
		{
			alfabet = newAlfabet;

			selector.OnDataChange += onSelectorDataChange;
			createDataItems();
			setSelectorData();
			updateShowedData();
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

		private void onSelectorDataChange()
		{
			txtInfo.text = getInfoString();
		}

		public string getInfoString()
		{
			StringBuilder builder = new StringBuilder();
			int count = 0;
			int localCount;
			for(int i = 0; i < items.Count; i++)
			{
				localCount = items[i].quantity+items[i].x2Count + items[i].x3Count;
				count += localCount;
				
				if(localCount > 0)
				{
					if(builder.Length != 0)
					{
						builder.Append(", ");
					}

					builder.Append((items[i].data as AlfabetUnit).cvalue);
					builder.Append(":");
					builder.Append(localCount);
				}
			}

			builder.Insert(0,"T:"+count.ToString()+ (builder.Length == 0 ? "":", "));

			return builder.ToString();
		}

		public void sincronizeDataWithCSV(string csv)
		{
			if(csv.Length == 0)
			{
				resetDataItems();
				updateShowedData();
				return;
			}

			string[] letters = csv.Split(',');
			ABCStringHandler handler = new ABCStringHandler();
			GridDataItem item;
				
			resetDataItems();

			for(int i = 0; i < letters.Length; i++)
			{
				handler.setString(letters[i]);
				item = findDataItem(handler.letter);

				if(item != null)
				{
					switch(handler.points)
					{
					case "x2":
						item.x2Count = handler.quantity;
						break;
					case "x3":
						item.x3Count = handler.quantity;
						break;
					default:
						item.points = int.Parse(handler.points);
						item.quantity = handler.quantity;
						break;
					}
				}
			}

			updateShowedData();
		}

		private void resetDataItems()
		{
			foreach(GridDataItem item in items)
			{
				item.points = 0;
				item.quantity = 0;
				item.x2Count = 0;
				item.x3Count = 0;
			}
		}

		public GridDataItem findDataItem(char value)
		{
			foreach(GridDataItem item in items)
			{
				if((item.data as AlfabetUnit).cvalue == value )
				{
					return item;
				}
			}

			return null;
		}

		public string getCSVData(string forcedType)
		{
			StringBuilder builder = new StringBuilder();
			ABCStringHandler handler = new ABCStringHandler();

			for(int i = 0; i < items.Count; i++)
			{
				handler.quantity = items[i].quantity;
				handler.type = forcedType;
				handler.letter = (items[i].data as AlfabetUnit).cvalue;

				if(handler.quantity != 0)
				{
					handler.points = items[i].points.ToString();	

					if(builder.Length != 0)
					{
						builder.Append(",");
					}

					builder.Append(handler.getString());
				}

				if(items[i].x2Count > 0)
				{
					handler.points = "x2";
					handler.quantity = items[i].x2Count;
					if(builder.Length != 0)
					{
						builder.Append(",");
					}
					builder.Append(handler.getString());
				}

				if(items[i].x3Count > 0)
				{
					handler.points = "x3";
					handler.quantity = items[i].x3Count;
					if(builder.Length != 0)
					{
						builder.Append(",");
					}
					builder.Append(handler.getString());
				}
			}

			return builder.ToString();
		}

		public void updateShowedData()
		{
			selector.updateAllRenderersShowedData();
			txtInfo.text = getInfoString();	
		}
	}

	public class ABCStringHandler
	{
		public int quantity;
		public char letter;
		public string points;
		public string type;

		public ABCStringHandler()
		{
		}

		public ABCStringHandler(string value)
		{
			setString(value);
		}

		public void setString(string value)
		{
			// cantidad_letra_puntos_tipo
			string[] splitted = value.Split('_');
			quantity = int.Parse(splitted[0]);
			letter = splitted[1].ToCharArray()[0];
			points = splitted[2];
			type = splitted[3];
		}

		public string getString()
		{
			// cantidad_letra_puntos_tipo
			StringBuilder builder = new StringBuilder();

			builder.Append(quantity.ToString("00"));
			builder.Append("_");
			builder.Append(letter);
			builder.Append("_");
			builder.Append(points);
			builder.Append("_");
			builder.Append(type);

			return builder.ToString();
		}
	}
}