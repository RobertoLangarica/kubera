using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	public class UIChar : MonoBehaviour 
	{
		[HideInInspector]
		public ABCChar character;

		protected Text textfield;
		protected Image myImage;

		[HideInInspector]
		public GameObject piece;

		// Use this for initialization
		void Start () 
		{
			//textfield = GetComponentInChildren<Text>();
			//textfield.text = character.character;
			myImage = GetComponent<Image> ();

			//myImage.sprite = PieceManager.instance.changeTexture (character.character.ToLower () + "1");
			//gameObject.transform.localScale = new Vector3(4, 4, 4);
		}

		public void DestroyPiece()
		{
			Destroy (piece);
		}



	}
}