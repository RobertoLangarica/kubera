﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParalaxManager : MonoBehaviour {

	public delegate void DOnMove(Vector2 newPos);
	public DOnMove OnMove;

	public delegate void DOnUnsubscribe();
	public DOnUnsubscribe OnUnsubscribe;

	public RectTransform rectTransform;

	protected Vector2 oldPos;
	public ScrollRect scrollRect;

	public bool toDoor;
	public bool toNextLevel;

	protected float posNextLevel;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
		oldPos = rectTransform.anchoredPosition;
		//OnMove (rectTransform.anchoredPosition);
		//scrollRect.verticalNormalizedPosition = 0.5f;
		print (Screen.height);
	}

	void Update()
	{
		if(oldPos != rectTransform.anchoredPosition)
		{
			if(OnMove != null)
			{
				OnMove( rectTransform.anchoredPosition);
			}
		}
		if(toDoor)
		{
			scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition + 0.01f;
			if(scrollRect.verticalNormalizedPosition >= 1)
			{
				toDoor = false;
			}
		}
		if(toNextLevel)
		{
			scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition + 0.001f;
			if(scrollRect.verticalNormalizedPosition >= posNextLevel)
			{
				toNextLevel = false;
			}
		}
	}

	public void setRectTransform(RectTransform rectTransform)
	{
		this.rectTransform.sizeDelta = rectTransform.sizeDelta;
	}

	public void setPosByCurrentLevel(float levelPosition)
	{
		print (levelPosition);
		scrollRect.verticalNormalizedPosition = levelPosition;
	}

	public float getPosByLevel(MapLevel mapLevel)
	{
		//print (mapLevel.GetComponent<RectTransform> ().anchorMax.y);
		//print ( mapLevel.GetComponent<RectTransform> ().anchorMin.y);
		float sizeOfLevelIcon = mapLevel.GetComponent<RectTransform> ().anchorMax.y - mapLevel.GetComponent<RectTransform> ().anchorMin.y;
		float levelPosition = mapLevel.GetComponent<RectTransform> ().anchorMin.y/* - sizeOfLevelIcon*/;
		//print (levelPosition);

		return levelPosition;
	}

	public void setPosToDoor()
	{
		toDoor = true;
	}

	public void setPosToNextLevel(MapLevel mapLevel)
	{

		posNextLevel = getPosByLevel (mapLevel);
		toNextLevel = true;
	}

	public void cancelAutomaticMovements()
	{
		if(toDoor || toNextLevel)
		{
			toDoor = false;
			toNextLevel = false;
		}
	}


	public void Unsubscribe()
	{
		OnUnsubscribe ();
	}
}
