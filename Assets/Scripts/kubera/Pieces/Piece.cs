using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Piece : MonoBehaviour 
{
	public enum EColor
	{
		NONE,
		AQUA,
		BLACK,
		BLUE,
		GREEN,
		GREY,
		MAGENTA,
		RED,
		YELLOW,
		LETTER_OBSTACLE
	}

	public enum EType
	{
		NONE,
		PIECE,
		LETTER,
		LETTER_OBSTACLE
	}


	public Color COLOR_AQUA				= new Color(0.376f, 0.698f, 0.639f); 
	public Color COLOR_BLACK			= new Color(0.180f, 0.188f, 0.192f);
	public Color COLOR_LETTER_OBSTACLE	= new Color(0.180f, 0.188f, 0.192f);
	public Color COLOR_BLUE				= new Color(0.133f, 0.565f, 0.945f);
	public Color COLOR_GREEN			= new Color(0.192f, 0.545f, 0.263f);
	public Color COLOR_GREY				= new Color(0.392f, 0.514f, 0.584f);
	public Color COLOR_MAGENTA			= new Color(0.643f, 0.059f, 0.482f);
	public Color COLOR_RED				= new Color(0.965f, 0.282f, 0.427f);
	public Color COLOR_YELLOW			= new Color(0.976f, 0.627f, 0.000f);
	public Color COLOR_NONE				= new Color(1, 1, 1);

	public GameObject[] squares;
	public EType starterType;

	protected EType _currentType;
	protected Color rendererColor;

	public EColor currentColor;

	[HideInInspector]public GameObject parent;

	[HideInInspector]public int createdIndex = 0;

	[HideInInspector]public int rotateTimes = 0;

	[HideInInspector]public int rotateCount = 0;

	[HideInInspector]public Vector3 positionOnScene;

	[HideInInspector]public Vector3 initialPieceScale;

	void Start () 
	{
		squares = new GameObject[transform.childCount];

		for(int i=0; i<transform.childCount; i++)
		{
			squares [i] = transform.GetChild (i).gameObject;
		}
	
		currentType = starterType;
	}

	protected void updateColorBasedOnType()
	{
		if(currentType == EType.LETTER_OBSTACLE)
		{
			return;
		}

		Color color = getColorOfType(currentColor);
			
		foreach(GameObject piece in squares)
		{
			piece.GetComponent<SpriteRenderer>().color = color;
		}
	}

	public Color getColorOfType(EColor color)
	{
		//print (color);

		switch(color)
		{
		case EColor.AQUA:
			return COLOR_AQUA;
		case EColor.BLACK:
			return COLOR_BLACK;
		case EColor.LETTER_OBSTACLE:
			return COLOR_LETTER_OBSTACLE;
		case EColor.BLUE:
			return COLOR_BLUE;
		case EColor.GREEN:
			return COLOR_GREEN;
		case EColor.GREY:
			return COLOR_GREY;
		case EColor.MAGENTA:
			return COLOR_MAGENTA;
		case EColor.RED:
			return COLOR_RED;
		case EColor.YELLOW:
			return COLOR_YELLOW;
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
