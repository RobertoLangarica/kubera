using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputBombAndDestroy : MonoBehaviour 
{
	public Vector3 offsetPositionOverFinger = new Vector3(0,0.3f,0);
	public bool allowInput = true;

	public delegate void DOnDragNotification();
	public DOnDragNotification OnDrop;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;

	public float pieceSpeed = 0.3f;

	void Start()
	{
		enabled = false;
		pieceSpeed = FindObjectOfType<InputPiece> ().pieceSpeed;
	}

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (multifinger puede llamarlo mas de una vez)
		if(!allowInput || lastTimeDraggedFrame == Time.frameCount)
		{
			return;
		}

		lastTimeDraggedFrame = Time.frameCount;

		switch(gesture.Phase)
		{
		case (ContinuousGesturePhase.Started):
			{
				if(currentSelected != null)
				{
					somethingDragged = true;

					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = currentSelected.transform.position.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
				}
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if(currentSelected != null)
				{
					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = currentSelected.transform.position.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
				}
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if(currentSelected)
				{
					if(OnDrop != null)
					{
						OnDrop();	
					}
					reset();
				}
			}
			break;
		}
	}

	void OnFingerUp()
	{
		if(!somethingDragged && currentSelected != null)
		{
			if(OnDrop != null)
			{
				OnDrop();	
			}

			reset();
		}

		somethingDragged = false;
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputBombDestroy_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputBombDestroy_Dragging");
	}

	public void reset()
	{
		currentSelected = null;
		somethingDragged = false;
	}

	public void setCurrentSelected(GameObject selected)
	{
		currentSelected = selected;
	}
}