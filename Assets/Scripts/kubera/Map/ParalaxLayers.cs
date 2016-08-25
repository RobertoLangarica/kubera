using UnityEngine;
using System.Collections;

public class ParalaxLayers : MonoBehaviour {

	public float value = .5f;

	protected ParalaxManager content;
	public RectTransform rectTransform;

	void Awake()
	{
		if(rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform> ();
		}
		content = FindObjectOfType<ParalaxManager> ();
		content.OnMove += OnMove;
		content.OnUnsubscribe += OnUnsubscribe;
	}

	protected void OnMove(Vector2 pos)
	{
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
