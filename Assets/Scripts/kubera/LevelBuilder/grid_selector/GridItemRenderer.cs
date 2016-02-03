using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GridItemRenderer : MonoBehaviour 
	{
		public Text txtQuantity;
		public GameObject container;

		public delegate void DNotification(GridItemRenderer target);
		public DNotification onQuantityChange;

		[HideInInspector]public int index = 0;
		private int quantity = 0;

		void Start()
		{
			onQuantityChange += foo;
		}

		void foo(GridItemRenderer target){}

		public void add()
		{
			quantity++;
			onQuantityChange(this);
			showQuantity();
		}

		public void remove()
		{
			quantity = quantity == 0 ? 0 : quantity-1;
			onQuantityChange(this);
			showQuantity();
		}

		public void showQuantity()
		{
			txtQuantity.text = quantity.ToString("000");
		}

		public void setQuantity(int value)
		{
			quantity = value;
			showQuantity();
		}

		public int getQuantity()
		{
			return quantity;
		}

		public void setObjectToShow(GameObject objectToShow)
		{
			objectToShow.transform.SetParent(container.transform);
		}
	}
}