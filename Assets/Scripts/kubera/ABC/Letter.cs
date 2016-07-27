using UnityEngine;
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
		NORMAL,WRONG,HINTED	
	}

	public Image myImage;
	public Sprite normalSprite;
	public Sprite normalSelectedSprite;
	public Sprite normalWrongSprite;
	public Sprite obstacleSprite;
	public Sprite obstacleSelectedSprite;
	public Sprite obstacleWrongSprite;
	public Sprite wildCardSprite;

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
		print ("setColorsBasedOnType");
		print (type);
		print (state);
		switch(type)
		{
		case EType.OBSTACLE:
			if(state == EState.NORMAL)
			{
				txtLetter.color = Color.white;
				txtPoints.color = Color.black;
			}
			else if( state == EState.WRONG)
			{
				txtLetter.color = Color.white;
				txtPoints.color = new Color(0.62f,0.62f,0.62f);
			}
			break;
		case EType.NORMAL:
			if(state == EState.NORMAL)
			{
				txtLetter.color = Color.black;
				txtPoints.color = Color.white;
			}
			else if( state == EState.WRONG)
			{
				txtLetter.color = new Color(0.62f,0.62f,0.62f);
				txtPoints.color = Color.white;
			}
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
		if(myImage == null){return;}

		this.state = state;
		setColorsBasedOnType (type);
		switch(state)
		{
		case EState.NORMAL:
			if(type == EType.NORMAL)
			{
				myImage.sprite = normalSprite;
			}
			else
			{
				myImage.sprite = obstacleSprite;
			}
			break;
		case EState.WRONG:
			//txtLetter.color = wrongColor;
			//txtPoints.color = stateColor;
			break;
		case EState.HINTED:
			//txtLetter.color = hintedWordsColor;
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