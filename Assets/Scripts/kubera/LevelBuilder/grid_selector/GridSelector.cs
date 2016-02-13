using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LevelBuilder
{
	public class GridSelector : MonoBehaviour 
	{
		public delegate void DNotification();
		public DNotification OnDataChange;

		public GameObject ItemRendererPrefab;
		public Text txtTitle;
		public Transform gridContainer;

		private List<GridDataItem> data;
		private List<GridItemRenderer> itemRenderers;

		public void setData(List<GridDataItem> newData)
		{
			data = newData;
			cleanItemRenderers();
			createItemRenderersUsingData();
			layoutItemRenderers();
		}

		public List<GridDataItem> getData(){return data;}

		private void cleanItemRenderers()
		{
			if(itemRenderers != null)
			{
				foreach(GridItemRenderer item in itemRenderers)
				{
					item.OnDataChange -= onRendererDataChange;
					GameObject.DestroyImmediate(item.gameObject);
				}	
			}
		}

		private void createItemRenderersUsingData()
		{
			itemRenderers = new List<GridItemRenderer>(data.Count);		

			for(int i = 0; i < data.Count; i++)
			{
				GridItemRenderer item = instantiateItemRenderer();
				item.index = i;
				item.setData(data[i]);
				item.setObjectToShow(data[i].objectToShow);
				item.showData();
				item.OnDataChange += onRendererDataChange;
				itemRenderers.Add(item);
			}
		}

		private void onRendererDataChange()
		{
			OnDataChange();
		}

		private GridItemRenderer instantiateItemRenderer()
		{
			return GameObject.Instantiate(ItemRendererPrefab).GetComponent<GridItemRenderer>();
		}

		private void layoutItemRenderers()
		{
			int rowCount = gridContainer.childCount;
			int itemsPerRow = Mathf.CeilToInt(((float)itemRenderers.Count/rowCount));
			int itemCount = 0;
			int currentRow = 0;

			for(int i = 0; i < itemRenderers.Count; i++,itemCount++)
			{
				if(itemCount >= itemsPerRow)
				{
					currentRow++;
					itemCount = 0;
				}

				itemRenderers[i].transform.SetParent(gridContainer.GetChild(currentRow));
			}
		}

		public void setTitle(string title)
		{
			if(txtTitle != null)
			{txtTitle.text = title;}
		}

		public void updateAllRenderersShowedData()
		{
			for(int i = 0; i < itemRenderers.Count; i++)
			{
				itemRenderers[i].showData();
			}
		}
	}
}