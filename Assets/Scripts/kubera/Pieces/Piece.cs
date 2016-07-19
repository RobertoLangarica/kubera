using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Piece : MonoBehaviour 
{
	public enum EColor
	{
		NONE,
		YELLOW,
		MAGENTA,
		CYAN,
		GREEN,
		RED,
		PURPLE,
		BLUE,
		ORANGE,
		VIOLET,
		TURQUOISE,
		LETTER_OBSTACLE
	}

	public enum EType
	{
		NONE,
		PIECE,
		LETTER,
		LETTER_OBSTACLE
	}

	public Color COLOR_YELLOW			= new Color(1, 1, 0); 
	public Color COLOR_MAGENTA			= new Color(0.890f, 0.011f, 0.549f);
	public Color COLOR_CYAN				= new Color(0, 0.623f, 1);
	public Color COLOR_GREEN			= new Color(0, 0.788f, 0.278f);
	public Color COLOR_RED				= new Color(1, 0, 0);
	public Color COLOR_PURPLE			= new Color(0.501f, 0.113f, 0.498f);
	public Color COLOR_BLUE				= new Color(0, 0.247f, 1);
	public Color COLOR_ORANGE			= new Color(1, 0.380f, 0);
	public Color COLOR_VIOLET			= new Color(0.282f, 0.156f, 0.670f);
	public Color COLOR_TURQUOISE		= new Color(0, 0.776f, 0.560f);
	public Color COLOR_LETTER_OBSTACLE	= new Color(0, 0, 0);
	public Color COLOR_NONE				= new Color(1, 1, 1);

	public GameObject[] squares;
	public SpriteRenderer[] squaresSprite;
	public SpriteRenderer[] shadows;
	public EType starterType;

	protected EType _currentType;
	protected Color rendererColor;

	public EColor currentColor;

	[HideInInspector]public int createdIndex = 0;

	[HideInInspector]public Vector3 positionOnScene;

	[HideInInspector]public Vector3 initialPieceScale;

	public GameObject toRotateObject;

	void Start () 
	{
		squares = new GameObject[transform.childCount];
		squaresSprite = new SpriteRenderer[transform.childCount];
		shadows = new SpriteRenderer[transform.childCount];

		for(int i=0; i<transform.childCount; i++)
		{
			squares [i] = transform.GetChild (i).gameObject;
			squaresSprite [i] = squares [i].GetComponent<SpriteRenderer> ();
			shadows [i] = squares [i].transform.GetChild(0).GetComponent<SpriteRenderer> ();
		}
	
		currentType = starterType;
	}

	protected void updateColorBasedOnType()
	{
		if(currentType == EType.LETTER_OBSTACLE || squares.Length != 1)
		{
			return;
		}

		Color color = getColorOfType(currentColor);

		foreach(SpriteRenderer piece in squaresSprite)
		{
			piece.color = color;
		}

		/*foreach(GameObject piece in squares)
		{
			piece.GetComponent<SpriteRenderer>().color = color;
		}*/
	}

	public Color getColorOfType(EColor color)
	{
		//print (color);

		switch(color)
		{
		case EColor.YELLOW:
			return COLOR_YELLOW;
		case EColor.MAGENTA:
			return COLOR_MAGENTA;		
		case EColor.CYAN:
			return COLOR_CYAN;
		case EColor.GREEN:
			return COLOR_GREEN;
		case EColor.RED:
			return COLOR_RED;
		case EColor.PURPLE:
			return COLOR_PURPLE;
		case EColor.BLUE:
			return COLOR_BLUE;
		case EColor.ORANGE:
			return COLOR_ORANGE;
		case EColor.VIOLET:
			return COLOR_VIOLET;
		case EColor.TURQUOISE:
			return COLOR_TURQUOISE;
		case EColor.LETTER_OBSTACLE:
			return COLOR_LETTER_OBSTACLE;
		}
		return COLOR_NONE;

	}
		
	public EType currentType
	{
		get{return _currentType;}

		set
		{
			starterType = value;//Para evitar que el Start() modifique este llamado
			_currentType = value;

			updateColorBasedOnType();
		}
	}
}
