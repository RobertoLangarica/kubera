using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIChar : MonoBehaviour 
{
	[HideInInspector]
	public ABCChar character;

	protected Text textfield;
	protected Image myImage;

	// Use this for initialization
	void Start () 
	{
		//textfield = GetComponentInChildren<Text>();
		//textfield.text = character.character;
		myImage = GetComponent<Image> ();

		myImage.sprite = PieceManager.instance.changeTexture (character.character.ToLower () + "1");
		//gameObject.transform.localScale = new Vector3(4, 4, 4);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
