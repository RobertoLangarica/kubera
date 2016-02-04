using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class UIChar : MonoBehaviour 
	{
		[HideInInspector]
		public ABCChar character;
		[HideInInspector]
		public string typeOfLetter;

		protected Text textfield;
		protected Image myImage;

		[HideInInspector]
		public GameObject piece;

		// Use this for initialization
		void Start () 
		{
			//textfield = GetComponentInChildren<Text>();
			//textfield.text = character.character;
			//myImage = GetComponent<Image> ();

			//myImage.sprite = PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			//gameObject.transform.localScale = new Vector3(4, 4, 4);
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

		public void DestroyPiece()
		{
			Destroy (piece);
		}

		protected void setColorToSpriteRendererTextureByType()
		{
			SpriteRenderer abcCharSprite = gameObject.GetComponent<SpriteRenderer>();

			if(abcCharSprite == null)
			{
				return;
			}

			switch(typeOfLetter)
			{
			case("0")://Son las letras que estan desde el inicio y bloquean las lineas
				abcCharSprite.color = Color.grey;
				break;
			case("1")://Letras normales
				abcCharSprite.color = Color.white;
				break;
			}
		}
	}
}