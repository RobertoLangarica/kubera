using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * Clase creada para evitar el uso de Tweens en los textos del dinero que dan los edificios.
 * Este texto es manipulado por una pool que verifica el estado de los textos para saber si esta disponible o no.
 */
public class ScoreText : MonoBehaviour 
{
	/*Valores de tuneo para el movimiento del texto*/
	public float steps;
	public float lerpTime;

	public Text myText;

	protected ScoreTextPool pool;

	protected float lerpPercent;
	protected bool animate = false;
	protected Vector3 startPosition;
	protected Vector3 finishPosition;

	protected float elapsedTime;
	protected float stepValue;
	protected float stepTime;
	
	void Start()
	{
		//Se calculan los valores de la animacion
		stepValue = 1 / steps;
		stepTime = lerpTime / steps;

		if(pool == null)
		{
			pool = FindObjectOfType<ScoreTextPool>();
		}
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
				pool.deativateText(myText);
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
	public void startAnim(Vector3 start,Vector3 end)
	{
		if(pool == null)
		{
			pool = FindObjectOfType<ScoreTextPool>();
		}

		transform.position = start;
		startPosition = start;
		finishPosition = end;
		animate = true;
	}
}