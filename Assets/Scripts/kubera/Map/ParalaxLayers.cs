using UnityEngine;
using System.Collections;

public class ParalaxLayers : MonoBehaviour {

	public float value = .5f;

	protected ParalaxManager content;
	public RectTransform rectTransform;

	public bool clamped;
	public float initialValue;

	private float XAnchorPos;//para evitar leer el anchor pos cada frame

	void Start()
	{
		if(rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform> ();
		}
		content = ParalaxManager.instance;
		content.OnMove += OnMove;
		content.OnUnsubscribe += OnUnsubscribe;

		if (clamped) 
		{
			initialValue = rectTransform.anchoredPosition.y;
		}

		XAnchorPos = rectTransform.anchoredPosition.x;
	}

	protected void OnMove(Vector2 pos)
	{
		if(clamped)
		{
			if(pos.y*value > initialValue)
			{
				pos.x = XAnchorPos;
				pos.y = initialValue;
				rectTransform.anchoredPosition = pos;
				return;
			}

			if(pos.y * value < -rectTransform.rect.yMax)
			{
				pos.x = XAnchorPos;
				pos.y = -rectTransform.rect.yMax;
				rectTransform.anchoredPosition = pos;
				return;
			}

		}

		if(value != -1)
		{
			pos.y*=value;
			pos.x = XAnchorPos;
			rectTransform.anchoredPosition = pos;
		}
	}

	protected void OnUnsubscribe()
	{
		content.OnMove -= OnMove;
		content.OnUnsubscribe -= OnUnsubscribe;
	}
}
