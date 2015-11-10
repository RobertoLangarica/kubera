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

	public float INDuration;
	public float INDelay;
	public Ease EaseIN = Ease.Linear;
	public float OUTDuration;
	public float OUTDelay;
	public Ease EaseOUT = Ease.Linear;
	public EOutPosition outPosX;
	public EOutPosition outPosY;
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
		switch(outPosY)
		{
		case EOutPosition.BOTTOM:
			outMaxPos.y-= 1;
			outMinPos.y-= 1;
			break;
		case EOutPosition.TOP:
			outMaxPos.y+= 1;
			outMinPos.y+= 1;
			break;
		}

		//Calculamos las posiciones de fuera
		switch(outPosX)
		{
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
		animate(outMaxPos,outMinPos,OUTDuration,OUTDelay,EaseOUT);
	}

	public void In()
	{
		animate(anchoredMaxPos,anchoredMinPos,INDuration,INDelay,EaseIN);
	}

	public void animate(Vector2 max, Vector2 min, float time, float delay = 0,Ease ease = Ease.Linear)
	{
		DOTween.To (()=>rect.anchorMax, m=>rect.anchorMax = m, max, time).SetDelay(delay).SetEase(ease);
		DOTween.To (()=>rect.anchorMin, m=>rect.anchorMin = m, min, time).SetDelay(delay).SetEase(ease);
	}
}
