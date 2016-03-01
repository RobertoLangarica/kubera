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

	protected bool somethingDragged = false;

	public DInputWordNotification onDragFinish;
	public DInputWordNotification onDragUpdate;
	public DInputWordNotification onDragStart;
	public DInputWordNotification onTap;
	public DInputWordNotification onFingerUpAfterDragEnd;

	public float letterSpeed = 0.5f;
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

				somethingDragged = true;
				letter = gesture.Raycast.Hit2D.transform.gameObject;

				onDragStart(letter);
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if (!letter) 
				{return;}
					
				Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
				tempV3.y = letter.transform.position.y;
				tempV3.z = letter.transform.position.z;
				moveTo(letter,tempV3,letterSpeed);

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

				DOTween.Kill ("InputW_Dragging");
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
			//gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

	void OnFingerUp(FingerUpEvent fingerUpEvent)
	{
		if (fingerUpEvent.Raycast.Hit2D) 
		{
			if (!somethingDragged) 
			{				
				letter = fingerUpEvent.Raycast.Hit2D.transform.gameObject;
				print ("OnFingerUp");
				onFingerUpAfterDragEnd (letter);
				letter = null;
			}
		}
		somethingDragged = false;
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputW_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputW_Dragging");
	}

}