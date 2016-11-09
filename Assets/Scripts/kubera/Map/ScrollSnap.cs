using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScrollSnap : MonoBehaviour 
{
	public ScrollRect scrollRect;
	public float lerpTime;

	protected bool onLerp;
	protected float closerIndex;
	protected float distance;

	protected float lerpPercent;
	protected float elapsedTime;
	protected float stepTime;

	protected bool waiting;
	protected Vector2 initialPos;
	private bool initialized = false;
	private float forcedIndexToScroll = -1;

	void Start()
	{
		if(!initialized)
		{
			initialize();
		}
	}

	public void initialize()
	{
		stepTime = 1.0f/lerpTime;
		distance = 1.0f/(scrollRect.content.childCount-1);
		initialized = true;
	}

	void Update()
	{
		if(forcedIndexToScroll >= 0)
		{
			scrollRect.normalizedPosition = new Vector2(0,forcedIndexToScroll * distance);

			forcedIndexToScroll = -1;
		}

		if (onLerp) 
		{
			if (elapsedTime < lerpTime) 
			{
				lerpPercent += stepTime*Time.deltaTime;
				scrollRect.normalizedPosition = Vector2.Lerp(initialPos,new Vector2(0,closerIndex * distance),lerpPercent);
				elapsedTime += Time.deltaTime;
			} 
			else 
			{
				onLerp = false;
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

	public void scrollToChild(float index, bool animate = true)
	{
		closerIndex = index;

		if(animate)
		{
			onLerp = true;
			initialPos = scrollRect.normalizedPosition;
		}
		else
		{
			if(!initialized)
			{
				forcedIndexToScroll = index;
			}
		}
	}
}