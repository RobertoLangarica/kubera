using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextScale : MonoBehaviour 
{
	public enum ETextGroups
	{
		TUTORIALS,
		TUTORIALS_SMALL_LETTER
	}

	public ETextGroups textType;

	protected float uiBaseScreenHeight = 512;
	//protected int baseFontSize = 9;

	private int getScaledFOntSize()
	{
		float uiScale = Screen.height / uiBaseScreenHeight;
		int scaledFontSize = Mathf.RoundToInt (getBaseFont() * uiScale);

		return scaledFontSize;
	}

	void Start()
	{
		GetComponent<Text> ().fontSize = getScaledFOntSize ();
	}

	protected int getBaseFont()
	{
		int result = 0;

		switch (textType) 
		{
		case(ETextGroups.TUTORIALS):
			result = 16;
			break;
		case(ETextGroups.TUTORIALS_SMALL_LETTER):
			result = 16;
			break;
		}
		return result;
	}
}
