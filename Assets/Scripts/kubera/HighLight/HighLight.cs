﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HighLight : MonoBehaviour 
{
	public enum EHIGHLIGHPARENTTYPE
	{
		NONE,
		CELL,
		RECTANGLE_BUTTON,
		CIRCLE_BUTTON,
		PIECES_AREA,
		WORD_AREA
	}

	public float minAlpha = 0.45f;
	public float animStep = 0.02f;

	public GameObject borderStars;

	public EHIGHLIGHPARENTTYPE parentType;

	protected bool isScaled;
	protected bool isDescending;
	protected bool startAnim;

	protected bool isActive;
	protected List<HighLightManager.EHighLightType> suscribedTypes = new List<HighLightManager.EHighLightType> ();
	protected List<HighLightManager.EHighLightStatus> suscribedStatus = new List<HighLightManager.EHighLightStatus> ();

	void Update()
	{
		if (startAnim) 
		{
			if (isDescending) 
			{
				if (getModifiedAlpha(-animStep) <= minAlpha) 
				{
					isDescending = false;
				}
			} 
			else 
			{

				if (getModifiedAlpha(animStep) >= 1) 
				{
					isDescending = true;
				}
			}
		}
	}

	public bool activateHighLight(HighLightManager.EHighLightType type,HighLightManager.EHighLightStatus status)
	{
		if (!isScaled) 
		{
			setScale ();
		}

		int index = suscribedTypes.IndexOf (type);

		if (index < 0) 
		{
			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (true);
			}

			suscribedTypes.Add (type);
			suscribedStatus.Add (status);

			updateColor ();

			initAnim ();

			return true;
		}
		return false;
	}

	protected void updateColor()
	{
		Color temp = Color.white;
		Image tempImg = null;
		SpriteRenderer tempSpt = null;

		switch (suscribedStatus[suscribedStatus.Count -1]) 
		{
		case(HighLightManager.EHighLightStatus.NORMAL):
			temp = HighLightManager.GetInstance().normalHighLight;
			break;
		case(HighLightManager.EHighLightStatus.WRONG):
			temp = HighLightManager.GetInstance().wrongHighLight;
			break;
		case(HighLightManager.EHighLightStatus.HINT):
			temp = HighLightManager.GetInstance().hintHighLight;
			break;
		}

		tempImg = gameObject.GetComponent<Image>();
		if (tempImg != null) 
		{
			tempImg.color = temp;
		} 
		else 
		{
			tempSpt = gameObject.GetComponent<SpriteRenderer> ();
			if (tempSpt != null) 
			{
				tempSpt.color = temp;
			}
		}

		if (borderStars != null) 
		{
			borderStars.SetActive (true);
		}
	}

	protected void initAnim()
	{
		if (!startAnim)
		{
			isDescending = true;
		}

		startAnim = true;
	}

	protected void finishAnim()
	{
		startAnim = false;

		getModifiedAlpha (0,true);
	}

	protected float getModifiedAlpha(float value,bool reset = false)
	{
		SpriteRenderer tempSpt = GetComponent<SpriteRenderer>();
		Image tempImg = GetComponent<Image>();

		Color tempCol = Color.white;

		if (tempSpt != null) 
		{
			if (!reset) 
			{
				tempCol = new Color (tempSpt.color.r, tempSpt.color.g, tempSpt.color.b, tempSpt.color.a + value);
			}

			tempSpt.color = tempCol;
		} 
		else if (tempImg != null) 
		{
			if (!reset) 
			{
				tempCol = new Color (tempImg.color.r, tempImg.color.g, tempImg.color.b, tempImg.color.a + value);
			} 

			tempImg.color = tempCol;

		}

		return tempCol.a;
	}

	public bool completlyDeactivateType(HighLightManager.EHighLightType type)
	{
		int index = suscribedTypes.IndexOf (type);

		if (index >= 0) 
		{
			suscribedTypes.RemoveAt (index);
			suscribedStatus.RemoveAt (index);

			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (false);
				if (borderStars != null) 
				{
					borderStars.SetActive (false);
				}

				finishAnim ();

				return true;
			} 
			else 
			{
				updateColor ();
				return false;
			}
		}
		return false;
	}

	protected void setScale()
	{
		switch (parentType) 
		{
		case(EHIGHLIGHPARENTTYPE.CELL):
			scaleSpriteToFather ();
			break;
		}
	}

	public void scaleSpriteToFather()
	{
		if (!isScaled) 
		{
			float percent = (GetComponent<SpriteRenderer> ().bounds.size.x / FindObjectOfType<CellsManager>().cellSize) * 0.1f;
			percent *= 1.18f;

			transform.localScale = new Vector3 (percent, percent, percent);

			Cell tempC = transform.parent.GetComponent<Cell> ();
			borderStars.transform.position = transform.position = tempC.transform.position + (new Vector3 (tempC.GetComponent<SpriteRenderer> ().bounds.extents.x,
				-tempC.GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

			isScaled = true;
		}
	}
}