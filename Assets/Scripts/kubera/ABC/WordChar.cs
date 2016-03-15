using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class WordChar : MonoBehaviour 
	{
		public Image myImage;
		public GameObject gridLetterReference;
		public bool isFromGrid;
		public Color selectedColor = new Color(1,1,1,0.2f);

		[HideInInspector]public ABCChar.EType type;
		[HideInInspector]public bool selected;

		public void changeSpriteRendererTexture(Sprite newSprite)
		{
			GetComponent<SpriteRenderer> ().sprite = newSprite;//PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			GetComponent<SpriteRenderer> ().color = Color.white;
			updateSpriteColor();
		}

		public void changeImageTexture(Sprite newSprite)
		{
			GetComponent<Image> ().sprite = newSprite;//PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			GetComponent<Image> ().color = Color.white;
		}

		public void destroyLetterFromGrid()
		{
			Destroy(gridLetterReference);
		}
	
		public void destroy()
		{
			//reference back to unused state
			gridLetterReference.GetComponent<WordChar> ().markAsUnselected();
			DestroyImmediate (gameObject);
		}

		public void updatecolor()
		{
			updateSpriteColor();
			updateImageColor();
		}

		protected void updateImageColor()
		{
			Image image = gameObject.GetComponent<Image>();

			if(image == null){return;}
			image.color = getColorBasedOnType(type);
		}

		protected void updateSpriteColor()
		{
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

			if(spriteRenderer == null){return;}
			spriteRenderer.color = getColorBasedOnType(type);
		}

		public Color getColorBasedOnType(ABCChar.EType type)
		{
			switch(type)
			{
			case ABCChar.EType.OBSTACLE:
				return Color.grey;
			case ABCChar.EType.NORMAL:
				return Color.white;
			}
			return Color.white;
		}

		public bool isPreviouslySelected()
		{
			return selected;
		}

		public void markAsSelected()
		{
			selected=true;
			changeColorToUsed ();
		}

		public void markAsUnselected()
		{
			selected=false;
			updatecolor();
		}

		protected void changeColorToUsed()
		{
			gameObject.GetComponent<Image> ().color = selectedColor;
		}
	}
}