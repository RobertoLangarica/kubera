using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class IntroUIObject : MonoBehaviour 
{

	public enum EOutPosition
	{
		LEFT,RIGHT,TOP,BOTTOM
	}

	public float duration;
	public EOutPosition outPos;
	public bool initOut;
	public bool autoIn;

	protected RectTransform rect;
	protected Vector2 anchoredMinPos;
	protected Vector2 anchoredMaxPos;
	protected Vector2 outMinPos;
	protected Vector2 outMaxPos;

	void Start () 
	{
		rect = GetComponent<RectTransform>();
		anchoredMinPos = rect.anchorMin;
		anchoredMaxPos = rect.anchorMax;
		outMinPos = anchoredMinPos;
		outMaxPos = anchoredMaxPos;

		//Calculamos las posiciones de fuera
		switch(outPos)
		{
		case EOutPosition.BOTTOM:
			outMaxPos.y-= 1;
			outMinPos.y-= 1;
			break;
		case EOutPosition.TOP:
			outMaxPos.y+= 1;
			outMinPos.y+= 1;
			break;
		case EOutPosition.RIGHT:
			outMaxPos.x+= 1;
			outMinPos.x+= 1;
			break;
		case EOutPosition.LEFT:
			outMaxPos.x-= 1;
			outMinPos.x-= 1;
			break;
		}

		if(initOut)
		{
			rect.anchorMax = outMaxPos;
			rect.anchorMin = outMinPos;

		}

		if(autoIn)
		{
			In();
		}
	}

	public void Out()
	{
		animate(outMaxPos,outMinPos,duration);
	}

	public void In()
	{
		animate(anchoredMaxPos,anchoredMinPos,duration);
	}

	public void animate(Vector2 max, Vector2 min, float time)
	{
		DOTween.To (()=>rect.anchorMax, m=>rect.anchorMax = m, max, time).SetDelay(.2f);
		DOTween.To (()=>rect.anchorMin, m=>rect.anchorMin = m, min, time).SetDelay(.2f);
	}
}
