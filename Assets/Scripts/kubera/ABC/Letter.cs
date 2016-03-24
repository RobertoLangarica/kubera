using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class Letter : MonoBehaviour 
	{
		public enum EType
		{
			OBSTACLE, NORMAL	
		}

		public Image myImage;
		public Color selectedColor = new Color(1,1,1,0.2f);
		public Text txtLetter;
		public Text txtPoints;
		[HideInInspector] public Letter letterReference;
		[HideInInspector] public bool isFromGrid;
		[HideInInspector] public EType type;
		[HideInInspector] public bool selected;
		[HideInInspector]public int index;//Indice del caracter en WordManager
		protected bool textActualized; //texto actualizado

		public ABCChar abcChar;

		public void changeSpriteRendererTexture(Sprite newSprite)
		{
			GetComponent<SpriteRenderer> ().sprite = newSprite;
			updateSpriteColor();
		}

		public void changeImageTexture(Sprite newSprite)
		{
			GetComponent<Image> ().sprite = newSprite;//PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			updateImageColor();
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

		public Color getColorBasedOnType(EType type)
		{
			switch(type)
			{
			case EType.OBSTACLE:
				return Color.grey;
			case EType.NORMAL:
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
			updateColorToSelected ();
		}

		private void markAsUnselected()
		{
			selected=false;
			updatecolor();
		}

		public void deselect()
		{
			markAsUnselected();

			if(letterReference != null)
			{
				letterReference.markAsUnselected();
				letterReference.letterReference = null;
			}

			letterReference = null;
		}

		protected void updateColorToSelected()
		{
			gameObject.GetComponent<Image> ().color = selectedColor;
		}

		public void updateTexts()
		{
			if(txtLetter != null && txtPoints != null)
			{
				txtLetter.text = abcChar.character;
				txtPoints.text = abcChar.pointsOrMultiple;
			}
		}

		public void initializeFromInfo(ABCCharinfo info)
		{
			abcChar = new ABCChar();
			abcChar.character = info.character;
			abcChar.pointsOrMultiple = info.pointsOrMultiple;
			type = (EType)info.type;

			if(abcChar.character == ".")
			{
				abcChar.wildcard = true;
			}

			updateTexts ();
		}
	}
}