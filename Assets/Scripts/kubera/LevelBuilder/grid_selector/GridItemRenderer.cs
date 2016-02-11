using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GridItemRenderer : MonoBehaviour 
	{
		public Text txtQuantity;
		public GameObject container;

		protected GridDataItem data;

		[HideInInspector]public int index = 0;

		protected void foo(GridItemRenderer target){}

		public void addQuantity()
		{
			data.quantity++;
			showData();
		}

		public void substractQuantity()
		{
			data.quantity = data.quantity == 0 ? 0 : data.quantity-1;
			showData();
		}

		public virtual void showData()
		{
			txtQuantity.text = data.quantity.ToString("00");
		}

		public virtual void setData(GridDataItem newData)
		{
			data = newData;
		}

		public int getQuantity()
		{
			return data.quantity;
		}

		public void setObjectToShow(GameObject objectToShow)
		{
			objectToShow.transform.SetParent(container.transform);
		}
	}
}