using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatePowerUp : PowerupBase
{
	protected PieceManager pieceManager;
	protected GameManager gameManager;
	public Vector3 offsetPositionOverFinger = new Vector3(0,1.5f,0);
	public Vector3 selectedScale = new Vector3 (4.5f, 4.5f, 4.5f);
	public bool allowInput = true;

	public delegate void DOnDragNotification(GameObject target);
	public DOnDragNotification OnDropPieceRotated;
	public DOnDragNotification OnDragStartPieceRotated;

	public delegate bool DOnFingerNotification(GameObject target);
	public DOnFingerNotification OnDown;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;
	protected Vector3 selectedInitialScale   = new Vector3(2.5f,2.5f,2.5f);
	protected Vector3 selectedInitialPosition;

	public float pieceSpeed = 0.3f;

	protected bool allowInputDuringRotate = true;

	void Start()
	{
		pieceManager = FindObjectOfType<PieceManager> ();
		gameManager = FindObjectOfType<GameManager> ();
		pieceManager.OnRotate += isRotating;

		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		pieceManager.activateRotation (true);
		this.gameObject.SetActive( true);
	}

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (multifinger puede llamarlo mas de una vez)
		if(!allowInput || lastTimeDraggedFrame == Time.frameCount || !allowInputDuringRotate)
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

					if(OnDragStartPieceRotated != null)
					{
						OnDragStartPieceRotated(currentSelected);
					}

					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = selectedInitialPosition.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
					currentSelected.transform.DOScale(selectedScale,.1f).SetId("InputRotate_SelectedScale");
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
					moveTo(currentSelected,posOverFinger,pieceSpeed);
				}
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if(currentSelected)
				{
					if (gameManager.dropPieceOnGrid (currentSelected.GetComponent<Piece> ())) 
					{
						pieceManager.setRotationPiecesAsNormalRotation ();
						completePowerUp ();
					}
					else
					{
						returnSelectedToInitialState();
						reset();
					}

					DOTween.Kill("InputRotate_Dragging",false);
				}
			}
			break;
		}
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if (allowInput && gesture.Raycast.Hits2D != null) 
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;

			DOTween.Kill ("InputRotate_InitialPosition", true);
			DOTween.Kill ("InputRotate_ScalePosition", true);

			selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;

			Vector3 overFingerPosition = Camera.main.ScreenToWorldPoint (new Vector3 (gesture.Position.x, gesture.Position.y, 0));
			overFingerPosition.z = selectedInitialPosition.z;
			overFingerPosition += offsetPositionOverFinger;

			//currentSelected.transform.DOMove(overFingerPosition,.1f).SetId("Input_SelectedPosition");
		}
		else if(!allowInput)
		{
			allowInputDuringRotate = false;
		}
	}

	void OnFingerUp()
	{
		if(!somethingDragged && currentSelected != null)
		{				
			pieceManager.setRotatePiece (currentSelected);
			reset ();
			//returnSelectedToInitialState (0.1f);
		}
		else if(!somethingDragged && allowInputDuringRotate)
		{
			completePowerUp ();
		}
		allowInputDuringRotate = true;

		somethingDragged = false;
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("InputRotate_SelectedPosition",true);
		DOTween.Kill("InputRotate_SelectedScale",true);

		if(delay == 0)
		{
			print (selectedInitialPosition);
			print (selectedInitialScale);

			currentSelected.transform.position = selectedInitialPosition;
			currentSelected.transform.localScale = selectedInitialScale;
		}
		else
		{
			currentSelected.transform.DOMove (selectedInitialPosition, .1f).SetId("InputRotate_InitialPosition");
			currentSelected.transform.DOScale (selectedInitialScale, .1f).SetId("InputRotate_ScalePosition");
		}
	}

	public void reset()
	{
		currentSelected = null;
		somethingDragged = false;
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputRotate_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputRotate_Dragging");
	}

	protected void isRotating(bool isRotating)
	{
		allowInput = !isRotating;
	}

	protected void completePowerUp()
	{
		DOTween.Kill ("InputRotate_Dragging");
		pieceManager.activateRotation (false);
		this.gameObject.SetActive( false);

		if (pieceManager.checkIfExistRotatedPiezes ()) 
		{
			OnComplete ();	
		}
		else
		{
			OnCancel ();
		}
	}
}

