using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ETYPEOFPIECE_ID
{
	NONE,
	AQUA,
	BLUE,
	GREEN,
	GREY,
	MAGENTA,
	RED,
	YELLOW,
	BLACK,
	LETTER,
	LETTER_FROM_BEGINING,
	LETTER_DESTROY_POWERUP,
	LETTER_ROTATE_POWERUP,
	LETTER_WILD_POWERUP,
	LETTER_BLOCK_POWERUP
}

public class Piece : MonoBehaviour {

	public GameObject[] pieces;
	public ETYPEOFPIECE_ID initialTypeOfPiece;
	public SpriteRenderer spriteRenderer; 
	public Image image;
	
	protected ETYPEOFPIECE_ID currentTypeOfPiece;
	protected Color rendererColor;
	protected float currentAlpha = 1;
	[HideInInspector]
	public bool powerUp;

	[HideInInspector]
	public Transform myFirstPos;
	[HideInInspector]
	public int myFirstPosInt;
	[HideInInspector]
	public GameObject parent;

	public GameObject[] rotatePieces;

	//[HideInInspector]
	public int howManyHasBeenRotated =0;

	public int colorToSet = 1;
	public bool randomColor=false;
	// Use this for initialization
	void Start () {
		selectTypeOfPiece ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void selectTypeOfPiece()
	{
		//Color myColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
		//typeOfPiece = (ETYPEOFPIECE_ID)Random.Range(1,8);

		if(typeOfPiece == ETYPEOFPIECE_ID.LETTER_FROM_BEGINING)
		{
			return;
		}


		foreach(GameObject piece in pieces)
		{
			piece.GetComponent<SpriteRenderer>().color = rendererColor;

			if(colorToSet ==0 && randomColor)
			{
				typeOfPiece = (ETYPEOFPIECE_ID)Random.Range(1,8);
			}
			else
			{
				typeOfPiece = (ETYPEOFPIECE_ID)colorToSet;
			}
			
			foreach(GameObject piece2 in pieces)
			{
				piece2.GetComponent<SpriteRenderer>().color = rendererColor;
			}
		}
	}

	public ETYPEOFPIECE_ID typeOfPiece
	{
		//get{return spriteRenderer.color;}
		//set{spriteRenderer.color = value;}
		
		get{return currentTypeOfPiece;}
		set
		{
			initialTypeOfPiece = value;
			currentTypeOfPiece = value;

			Debug.Log(currentTypeOfPiece);

			switch(currentTypeOfPiece)
			{
			case ETYPEOFPIECE_ID.AQUA:
				rendererColor = new Color(0.376f, 0.698f, 0.639f); 
				break;
			case ETYPEOFPIECE_ID.BLACK:
			case ETYPEOFPIECE_ID.LETTER_FROM_BEGINING:
				rendererColor = new Color(0.180f, 0.188f, 0.192f);
				break;
			case ETYPEOFPIECE_ID.BLUE:
				rendererColor = new Color(0.133f, 0.565f, 0.945f);
				break;
			case ETYPEOFPIECE_ID.GREEN:
				rendererColor = new Color(0.192f, 0.545f, 0.263f);
				break;
			case ETYPEOFPIECE_ID.GREY:
				rendererColor = new Color(0.392f, 0.514f, 0.584f);
				break;
			case ETYPEOFPIECE_ID.MAGENTA:
				rendererColor = new Color(0.643f, 0.059f, 0.482f);
				break;
			case ETYPEOFPIECE_ID.RED:
				rendererColor = new Color(0.965f, 0.282f, 0.427f);
				break;
//			case ECOLORS_ID.WHITE:
//				rendererColor = new Color(0.910f, 0.937f, 0.957f);
//				break;
			case ETYPEOFPIECE_ID.YELLOW:
				rendererColor = new Color(0.976f, 0.627f, 0.000f);
				break;
			case ETYPEOFPIECE_ID.NONE:
				rendererColor = Color.white;
				return;
			}
			
			rendererColor.a = currentAlpha;
			
			if(spriteRenderer)
			{
				spriteRenderer.color = rendererColor;
			}
			else if(image)
			{
				image.color = rendererColor;
			}
		}
	}
}
