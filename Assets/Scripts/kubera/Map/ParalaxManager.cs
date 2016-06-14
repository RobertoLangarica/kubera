﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParalaxManager : MonoBehaviour {

	public delegate void DOnMove(Vector2 newPos);
	public DOnMove OnMove;

	public RectTransform rectTransform;

	protected Vector2 oldPos;
	public ScrollRect scrollRect;
	void Start()
	{
		oldPos = rectTransform.anchoredPosition;
		OnMove (rectTransform.anchoredPosition);
		//scrollRect.verticalNormalizedPosition = 0.5f;
		print (Screen.height);
	}

	void Update()
	{
		if(oldPos != rectTransform.anchoredPosition)
		{
			OnMove( rectTransform.anchoredPosition);
		}
	}

	public void setPosByCurrentLevel(MapLevel mapLevel)
	{
		print (mapLevel.GetComponent<RectTransform> ().anchorMax.y);
		print ( mapLevel.GetComponent<RectTransform> ().anchorMin.y);
		float percentage = (mapLevel.GetComponent<RectTransform> ().anchorMax.y + mapLevel.GetComponent<RectTransform> ().anchorMin.y )*0.5f;
		print (percentage);
		scrollRect.verticalNormalizedPosition = percentage;

	}
}
