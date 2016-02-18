using UnityEngine;
using System.Collections;
using DG.Tweening;
using ABC;

public class InputWords : MonoBehaviour 
{
	protected int lastDragFrame;
	protected GameObject letter;

	//Para notificar estados del drag a otros objetos
	public delegate void DDragWordNotification(GameObject letter);
	[HideInInspector]
	public DDragWordNotification onDragFinish;
	[HideInInspector]
	public DDragWordNotification onDragUpdate;
	[HideInInspector]
	public DDragWordNotification onDragStart;

	public float velocityOfMovmentOfTheLetter = 0.8f;

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
				if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<UIChar> ())
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
				
				if (!letter) 
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
				//wordManager.swappingLetters(letter);
							
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				//si no hay pieza terminamos
				if (!letter) 
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
	 * Movimiento y velocidad de la letra
	 **/
	public void movingLerping(Vector3 end,GameObject piece)
	{		
		piece.transform.DOMove (end, velocityOfMovmentOfTheLetter).SetId("MovingPiece");
	}

}