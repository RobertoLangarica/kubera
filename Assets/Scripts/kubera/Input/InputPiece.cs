using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputPiece : MonoBehaviour 
{
	public Vector3 offsetPositionOverFinger = new Vector3(0,1.5f,0);
	public Vector3 selectedScale = new Vector3 (4.5f, 4.5f, 4.5f);
	public bool allowInput = true;

	public delegate void DOnDragNotification(GameObject target);
	public delegate void DOnPlayer(bool onPlayer);
	public DOnDragNotification OnDrop;
	public DOnDragNotification OnDragStart;
	public DOnPlayer OnPlayer;

	public delegate void DOnSelectedNotification(GameObject target,bool selected);
	public DOnSelectedNotification OnSelected;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	public GameObject currentSelected = null;
	protected Vector3 selectedInitialScale;
	protected Vector3 selectedInitialPosition;

	public float pieceSpeed = 0.3f;

	protected bool isLongPressed = false;

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
					posOverFinger.z = -1;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);

					DOTween.Kill("Input_SelectedScale",false);

					currentSelected.transform.DOScale(selectedScale,.1f).SetId("Input_SelectedScale");
					OnSelected (currentSelected,true);
					isOnPlayer (true);
				}
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if(currentSelected != null)
				{
					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = -1;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
				}
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if(currentSelected)
				{
					changePositionZ (currentSelected,selectedInitialPosition.z);

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

	void changePositionZ(GameObject go,float z)
	{
		go.transform.position = new Vector3 (go.transform.position.x, go.transform.position.y, z);
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(currentSelected == false && allowInput && gesture.Raycast.Hits2D != null)
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;
			offsetPositionOverFinger.y = Mathf.Round ((gesture.Raycast.Hit2D.collider.bounds.size.y - 0.15f) * 15) * .10f;

			/*selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;*/
		}
	}

	void OnLongPress(LongPressGesture gesture)
	{
		if (allowInput && gesture.Raycast.Hits2D != null) 
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;
			//DOTween.Kill("Input_InitialPosition",true);
			//DOTween.Kill("Input_ScalePosition",true);

			selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;

			Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
			posOverFinger.z = -1;
			posOverFinger += offsetPositionOverFinger;

			moveTo(currentSelected,posOverFinger,pieceSpeed); 
			OnSelected (currentSelected,true);
			currentSelected.transform.DOScale(selectedScale,.075f).SetId("Input_SelectedScale").OnComplete(()=>{});

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("pieceSelected");
				AudioManager.GetInstance().Play("pieceSelected");
			}

			/*currentSelected.transform.DOScale (currentSelected.transform.localScale * 0.8f, 0.075f).SetId(currentSelected).OnComplete (()=>
				{
					currentSelected.transform.DOScale(selectedScale,.075f).SetId("Input_SelectedScale").OnComplete(()=>{});
				});*/


			isLongPressed = true;
		}
	}

	void OnFingerUp()
	{
		if(!somethingDragged && currentSelected != null && isLongPressed)
		{		
			DOTween.Kill (currentSelected);
			returnSelectedToInitialState(0.1f);
			reset();
		}

		isLongPressed = false;

		somethingDragged = false;
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("Input_InitialPosition",false);
		DOTween.Kill("Input_SelectedScale",false);
		DOTween.Kill("Input_Dragging",false);

		Piece piece = currentSelected.GetComponent<Piece> ();

		if(delay == 0)
		{
			currentSelected.transform.position = piece.positionOnScene;
			currentSelected.transform.localScale = piece.initialPieceScale;
		}
		else
		{
			allowInput = false;
			currentSelected.transform.DOScale (piece.initialPieceScale, delay).SetId("Input_ScalePosition");
			currentSelected.transform.DOMove (piece.positionOnScene, delay).SetId("Input_InitialPosition").OnComplete(()=>{allowInput = true; });
		}
		OnSelected (currentSelected,false);
	}

	public void reset()
	{
		currentSelected = null;
		somethingDragged = false;
		isOnPlayer (false);
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("Input_Dragging",false);
		target.transform.DOMove (to, delay).SetId("Input_Dragging");
	}

	protected void isOnPlayer(bool isOn)
	{
		if(OnPlayer != null)
		{
			OnPlayer (isOn);
		}
	}
}
