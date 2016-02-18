using UnityEngine;
using System.Collections;
using DG.Tweening;
using ABC;

public class InputWords : MonoBehaviour 
{
	protected int lastTimeDraggedFrame;
	protected GameObject letter;

	//Para notificar estados del drag a otros objetos
	public delegate void DInputWordNotification(GameObject letter);

	public DInputWordNotification onDragFinish;
	public DInputWordNotification onDragUpdate;
	public DInputWordNotification onDragStart;
	public DInputWordNotification onTap;

	/**
	 * @return 0:nada,  1:uicharFromGrid,  2:conuiCharSinFromGrid
	 */ 
	public delegate int DCheckIfObjectExist(GameObject Go);

	public DCheckIfObjectExist checkIfObjectExist;
	public float letterSpeed = 0.8f;
	public bool allowInput = true;

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (para que el multifinger funcione sin encimarse)
		if(!allowInput || lastTimeDraggedFrame == Time.frameCount)
		{
			return;
		}

		lastTimeDraggedFrame = Time.frameCount;


		switch(gesture.Phase)
		{
		case (ContinuousGesturePhase.Started):
			{	
				if (!gesture.Raycast.Hit2D) 
				{
					return;
				}


				letter = gesture.Raycast.Hit2D.transform.gameObject;

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
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
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
	 * tap de la letra en el tablero
	 **/
	void OnTap(TapGesture gesture)
	{
		if(allowInput && gesture.Raycast.Hit2D)
		{		
			onTap (gesture.Raycast.Hit2D.transform.gameObject);
			//[TODO]
			//wordManager.activateButtonOfWordsActions (true);
			//gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	/**
	 * Movimiento y velocidad de la letra
	 **/
	public void movingLerping(Vector3 end,GameObject piece)
	{		
		piece.transform.DOMove (end, letterSpeed).SetId("MovingPiece");
	}

}