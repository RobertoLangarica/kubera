using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace LevelBuilder
{
	public class GridSelector : MonoBehaviour 
	{
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
					item.onQuantityChange -= onItemQuantityChange;
					GameObject.DestroyImmediate(item);
				}	
			}
		}

		private void createItemRenderersUsingData()
		{
			itemRenderers = new List<GridItemRenderer>(data.Count);		

			for(int i = 0; i < data.Count; i++)
			{
				GridItemRenderer item = instantiateItemRenderer();
				item.onQuantityChange += onItemQuantityChange;
				item.index = i;
				item.setQuantity(data[i].quantity);
				item.setObjectToShow(data[i].objectToShow);
				itemRenderers.Add(item);
			}
		}

		private GridItemRenderer instantiateItemRenderer()
		{
			return GameObject.Instantiate(ItemRendererPrefab).GetComponent<GridItemRenderer>();
		}

		private void onItemQuantityChange(GridItemRenderer target)
		{
			data[target.index].quantity = target.getQuantity();
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
			txtTitle.text = title;
		}

		public void showAllQuantities()
		{
			for(int i = 0; i < itemRenderers.Count; i++)
			{
				itemRenderers[i].showQuantity();
			}
		}
	}
}