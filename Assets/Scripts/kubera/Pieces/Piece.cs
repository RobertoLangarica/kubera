using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ECOLORS_ID
{
	NONE,
	AQUA,
	BLACK,
	BLUE,
	GREEN,
	GREY,
	MAGENTA,
	RED,
	//WHITE,
	YELLOW,
	LETER
}

public class Piece : MonoBehaviour {

	public GameObject[] pieces;
	public ECOLORS_ID initialColor;
	public SpriteRenderer spriteRenderer; 
	public Image image;
	
	protected ECOLORS_ID currentColor;
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

	public bool firstPiece;
	public GameObject[] rotatePieces;

	// Use this for initialization
	void Start () {
		selectAColor ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void selectAColor()
	{
		//Color myColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
		color = (ECOLORS_ID)Random.Range(1,8);

		foreach(GameObject piece in pieces)
		{
			piece.GetComponent<SpriteRenderer>().color = rendererColor;
			piece.GetComponent<Tile>().color = currentColor;
		}
	}

	public ECOLORS_ID color
	{
		//get{return spriteRenderer.color;}
		//set{spriteRenderer.color = value;}
		
		get{return currentColor;}
		set
		{
			initialColor = value;
			currentColor = value;
			
			switch(currentColor)
			{
			case ECOLORS_ID.AQUA:
				rendererColor = new Color(0.376f, 0.698f, 0.639f); 
				break;
			case ECOLORS_ID.BLACK:
				rendererColor = new Color(0.180f, 0.188f, 0.192f);
				break;
			case ECOLORS_ID.BLUE:
				rendererColor = new Color(0.133f, 0.565f, 0.945f);
				break;
			case ECOLORS_ID.GREEN:
				rendererColor = new Color(0.192f, 0.545f, 0.263f);
				break;
			case ECOLORS_ID.GREY:
				rendererColor = new Color(0.392f, 0.514f, 0.584f);
				break;
			case ECOLORS_ID.MAGENTA:
				rendererColor = new Color(0.643f, 0.059f, 0.482f);
				break;
			case ECOLORS_ID.RED:
				rendererColor = new Color(0.965f, 0.282f, 0.427f);
				break;
//			case ECOLORS_ID.WHITE:
//				rendererColor = new Color(0.910f, 0.937f, 0.957f);
//				break;
			case ECOLORS_ID.YELLOW:
				rendererColor = new Color(0.976f, 0.627f, 0.000f);
				break;
			case ECOLORS_ID.NONE:
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
