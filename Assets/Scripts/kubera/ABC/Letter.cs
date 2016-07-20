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
	public Sprite blacSprite;
	public Sprite blacSelectedSprite;
	public Sprite wildCardSprite;

	public Color selectedColor = new Color(1,1,1,0.2f);
	public Color wildCardColor = new Color(1,1,1,1);
	public Color wrong = new Color (1, 0, 0);
	public Color hintedWords = new Color (0, 1, 0);

	public Text txtLetter;
	public Text txtPoints;
	[HideInInspector] public Letter letterReference;
	[HideInInspector] public bool isFromGrid;
	[HideInInspector] public EType type;
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
		updateColor();
	}

	public void updateColor()
	{
		if(myImage == null){return;}

		myImage.color = getColorBasedOnType(type);
	}

	public Color getColorBasedOnType(EType type)
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
	}

	/*public Sprite getSpriteBasedOnType(EType type)
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

	public void updateState(EState state)
	{
		switch(state)
		{
		case EState.NORMAL:
			txtLetter.color = Color.black;
			//txtPoints.color = stateColor;
			break;
		case EState.WRONG:
			txtLetter.color = wrong;
			//txtPoints.color = stateColor;
			break;
		case EState.HINTED:
			txtLetter.color = hintedWords;
			//txtPoints.color = stateColor;
			break;
		}
	}

	protected void updateColorToSelectedBasedOnType()
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
	}

	public bool isPreviouslySelected()
	{
		return selected;
	}

	private void markAsSelected()
	{
		selected=true;
		updateColorToSelectedBasedOnType ();
	}

	private void markAsUnselected()
	{
		selected=false;
		updateColor();
	}

	public void select()
	{
		markAsSelected();

		if(letterReference != null)
		{
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