using UnityEngine;
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
	void Start()
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
	}

	public void setRectTransform(RectTransform rectTransform)
	{
		this.rectTransform.sizeDelta = rectTransform.sizeDelta;
	}

	public void setPosByCurrentLevel(MapLevel mapLevel)
	{
		print (mapLevel.GetComponent<RectTransform> ().anchorMax.y);
		print ( mapLevel.GetComponent<RectTransform> ().anchorMin.y);
		float sizeOfLevelIcon = mapLevel.GetComponent<RectTransform> ().anchorMax.y - mapLevel.GetComponent<RectTransform> ().anchorMin.y;
		float positionCalculated = mapLevel.GetComponent<RectTransform> ().anchorMin.y/* - sizeOfLevelIcon*/;
		print (positionCalculated);
		scrollRect.verticalNormalizedPosition = positionCalculated;
	}

	public void Unsubscribe()
	{
		OnUnsubscribe ();
	}
}
