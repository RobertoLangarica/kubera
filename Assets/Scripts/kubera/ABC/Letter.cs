using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ABC;

public class Letter : MonoBehaviour 
{
	public enum EType
	{
		OBSTACLE, NORMAL	
	}

	public Image myImage;
	public Color selectedColor = new Color(1,1,1,0.2f);
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
			return Color.grey;
		case EType.NORMAL:
			return Color.white;
		}
		return Color.white;
	}

	public bool isPreviouslySelected()
	{
		return selected;
	}

	private void markAsSelected()
	{
		selected=true;
		updateColorToSelected ();
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

	protected void updateColorToSelected()
	{
		if(myImage == null){return;}

		myImage.color = selectedColor;	
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