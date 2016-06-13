using UnityEngine;
using System.Collections;

public class ParalaxLayers : MonoBehaviour {

	public float value = .5f;

	protected ParalaxManager content;
	public RectTransform rectTransform;

	void Start()
	{
		content = FindObjectOfType<ParalaxManager> ();
		content.OnMove += OnMove;
		print (rectTransform.rect.size+"   "+ name);
	}

	protected void OnMove(Vector2 pos)
	{
		rectTransform.anchoredPosition = pos*value;
	}
}
