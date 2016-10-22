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
	public Sprite hintSprite;
	public Sprite hintSelectedSprite;
	public Sprite obstacleSprite;
	public Sprite obstacleSelectedSprite;
	public Sprite obstacleWrongSprite;
	public Sprite wildCardSprite;
	public Sprite wildCardSelectedSprite;

	public Text txtLetter;
	public Text txtPoints;
	[HideInInspector] public Letter letterReference;
	[HideInInspector] public bool isFromGrid;
	[HideInInspector] public EType type;
	[HideInInspector] public EState state;
	[HideInInspector] public bool selected;
	[HideInInspector] public bool hinted;
	[HideInInspector] public int index;//Indice del caracter en WordManager
	[HideInInspector] public bool wildCard;
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
		setColorsBasedOnType (type,state);
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

	public void setColorsBasedOnType(EType type,EState state)
	{
		if(state == EState.HINTED)
		{
			if (!selected) 
			{
				txtLetter.color = Color.black;
				if (txtPoints.text == "x2") 
				{
					txtPoints.color = new Color(1,0.6313f,0.047f);
				}
				else if (txtPoints.text == "x3") 
				{
					txtPoints.color = new Color(1,1,0);
				} 
				else 
				{
					txtPoints.color = Color.white;
				}
			}
			else
			{
				txtLetter.color = Color.black;
				if (txtPoints.text == "x2") 
				{
					txtPoints.color = new Color(1,0.6313f,0.047f);
				}
				else if (txtPoints.text == "x3") 
				{
					txtPoints.color = new Color(1,1,0);
				} 
				else 
				{
					txtPoints.color = Color.black;
				}
			}
			return;
		}
		
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
				if(!selected)
				{
					txtLetter.color = Color.white;
					txtPoints.color = new Color(0.62f,0.62f,0.62f);
				}
				else
				{
					txtLetter.color = new Color(0.62f,0.62f,0.62f);
					txtPoints.color = Color.black;
				}
			}
			break;
		case EType.NORMAL:
			if(state == EState.NORMAL)
			{
				if (!selected) 
				{
					txtLetter.color = Color.black;
					if (txtPoints.text == "x2") 
					{
						txtPoints.color = new Color(1,0.6313f,0.047f);
					}
					else if (txtPoints.text == "x3") 
					{
						txtPoints.color = new Color(1,1,0);
					} 
					else 
					{
						txtPoints.color = Color.white;
					}
				}
				else
				{
					txtLetter.color = Color.black;
					if (txtPoints.text == "x2") 
					{
						txtPoints.color = new Color(1,0.6313f,0.047f);
					}
					else if (txtPoints.text == "x3") 
					{
						txtPoints.color = new Color(1,1,0);
					} 
					else 
					{
						txtPoints.color = Color.black;
					}
				}
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
		if(hinted)
		{
			return hintSprite;
		}
		switch(type)
		{
		case EType.OBSTACLE:
			if(state == EState.WRONG)
			{				
				return obstacleWrongSprite;
			} 
			return obstacleSprite;
		case EType.NORMAL:
			if(state == EState.WRONG)
			{				
				return normalWrongSprite;
			} 
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
		setColorsBasedOnType (type,state);

		switch(state)
		{
		case EState.NORMAL:
			if(type == EType.NORMAL)
			{
				if(selected)
				{
					myImage.sprite = normalSelectedSprite;
				}
				else
				{					
					myImage.sprite = normalSprite;
				}
			}
			else if(type == EType.WILD_CARD)
			{
				if(selected)
				{
					myImage.sprite = wildCardSelectedSprite; 
				}
				else
				{					
					myImage.sprite = wildCardSprite;
				}
			}
			else
			{
				if(selected)
				{
					myImage.sprite = obstacleSelectedSprite;
				}
				else
				{					
					myImage.sprite = obstacleSprite;
				}
			}
			break;
		case EState.WRONG:
			if(type == EType.NORMAL)
			{
				if(selected)
				{
					myImage.sprite = normalSelectedSprite;
				}
				else
				{					
					myImage.sprite = normalWrongSprite;
				}
			}
			else if(type == EType.WILD_CARD)
			{
				if(selected)
				{
					myImage.sprite = wildCardSelectedSprite; 
				}
				else
				{					
					myImage.sprite = wildCardSprite;
				}
			}
			else
			{
				if(selected)
				{
					myImage.sprite = obstacleSelectedSprite;
				}
				else
				{					
					myImage.sprite = obstacleWrongSprite;
				}
			}
			break;
		case EState.HINTED:
			if(selected)
			{
				myImage.sprite = hintSelectedSprite;
			}
			else
			{					
				myImage.sprite = hintSprite;

			}
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

		if(hinted)
		{				
			myImage.sprite = hintSelectedSprite;
			return;
		}

		switch(type)
		{
		case EType.OBSTACLE:
			myImage.sprite = obstacleSelectedSprite;
			break;
		case EType.NORMAL:
			myImage.sprite = normalSelectedSprite;
			break;
		case EType.WILD_CARD:
			myImage.sprite = wildCardSelectedSprite;
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

		setColorsBasedOnType (type,state);
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
			//setColorsBasedOnType (type,state);
			letterReference.letterReference.isFromGrid = true;
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
			//abcChar.wildcard = true;
		}
	}


}