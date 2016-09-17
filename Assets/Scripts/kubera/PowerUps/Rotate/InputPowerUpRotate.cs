﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class InputPowerUpRotate : MonoBehaviour
{
	protected PieceManager pieceManager;
	protected GameManager gameManager;
	protected HUDManager hudManager;

	public Vector3 offsetPositionOverFinger = new Vector3(0,1.5f,0);
	public Vector3 selectedScale = new Vector3 (4.5f, 4.5f, 4.5f);
	public bool allowInput = true;
	public Transform pieceStock;

	public delegate void DOnDragNotification(GameObject target);
	public DOnDragNotification OnRotateArrowsActivated;
	public DOnDragNotification OnDragStartPieceRotated;

	public delegate void DPowerUpRotateNotification();
	public DPowerUpRotateNotification OnPowerupRotateCompleted;
	public DPowerUpRotateNotification OnPieceRotated;

	public delegate bool DOnFingerNotification(GameObject target);
	public DOnFingerNotification OnDown;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;

	public Vector3 selectedInitialScale   = new Vector3(2.5f,2.5f,2.5f);
	public float rotateSpeed = 0.25f;

	protected Vector3 selectedInitialPosition;

	public float pieceSpeed = 0.3f;

	protected bool allowInputDuringRotate = true;
	protected bool isLongPressed = false;

	void Start()
	{
		pieceManager = FindObjectOfType<PieceManager> ();
		gameManager = FindObjectOfType<GameManager> ();
		hudManager = FindObjectOfType<HUDManager> ();
		pieceSpeed = FindObjectOfType<InputPiece> ().pieceSpeed;
		if(pieceStock == null)
		{
			print("Falta asignarlo en el editor");
		}
	}

	public void startRotate()
	{
		if(pieceManager == null ||gameManager == null|| hudManager == null)
		{
			pieceManager = FindObjectOfType<PieceManager> ();
			gameManager = FindObjectOfType<GameManager> ();
			hudManager = FindObjectOfType<HUDManager> ();
		}
	
		activateRotateImage (true);
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
					gameManager.showShadowOnPiece (currentSelected, true);
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
					if(!gameManager.canDropOnGrid(currentSelected.GetComponent<Piece>()))
					{
						returnSelectedToInitialState();
						reset();	
					}
					else if(gameManager.dropOnGrid (currentSelected.GetComponent<Piece> ())) 
					{
						onPiecePositionatedCompleted (false,currentSelected.GetComponent<Piece> ().createdIndex);
						reset();
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
		if (!currentSelected && allowInput && gesture.Raycast.Hits2D != null) 
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;
			offsetPositionOverFinger.y = Mathf.Round ((gesture.Raycast.Hit2D.collider.bounds.size.y - 0.15f) * 15) * .10f;
		//currentSelected.transform.DOMove(overFingerPosition,.1f).SetId("Input_SelectedPosition");

		}
		else if(!allowInput)
		{
			allowInputDuringRotate = false;
		}
	}

	void OnLongPress(LongPressGesture gesture)
	{
		if (allowInput && currentSelected != null) 
		{
			/*DOTween.Kill ("InputRotate_InitialPosition", true);
			DOTween.Kill ("InputRotate_SelectedScale", true);*/
			/*selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;*/

			Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
			posOverFinger.z = selectedInitialPosition.z;
			posOverFinger += offsetPositionOverFinger;

			moveTo(currentSelected,posOverFinger,pieceSpeed);
			currentSelected.transform.DOScale(selectedScale,.1f).SetId("InputRotate_SelectedScale");
			gameManager.showShadowOnPiece (currentSelected, true);

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("pieceSelected");
				AudioManager.GetInstance().Play("pieceSelected");
			}

			isLongPressed = true;
		}
	}

	void OnFingerUp()
	{
		if(!somethingDragged && currentSelected != null && !isLongPressed)
		{				
			RotatePiece (currentSelected);
			reset ();
			//returnSelectedToInitialState (0.1f);
		}
		else if(!somethingDragged && currentSelected != null && isLongPressed)
		{
			returnSelectedToInitialState(0.1f);
			reset();
		}

		isLongPressed = false;
		allowInputDuringRotate = true;
		somethingDragged = false;
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("InputRotate_InitialPosition",false);
		DOTween.Kill("InputRotate_SelectedScale",false);
		DOTween.Kill("InputRotate_Dragging",false);

		Piece piece = currentSelected.GetComponent<Piece> ();

		if(delay == 0)
		{
			currentSelected.transform.position = piece.positionOnScene;
			currentSelected.transform.localScale = piece.initialPieceScale;
		}
		else
		{
			currentSelected.transform.DOMove (piece.positionOnScene, .1f).SetId("InputRotate_InitialPosition").OnComplete(()=>{allowInput = true; });;
			currentSelected.transform.DOScale (piece.initialPieceScale, .1f).SetId("InputRotate_SelectedScale");
		}
		gameManager.showShadowOnPiece (currentSelected, false);

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

	protected void onPiecePositionatedCompleted (bool cancel, int posOnScene)
	{
		DOTween.Kill ("InputRotate_Dragging");

		activateRotateImage (cancel,posOnScene);

		// por los delay cuando es 1 es cuando solo queda la pieza que se esta poniendo
		if (pieceManager.getShowingPieces ().Count == 1) 
		{
			OnPowerupRotateCompleted ();
		}
	}

	public void RotatePiece(GameObject go)
	{
		if (go.GetComponent<Piece> ()) 
		{
			RotatePiece(go.GetComponent<Piece>());
		}
	}

	//Se rotan las piezas
	public void RotatePiece(Piece piece)
	{	
		if (!allowInput)
		{
			return;
		}

		if (piece.toRotateObject != null) 
		{
			isRotating (true);

			StartCoroutine( changePiece (piece,piece.transform.position, rotateSpeed));

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("rotate");
				AudioManager.GetInstance().Play("rotate");
			}

			piece.gameObject.transform.DOScale (new Vector3 (0, 0), rotateSpeed).OnComplete (() => {

				/*piece.transform.localRotation = Quaternion.Euler(new Vector3(0,0,piece.transform.rotation.eulerAngles.z+90));

				piece.transform.DOScale (selectedInitialScale, 0.25f).OnComplete (() => {
					isRotating(false);
				});*/
				
				if(OnPieceRotated != null)
				{
					OnPieceRotated();
				}

				DestroyImmediate(piece.gameObject);
			});
		}
	}

	IEnumerator changePiece (Piece piece, Vector3 position, float delay =0)
	{
		yield return new WaitForSeconds(delay);
		GameObject go = GameObject.Instantiate (piece.toRotateObject) as GameObject;
		Piece newPiece = go.GetComponent<Piece> ();
		initializeNewPiece (piece, newPiece);
		go.transform.SetParent (hudManager.showingPiecesContainer);
		pieceManager.showingPieces.Remove (piece);
		pieceManager.showingPieces.Add (newPiece);

		go.transform.localScale = new Vector3 (0, 0, 0);
		go.transform.position = position;
		go.SetActive (true);
		go.transform.DOScale (selectedInitialScale, 0.25f).OnComplete (() => {
			isRotating (false);
		});
	}

	protected void initializeNewPiece (Piece oldPiece, Piece piece)
	{
		piece.createdIndex = oldPiece.createdIndex;
		piece.positionOnScene = oldPiece.positionOnScene;
		piece.initialPieceScale = oldPiece.initialPieceScale;
		piece.switchGreyPiece (oldPiece.isGrey());
	}

	public void activateRotateImage(bool activate,int posOnScene =-1)
	{
		if(activate)
		{
			for (int j = 0; j < pieceManager.showingPieces.Count; j++) 
			{				
				hudManager.activateRotateImage (true, pieceManager.showingPieces[j].GetComponent<Piece>().createdIndex);

				if (OnRotateArrowsActivated != null) 
				{
					OnRotateArrowsActivated (currentSelected);
				}
			}
		}
		else
		{
			hudManager.activateRotateImage (false, posOnScene);
		}
	}
}


