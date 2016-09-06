using UnityEngine;
using System.Collections;

public class ParalaxLayers : MonoBehaviour {

	public float value = .5f;

	protected ParalaxManager content;
	public RectTransform rectTransform;

	public bool clamped;
	public float initialValue;

	void Awake()
	{
		if(rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform> ();
		}
		content = FindObjectOfType<ParalaxManager> ();
		content.OnMove += OnMove;
		content.OnUnsubscribe += OnUnsubscribe;
		if (clamped) 
		{
			initialValue = rectTransform.anchoredPosition.y;
		}
	}

	protected void OnMove(Vector2 pos)
	{
		if(clamped)
		{
			if(pos.y*value > initialValue)
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,initialValue);
				return;
			}

			if(pos.y * value < -rectTransform.rect.yMax)
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,-rectTransform.rect.yMax);
				return;
			}

		}
		if(value != -1)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,pos.y*value);
		}
		else
		{
			rectTransform.anchoredPosition = rectTransform.anchoredPosition;
		}
	}

	protected void OnUnsubscribe()
	{
		content.OnMove -= OnMove;
		content.OnUnsubscribe -= OnUnsubscribe;
	}
}
