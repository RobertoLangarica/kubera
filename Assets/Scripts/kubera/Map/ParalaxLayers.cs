using UnityEngine;
using System.Collections;

public class ParalaxLayers : MonoBehaviour {

	public float value = .5f;

	protected ParalaxManager content;
	public RectTransform rectTransform;

	void Awake()
	{
		content = FindObjectOfType<ParalaxManager> ();
		content.OnMove += OnMove;
	}

	protected void OnMove(Vector2 pos)
	{
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,pos.y*value);
	}
}
