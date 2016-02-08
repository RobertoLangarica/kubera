using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LevelBuilder
{
	public class ButtonTile : MonoBehaviour 
	{

		[HideInInspector]public Sprite spriteToshow;
		public int dataValue;

		public delegate void DNotification(ButtonTile target);
		public DNotification onClickNotification;

		void Start()
		{
			findSpriteToShow();
		}
			
		public void OnClick()
		{
			onClickNotification(this);
		}

		public void findSpriteToShow()
		{
			spriteToshow = GetComponent<Image>().sprite;
		}
	}
}