﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ABC;

public class Letter : MonoBehaviour 
{
	public enum EType
	{
		OBSTACLE, NORMAL,WILD_CARD	
	}

	public enum EState
	{
		WRONG,NORMAL,HINTED	
	}

	public Image myImage;
	public Sprite normalSprite;
	public Sprite normalSelectedSprite;
	public Sprite obstacleSprite;
	public Sprite obstacleSelectedSprite;
	public Sprite wildCardSprite;

	public Color wrongColor = new Color (1, 0, 0);
	public Color hintedWordsColor = new Color (0, 1, 0);

	public Text txtLetter;
	public Text txtPoints;
	[HideInInspector] public Letter letterReference;
	[HideInInspector] public bool isFromGrid;
	[HideInInspector] public EType type;
	[HideInInspector] public EState state;
	[HideInInspector] public bool selected;
	[HideInInspector]public int index;//Indice del caracter en WordManager
	[HideInInspector]public bool wildCard;
	protected bool textActualized; //texto actualizado

	public ABCChar abcChar;

	void Start()
	{
		myImage = gameObject.GetComponent<Image>();	
	}

	public void changeImageTexture(Sprite newSprite)
	{
		GetComponent<Image> ().sprite = newSprite;
		updateSprite();
	}

	public void updateSprite()
	{
		if(myImage == null){return;}

		myImage.sprite = getSpriteBasedOnType(type);
		setColorsBasedOnType (type);
	}

	/*public Color getColorBasedOnType(EType type)
	{
		switch(type)
		{
		case EType.OBSTACLE:
			txtLetter.color = Color.white;
			txtPoints.color = Color.white;
			return Color.black;
		case EType.NORMAL:
			return Color.white;
		case EType.WILD_CARD:
			return wildCardColor;
		}
		return Color.white;
	}*/

	public void setColorsBasedOnType(EType type)
	{
		switch(type)
		{
		case EType.OBSTACLE:
			if(state == EState.NORMAL)
			{
				txtLetter.color = Color.white;
				txtPoints.color = Color.black;
			}
			else
			{
				txtLetter.color = wrongColor;
				txtPoints.color = Color.black;
			}
			break;
		case EType.NORMAL:
			break;
		case EType.WILD_CARD:
			break;
		}
	}

	public Sprite getSpriteBasedOnType(EType type)
	{
		switch(type)
		{
		case EType.OBSTACLE:
			return obstacleSprite;
		case EType.NORMAL:
			return normalSprite;
		case EType.WILD_CARD:
			return wildCardSprite;
		}
		return normalSprite;
	}

	public void updateState(EState state)
	{
		this.state = state;
		switch(state)
		{
		case EState.NORMAL:
			if(type == EType.NORMAL)
			{
				txtLetter.color = Color.black;
			}
			else if(type == EType.OBSTACLE)
			{
				txtLetter.color = Color.white;
			}
			//txtPoints.color = stateColor;
			break;
		case EState.WRONG:
			txtLetter.color = wrongColor;
			//txtPoints.color = stateColor;
			break;
		case EState.HINTED:
			txtLetter.color = hintedWordsColor;
			//txtPoints.color = stateColor;
			break;
		}
	}

	/*protected void updateColorToSelectedBasedOnType()
	{
		if(myImage == null){return;}


		switch(type)
		{
		case EType.OBSTACLE:
			txtLetter.color = selectedColor;
			txtPoints.color = selectedColor;
			myImage.color = Color.black;
			break;
		case EType.NORMAL:
			myImage.color = selectedColor;
			break;
		}
	}*/

	protected void updateSpriteToSelectedBasedOnType()
	{
		if(myImage == null){return;}

		switch(type)
		{
		case EType.OBSTACLE:
			myImage.sprite = obstacleSelectedSprite;
			break;
		case EType.NORMAL:
			myImage.sprite = normalSelectedSprite;
			break;
		}
	}

	public bool isPreviouslySelected()
	{
		return selected;
	}

	private void markAsSelected()
	{
		selected=true;
		updateSpriteToSelectedBasedOnType ();
	}

	private void markAsUnselected()
	{
		selected=false;
		updateSprite();
	}

	public void select()
	{
		markAsSelected();

		if(letterReference != null)
		{
			setColorsBasedOnType (type);
			letterReference.markAsSelected();
		}
	}

	public void deselect()
	{
		markAsUnselected();

		if(letterReference != null)
		{
			letterReference.markAsUnselected();
			letterReference.letterReference = null;
		}

		letterReference = null;
	}



	public void updateTexts()
	{
		if(txtLetter != null && txtPoints != null)
		{
			txtLetter.text = abcChar.character;
			txtPoints.text = abcChar.pointsOrMultiple;
		}
	}

	public void initializeFromABCChar(ABCChar abc)
	{
		abcChar = abc;
		type = (EType)abcChar.typeInfo;

		if(abcChar.character == ".")
		{
			abcChar.wildcard = true;
		}
	}


}