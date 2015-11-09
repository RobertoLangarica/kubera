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
	WHITE,
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

	[HideInInspector]
	public Transform myFirstPos;

	// Use this for initialization
	void Start () {
		selectAColor ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void selectAColor()
	{
		Color myColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));

		foreach(GameObject piece in pieces)
		{
			piece.GetComponent<SpriteRenderer>().color = myColor;
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
			case EShapeColor.AQUA:
				rendererColor = new Color(0.376f, 0.698f, 0.639f); 
				break;
			case EShapeColor.BLACK:
				rendererColor = new Color(0.180f, 0.188f, 0.192f);
				break;
			case EShapeColor.BLUE:
				rendererColor = new Color(0.133f, 0.565f, 0.945f);
				break;
			case EShapeColor.GREEN:
				rendererColor = new Color(0.192f, 0.545f, 0.263f);
				break;
			case EShapeColor.GREY:
				rendererColor = new Color(0.392f, 0.514f, 0.584f);
				break;
			case EShapeColor.MAGENTA:
				rendererColor = new Color(0.643f, 0.059f, 0.482f);
				break;
			case EShapeColor.RED:
				rendererColor = new Color(0.965f, 0.282f, 0.427f);
				break;
			case EShapeColor.WHITE:
				rendererColor = new Color(0.910f, 0.937f, 0.957f);
				break;
			case EShapeColor.YELLOW:
				rendererColor = new Color(0.976f, 0.627f, 0.000f);
				break;
			case EShapeColor.NONE:
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
