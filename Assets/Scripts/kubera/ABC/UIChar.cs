using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class UIChar : MonoBehaviour 
	{
		public Image myImage;
		public GameObject piece;
		public bool isFromGrid;
		public Color usedColor = new Color(1,1,1,0.2f);

		[HideInInspector]public ABCChar.EType type;
		[HideInInspector]public bool usedFromGrid;

		public void changeSpriteRendererTexture(Sprite newSprite)
		{
			GetComponent<SpriteRenderer> ().sprite = newSprite;//PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			GetComponent<SpriteRenderer> ().color = Color.white;
			setColorToSpriteRendererTextureByType();
		}

		public void changeImageTexture(Sprite newSprite)
		{
			GetComponent<Image> ().sprite = newSprite;//PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			GetComponent<Image> ().color = Color.white;
		}

		public void changeColorAndSetValues(string letter)
		{
			setColorToImage();
		}

		public void destroyPiece()
		{
			Destroy (piece);
		}
	
		public void destroyLetter()
		{
			piece.GetComponent<UIChar> ().backToNormal ();
			DestroyImmediate (gameObject);
		}

		protected void setColorToSpriteRendererTextureByType()
		{
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

			if(spriteRenderer == null)
			{
				return;
			}

			spriteRenderer.color = getColorBasedOnType(type);
		}

		protected void setColorToImage()
		{
			Image image = gameObject.GetComponent<Image>();

			if(image == null)
			{
				setColorToSpriteRendererTextureByType ();
				return;
			}
				
			image.color = getColorBasedOnType(type);
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

		public bool checkIfLetterCanBeUsedFromGrid()
		{
			if(!usedFromGrid)
			{
				usedFromGrid=true;
				changeColorToUsed ();

				return true;
			}
			return false;
		}

		protected void changeColorToUsed()
		{
			gameObject.GetComponent<Image> ().color = usedColor;
		}

		public void backToNormal()
		{
			usedFromGrid=false;
			setColorToImage();
		}
	}
}