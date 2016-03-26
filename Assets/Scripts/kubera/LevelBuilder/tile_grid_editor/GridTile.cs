using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class GridTile : MonoBehaviour 
	{
		public delegate void DOnClick(GridTile target);
		public DOnClick onClick;

		[HideInInspector]private int dataValue;

		private Image tileImage;
		private Sprite spriteToShow = null;

		void Start () 
		{
			GetComponent<Button>().onClick.AddListener(onButtonClick);
			tileImage = GetComponent<Image>();

			onClick+=foo;

			if(spriteToShow != null)
			{
				setSpriteToShow(spriteToShow);
			}
		}

		private void foo(GridTile t){}

		private void onButtonClick()
		{
			onClick(this);
		}
			
		public void setDataValue(int value){dataValue = value;}

		public int getValue()
		{
			return  dataValue;
		}

		public void setSpriteToShow(Sprite sprite)
		{
			if(tileImage == null)
			{
				spriteToShow = sprite;
			}
			else
			{
				tileImage.sprite = sprite;	
			}
		}
	}
}