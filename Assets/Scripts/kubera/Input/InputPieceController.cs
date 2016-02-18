using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputPieceController : MonoBehaviour 
{
	public Vector3 offsetPositionOverFinger = new Vector3(0,1.5f,0);
	public Vector3 selectedScale = new Vector3 (4.5f, 4.5f, 4.5f);
	public bool allowInput = true;

	public delegate void DOnDragNotification(GameObject target);
	public DOnDragNotification OnDrop;
	public DOnDragNotification OnDragStart;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;
	protected Vector3 selectedInitialScale;
	protected Vector3 selectedInitialPosition;

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

					if(OnDragStart != null)
					{
						OnDragStart(currentSelected);
					}

					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = selectedInitialPosition.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,0.1f);
				}
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if(currentSelected != null)
				{
					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = selectedInitialPosition.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,0.1f);
				}
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if(currentSelected)
				{
					if(OnDrop != null)
					{
						OnDrop(currentSelected);	
					}

					DOTween.Kill("Input_Dragging",false);


					//Para un autoreset hay que descomentar las siguientes lineas
					//returnSelectedToInitialState();
					//reset();
				}
			}
			break;
		}
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(allowInput && gesture.Raycast.Hits2D != null)
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;

			DOTween.Kill("Input_InitialPosition",true);
			DOTween.Kill("Input_ScalePosition",true);

			selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;


			Vector3 overFingerPosition = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
			overFingerPosition.z = selectedInitialPosition.z;
			overFingerPosition += offsetPositionOverFinger;

			currentSelected.transform.DOMove(overFingerPosition,.1f).SetId("Input_SelectedPosition");
			currentSelected.transform.DOScale(selectedScale,.1f).SetId("Input_SelectedScale");
		}
	}

	void OnFingerUp()
	{
		if(!somethingDragged && currentSelected != null)
		{				
			returnSelectedToInitialState(0.1f);
			reset();
		}

		somethingDragged = false;
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("Input_SelectedPosition",true);
		DOTween.Kill("Input_SelectedScale",true);

		if(delay == 0)
		{
			currentSelected.transform.position = selectedInitialPosition;
			currentSelected.transform.localScale = selectedInitialScale;
		}
		else
		{
			currentSelected.transform.DOMove (selectedInitialPosition, .1f).SetId("Input_InitialPosition");
			currentSelected.transform.DOScale (selectedInitialScale, .1f).SetId("Input_ScalePosition");
		}
	}

	public void reset()
	{
		currentSelected = null;
		somethingDragged = false;
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("Input_Dragging",false);
		target.transform.DOMove (to, delay).SetId("Input_Dragging");
	}
}
