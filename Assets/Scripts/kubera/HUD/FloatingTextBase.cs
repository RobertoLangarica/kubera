using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;

public class FloatingTextBase : MonoBehaviour 
{
	//public delegate void textAnimationEnded(FloatingTextBase text);

	/*Valores de tuneo para el movimiento del texto*/
	public float steps;
	public float lerpTime;

	public bool returnToInitialPosition;
	public bool goToZeroAlphaOnStart;

	public Text myText;

	public Action OnEnded;

	protected float lerpPercent;
	protected bool animate = false;
	protected Vector3 startPosition;
	protected Vector3 finishPosition;

	protected float elapsedTime;
	protected float stepValue;
	protected float stepTime;

	protected virtual void Start () 
	{
		//Se calculan los valores de la animacion
		stepValue = 1.0f / steps;
		stepTime = lerpTime / steps;
	}

	void Update () 
	{
		if(animate)
		{
			elapsedTime += Time.deltaTime;
			if(lerpPercent < 1)
			{
				if(elapsedTime >= stepTime)
				{
					elapsedTime = 0;
					lerpPercent += stepValue;
					transform.position = Vector3.Lerp(startPosition,finishPosition,lerpPercent);
				}
			}
			else
			{
				lerpPercent = 0;
				animate = false;
				gameObject.SetActive (false);
				if (OnEnded != null) 
				{
					OnEnded ();
				}

				if (returnToInitialPosition) 
				{
					transform.position = startPosition;
					transform.localScale = Vector3.zero;
				}
			}
		}
	}

	/*
	 * La funcion mueve el texto al centro del edificiio, desde donde se inicia su animacion.
	 * 
	 * @params start{Vector3}: El punto de inicio de la animacion, el texto se mueve en un inicio a esta posicion
	 * 
	 * @params end{Vector3}: El punto en el que se termina la animacion, y se libera el texto para ser reutilizado
	 */
	public virtual void startAnim(Vector3 start,Vector3 end)
	{
		gameObject.SetActive (true);
		transform.position = start;
		startPosition = start;
		finishPosition = end;
		animate = true;

		if (goToZeroAlphaOnStart) 
		{
			goToAlphaZero ();
		}
	}

	public void goToAlphaZero()
	{
		Color temp = myText.color;

		myText.DOColor (new Color(temp.r,temp.g,temp.b,0),0.6f).OnComplete(
			()=>{myText.color = temp;});
	}
}
