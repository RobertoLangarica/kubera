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
	public DInputWordNotification onTapToDelete;

	public float letterSpeed = 0.5f;
	public bool allowInput = true;
	protected bool canDeleteLetter = true;

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (para que el multifinger funcione sin encimarse)
		if(!allowInput || lastTimeDraggedFrame == Time.frameCount)
		{
			return;
		}

		lastTimeDraggedFrame = Time.frameCount;

		canDeleteLetter = false;
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
				canDeleteLetter = true;

			}
			break;
		}
	}

	void OnLetterGridTap(TapGesture gesture)
	{
		if(allowInput && gesture.Raycast.Hit2D)
		{		
			onTap(gesture.Raycast.Hit2D.transform.gameObject);
		}
	}

	void OnLetterWordTap(TapGesture gesture)
	{
		if (allowInput && gesture.Raycast.Hit2D ) 
		{	
			onTapToDelete(gesture.Raycast.Hit2D.transform.gameObject);
		}
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputW_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputW_Dragging");
	}

}