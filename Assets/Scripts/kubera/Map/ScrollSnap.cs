using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollSnap : MonoBehaviour 
{
	public ScrollRect scrollRect;
	public float lerpTime;

	protected bool coroutineSend;
	protected bool onLerp;
	protected int closerIndex;
	protected float distance;

	protected float lerpPercent;
	protected float elapsedTime;
	protected float stepTime;

	protected bool waiting;

	void Start()
	{
		stepTime = Time.deltaTime/lerpTime;

		scrollRect.normalizedPosition = new Vector2 (0,0);

		distance = 1.0f / (scrollRect.content.childCount-1);
	}

	void Update()
	{
		if (onLerp) 
		{
			if (elapsedTime < lerpTime) 
			{
				lerpPercent += stepTime;
				scrollRect.normalizedPosition = 
					Vector2.Lerp(scrollRect.normalizedPosition,new Vector2(0,closerIndex * distance),lerpPercent);
				elapsedTime += Time.deltaTime;
			} 
			else 
			{
				onLerp = false;
				coroutineSend = false;
				lerpPercent = 0;
				elapsedTime = 0;
				scrollRect.enabled = true;
			}
		}
		if (waiting) 
		{
			#if UNITY_EDITOR
			if (scrollRect.velocity == Vector2.zero && !Input.GetMouseButton(0)) 
			{
				startSnap ();
			}
			#else
			if (scrollRect.velocity == Vector2.zero && Input.touchCount == 0) 
			{
				startSnap ();
			}
			#endif
		}
	}

	public void OnScrollFinished()
	{
		waiting = true;
	}

	protected void startSnap()
	{
		scrollRect.enabled = false;

		analizeCloserToZero ();

		onLerp = true;
		waiting = false;
	}

	protected void analizeCloserToZero()
	{
		closerIndex = Mathf.RoundToInt(scrollRect.normalizedPosition.y / distance);
	}
}