using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class UIChar : MonoBehaviour 
	{
		[HideInInspector]
		public string typeOfLetter;

		public Image myImage;

		//[HideInInspector]
		public GameObject piece;

		protected WordManager wordManager;

		[HideInInspector]
		public bool usedFromGrid;

		//[HideInInspector]
		public bool isFromGrid;

		public Color usedColor = new Color(1,1,1,0.2f);

		// Use this for initialization
		void Start () 
		{
			wordManager = FindObjectOfType<WordManager>();
		}

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

		public void DestroyPiece()
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

			switch(typeOfLetter)
			{
			case("0")://Son las letras que estan desde el inicio y bloquean las lineas
				spriteRenderer.color = Color.grey;
				break;
			case("1")://Letras normales
				spriteRenderer.color = Color.white;
				break;
			}
		}

		protected void setColorToImage()
		{
			Image image = gameObject.GetComponent<Image>();

			if(image == null)
			{
				setColorToSpriteRendererTextureByType ();
				return;
			}
				
			switch(typeOfLetter)
			{
			case("0")://Son las letras que estan desde el inicio y bloquean las lineas
				image.color = Color.grey;
				break;
			case("1")://Letras normales
				image.color = Color.white;
				break;
			}
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