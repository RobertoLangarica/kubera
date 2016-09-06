using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParalaxManager : MonoBehaviour {

	public delegate void DOnMove(Vector2 newPos);
	public DOnMove OnMove;

	public delegate void DOnFinishToNextLevel();
	public DOnFinishToNextLevel OnFinish;

	public delegate void DOnUnsubscribe();
	public DOnUnsubscribe OnUnsubscribe;

	public RectTransform rectTransform;
	public RectTransform canvasRectTransform;

	protected Vector2 oldPos;
	public ScrollRect scrollRect;

	public bool toDoor;
	public bool toNextLevel;

	protected float posNextLevel;
	protected float fixHeight;
	protected float fixMin;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform> ();
		oldPos = rectTransform.anchoredPosition;
		//OnMove (rectTransform.anchoredPosition);
		//scrollRect.verticalNormalizedPosition = 0.5f;
		Invoke ("setFixHeight",0.05f);
		//print (Screen.height);
	}

	void Update()
	{
		if(oldPos != rectTransform.anchoredPosition)
		{
			if(OnMove != null)
			{
				//Intento de 
				/*//print (rectTransform.localPosition.y);
				if (rectTransform.localPosition.y > Screen.height*0.2f || rectTransform.localPosition.y < -Screen.height*0.2f)
				{
					scrollRect.
				}*/
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
			scrollRect.enabled = false;
			scrollRect.verticalNormalizedPosition = scrollRect.verticalNormalizedPosition + 0.005f;
			if(scrollRect.verticalNormalizedPosition >= posNextLevel)
			{
				toNextLevel = false;
				scrollRect.enabled = true;

			}
		}
	}

	public void setFixHeight()
	{
		float fullHeight = GetComponent<RectTransform> ().rect.height;
		fixHeight = fullHeight * 0.5f;
	
		fixMin = fullHeight - transform.parent.GetComponent<RectTransform> ().rect.height *0.8f;
	}

	public void setRectTransform(RectTransform rectTransform)
	{
		this.rectTransform.sizeDelta = rectTransform.sizeDelta;
	}

	public void setPosByCurrentLevel(float levelPosition)
	{
		scrollRect.verticalNormalizedPosition = levelPosition;
		Canvas.ForceUpdateCanvases ();
	}

	public void setPosLastOrFirst(bool first)
	{
		if(first)
		{
			scrollRect.verticalNormalizedPosition = 0;
		}
		else
		{
			scrollRect.verticalNormalizedPosition = 1;
		}
		Canvas.ForceUpdateCanvases ();
	}

	public float getPosByLevel(MapLevel mapLevel)
	{
		float levelPosition = 0;

		levelPosition = fixHeight + mapLevel.transform.localPosition.y;
		
		levelPosition = levelPosition / fixMin;

		if(levelPosition >1)
		{
			levelPosition = 1;
		}
		else if(levelPosition < 0)
		{
			levelPosition = 0;
		}

		//float sizeOfLevelIcon = mapLevel.GetComponent<RectTransform> ().anchorMax.y - mapLevel.GetComponent<RectTransform> ().anchorMin.y;
		//mapLevel.GetComponent<RectTransform> ().anchorMin.y - sizeOfLevelIcon;

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
		//print (posNextLevel);
	}

	public void cancelAutomaticMovements()
	{
		if(toDoor || toNextLevel)
		{
			//toDoor = false;
			//toNextLevel = false;
		}
	}

	public void finish()
	{
		if(OnFinish != null)
		{
			OnFinish();
		}
	}

	public void Unsubscribe()
	{
		OnUnsubscribe ();
	}
}
