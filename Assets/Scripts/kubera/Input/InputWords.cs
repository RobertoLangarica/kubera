using UnityEngine;
using System.Collections;
using DG.Tweening;
using ABC;

public class InputWords : MonoBehaviour 
{
	protected int lastDragFrame;
	protected GameObject letter;

	//Para notificar estados del drag a otros objetos
	public delegate void DInputWordNotification(GameObject letter);
	[HideInInspector]
	public DInputWordNotification onDragFinish;
	[HideInInspector]
	public DInputWordNotification onDragUpdate;
	[HideInInspector]
	public DInputWordNotification onDragStart;
	[HideInInspector]
	public DInputWordNotification onTap;

	// 0 nada 1 con uicharFromGrid,  2 conuiCharSinFromGrid
	public delegate int DCheckIfObjectExist(GameObject Go);
	[HideInInspector]
	public DCheckIfObjectExist checkIfObjectExist;

	public float velocityOfMovmentOfTheLetter = 0.8f;
	public bool stopInput;

	/**
	 * chequeo del drag
	 **/
	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (para que el multifinger funcione sin encimarse)
		if(lastDragFrame == Time.frameCount)
		{
			return;
		}

		lastDragFrame = Time.frameCount;


		switch(gesture.Phase)
		{
		case (ContinuousGesturePhase.Started):
			{	
				if (!gesture.Raycast.Hit2D && !stopInput) {
					return;
				}

				if(checkIfObjectExist(gesture.Raycast.Hit2D.transform.gameObject) == 2)
				{
					letter = gesture.Raycast.Hit2D.transform.gameObject;
				}
				
				if (!letter) 
				{
					return;
				}

				onDragStart(letter);
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				
				if (!letter && !stopInput ) 
				{
					return;
				}

				// si existe la pieza la movemos con movingLerping de acuerdo a la posicion del mouse

				float y = letter.transform.position.y;
				float z = letter.transform.position.z;
				Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
				tempV3.z = -1;

				tempV3.y = y;
				tempV3.z = z;
				movingLerping(tempV3,letter);

				onDragUpdate (letter);							
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				//si no hay pieza terminamos
				if (!letter && !stopInput ) 
				{
					return;
				}
					
				onDragFinish(letter);

				letter = null;

				DOTween.Kill ("MovingPiece");
			}
			break;
		}
	}

	/**
	 * tap de la letra en el tablero
	 **/
	void OnTap(TapGesture gesture)
	{
		if(gesture.Raycast.Hit2D && !stopInput)
		{				
			if (checkIfObjectExist(gesture.Raycast.Hit2D.transform.gameObject) == 1) 
			{
				onTap (gesture.Raycast.Hit2D.transform.gameObject);
					//wordManager.activateButtonOfWordsActions (true);
					//gameObject.GetComponent<AudioSource> ().Play ();
			}
		}
	}

	/**
	 * Movimiento y velocidad de la letra
	 **/
	public void movingLerping(Vector3 end,GameObject piece)
	{		
		piece.transform.DOMove (end, velocityOfMovmentOfTheLetter).SetId("MovingPiece");
	}

}