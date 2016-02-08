using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LevelBuilder
{
	public class PiecesSelector : MonoBehaviour 
	{
		public GridSelector selector;
		public GameObject PiecePrefab;
		public Sprite[] sprites;

		public	List<GridDataItem> items;
		private bool initialized = false;

		void Start()
		{
			if(!initialized)
			{
				createDataItems();
				setSelectorData();
			}
			initialized = true;
		}

		public void Initialize()
		{
			Start();
		}

		private void createDataItems()
		{
			items = new List<GridDataItem>();

			foreach(Sprite s in sprites)
			{
				items.Add(new GridDataItem());
				items[items.Count-1].data = s.name;
				items[items.Count-1].objectToShow = Instantiate(PiecePrefab) as GameObject;
				items[items.Count-1].objectToShow.GetComponent<Image>().sprite = s;
				items[items.Count-1].quantity = 0;
			}
		}

		private void setSelectorData()
		{
			selector.setData(items);
		}

		public void updateShowedData()
		{
			selector.updateAllRenderersShowedData();
		}

		public void sincronizeDataWithCSV(string csv)
		{
			if(csv.Length == 0)
			{
				resetDataItems();
				selector.updateAllRenderersShowedData();
				return;
			}

			string[] pieces = csv.Split(',');
			PieceStringHandler handler = new PieceStringHandler();
			GridDataItem item;

			resetDataItems();

			for(int i = 0; i < pieces.Length; i++)
			{
				handler.setString(pieces[i]);
				item = findDataItem(handler.name);

				if(item != null)
				{
					item.quantity = handler.quantity;
				}
			}

			selector.updateAllRenderersShowedData();
		}

		public void resetDataItems()
		{
			foreach(GridDataItem item in items)
			{
				item.quantity = 0;
			}
		}

		public GridDataItem findDataItem(string value)
		{
			foreach(GridDataItem item in items)
			{
				if((item.data as string).Equals(value))
				{
					return item;
				}
			}

			return null;
		}

		public string getCSVData()
		{
			StringBuilder builder = new StringBuilder();
			PieceStringHandler handler = new PieceStringHandler();

			for(int i = 0; i < items.Count; i++)
			{
				handler.quantity = items[i].quantity;
				handler.name = items[i].data as string;

				if(handler.quantity != 0)
				{
					if(builder.Length != 0)
					{
						builder.Append(",");
					}

					builder.Append(handler.getString());
				}
			}

			return builder.ToString();
		}
	}

	public class PieceStringHandler
	{
		public int quantity;
		public string name;

		public PieceStringHandler()
		{
		}

		public PieceStringHandler(string value)
		{
			setString(value);
		}

		public void setString(string value)
		{
			// cantidad_nombre
			string[] splitted = value.Split('_');
			quantity = int.Parse(splitted[0]);
			name = splitted[1];
		}

		public string getString()
		{
			// cantidad_nombre
			StringBuilder builder = new StringBuilder();

			builder.Append(quantity.ToString("00"));
			builder.Append("_");
			builder.Append(name);

			return builder.ToString();
		}
	}
}
