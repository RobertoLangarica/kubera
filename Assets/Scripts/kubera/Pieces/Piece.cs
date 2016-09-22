using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

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
		GREY,
		RED,
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
	public Sprite SPRITE_GREY;
	public Sprite SPRITE_RED;

	public GameObject[] squares;
	public SpriteRenderer[] squaresSprite;
	public SpriteRenderer[] shadows;
	public EType starterType;

	protected EType _currentType;
	protected Color rendererColor;

	protected EColor previousColor;

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
		BoxCollider2D boxCollider = GetComponent<BoxCollider2D> ();
		boxCollider.size = new Vector2 (boxCollider.size.x + 0.1f, boxCollider.size.y + 0.1f);
	}

	protected void updateSpriteBasedOnType()
	{
		if(currentType == EType.LETTER_OBSTACLE)
		{
			return;
		}

		Sprite sprite = getSpriteByCurrentColor(currentColor);

		if(sprite == null)
		{
			return;
		}

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
		case EColor.GREY:
			return SPRITE_GREY;
		case EColor.RED:
			return SPRITE_RED;
		}
		return null;
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

	public void switchGreyPiece(bool flag)
	{
		if (flag) 
		{
			if (previousColor == EColor.GREY || previousColor == EColor.NONE) 
			{
				previousColor = currentColor;
			}

			currentColor = EColor.GREY;
		}
		else
		{
			if (previousColor != EColor.NONE && previousColor != EColor.GREY) 
			{
				currentColor = previousColor;
				previousColor = EColor.GREY;
			}
		}

		Sprite sprite = getSpriteByCurrentColor(currentColor);

		if(sprite == null || currentColor == EColor.NONE)
		{
			return;
		}

		foreach(SpriteRenderer piece in squaresSprite)
		{
			piece.sprite = sprite;
		}
	}

	public bool isGrey()
	{
		if(currentColor == EColor.GREY)
		{
			return true;
		}
		return false;
	}

	public void moveAlphaByTween(float alphaPercent,float time,string tweenID,TweenCallback onLastSquareComplete)
	{
		for (int i = 0; i < squaresSprite.Length; i++) 
		{
			if (i == squaresSprite.Length - 1) 
			{
				squaresSprite [i].DOColor (
					new Color (squaresSprite [i].color.r, squaresSprite [i].color.g, squaresSprite [i].color.b, alphaPercent),
					time).SetId (tweenID).OnComplete (onLastSquareComplete);
			} 
			else 
			{
				squaresSprite [i].DOColor (
					new Color (squaresSprite [i].color.r, squaresSprite [i].color.g, squaresSprite [i].color.b, alphaPercent)
					,time).SetId (tweenID);
			}
		}
	}
}
