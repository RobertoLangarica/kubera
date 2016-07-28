using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Piece : MonoBehaviour 
{
	public enum EColor
	{
		NONE,
		BLUE,
		GREEN,
		ORANGE,
		PINK,
		PURPLE,
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

	public Sprite SPRITE_BLUE;
	public Sprite SPRITE_GREEN;
	public Sprite SPRITE_ORANGE;
	public Sprite SPRITE_PINK;
	public Sprite SPRITE_PURPLE;
	public Sprite SPRITE_YELLOW;

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

	protected void updateSpriteBasedOnType()
	{
		if(currentType == EType.LETTER_OBSTACLE)
		{
			return;
		}

		Sprite sprite = getSpriteByCurrentColor(currentColor);

		foreach(SpriteRenderer piece in squaresSprite)
		{
			piece.sprite = sprite;
		}

		/*foreach(GameObject piece in squares)
		{
			piece.GetComponent<SpriteRenderer>().color = color;
		}*/
	}

	public Sprite getSpriteByCurrentColor(EColor color)
	{
		//print (color);

		switch (color) {
		case EColor.YELLOW:
			return SPRITE_YELLOW;
		case EColor.GREEN:
			return SPRITE_GREEN;
		case EColor.PURPLE:
			return SPRITE_PURPLE;
		case EColor.BLUE:
			return SPRITE_BLUE;
		case EColor.ORANGE:
			return SPRITE_ORANGE;
		case EColor.PINK:
			return SPRITE_PINK;
		}
		return SPRITE_YELLOW;
	}
		
	public EType currentType
	{
		get{return _currentType;}

		set
		{
			starterType = value;//Para evitar que el Start() modifique este llamado
			_currentType = value;

			updateSpriteBasedOnType();
		}
	}
}
