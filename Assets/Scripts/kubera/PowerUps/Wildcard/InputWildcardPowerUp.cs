﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputWildcardPowerUp : MonoBehaviour {

	Vector3 butonPowerUpPosition;

	public Vector3 offsetPositionOverFinger = new Vector3(0,0,0);
	public Vector3 initialScale = new Vector3(4,4,4);
	public float pieceSpeed = 0.3f;
	public float delaySpeed = 0.2f;
	public GameObject[] rayCasters;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	public GameObject currentSelected = null;

	public GameManager gameManager;
	public CellsManager cellsManager;
	public WordManager wordManager;
	public KeyBoardManager keyBoardManager;
	public InputPiece inputPiece;

	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;
	public DPowerUpNotification OnPowerupCompletedNoGems;
	public DPowerUpNotification OnPowerupOverObstacleLetter;

	public delegate void DOnPlayer(bool onPlayer);
	public DOnPlayer OnPlayer;

	protected bool canUse;
	public Transform parent;

	protected bool isLongPressed = false;

	void Start()
	{
		pieceSpeed = inputPiece.pieceSpeed;

		if(rayCasters != null)
		{
			InputBase.registerRayCasters(rayCasters);
		}
	}

	void OnDestroy()
	{
		InputBase.clearRaycasters();
	}

	public void createBlock(GameObject block, Vector3 bottonPosition,bool canUse)
	{
		GameObject blockGO = Instantiate (block,bottonPosition,Quaternion.identity) as GameObject;
		if (currentSelected) 
		{
			DestroyImmediate (currentSelected);
		}

		butonPowerUpPosition = bottonPosition;
		butonPowerUpPosition.z = 0;
		currentSelected = blockGO;
		blockGO.transform.localScale = initialScale;
		moveTo(currentSelected,butonPowerUpPosition,pieceSpeed);
		blockGO.name = "powerUPBlock";
		blockGO.transform.SetParent (parent, false);

		blockGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";

		this.canUse = canUse;
	}

	public GameObject getCurrentSelected()
	{
		return currentSelected;
	}

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (multifinger puede llamarlo mas de una vez)
		if(lastTimeDraggedFrame == Time.frameCount || !enabled)
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
					activateRayCasters(false);
					somethingDragged = true;
					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = currentSelected.transform.position.z;

					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
					//currentSelected.transform.DOScale(selectedScale,.1f).SetId("InputBlock_SelectedScale");
					isOnPlayer (true);
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
					activateRayCasters(true);

					Cell cell = cellsManager.getCellUnderPoint (currentSelected.transform.position);


					if (cell != null) 
					{
						if (cell.cellType != Cell.EType.EMPTY && cell.cellType != Cell.EType.OBSTACLE_LETTER && !isWildCard(cell)) 
						{
							if (!canUse) 
							{
								returnSelectedToInitialState (delaySpeed);
								completePowerUpNoGems ();
							}
							else 
							{
								insertWildcard (cellsManager.getCellUnderPoint (currentSelected.transform.position));
								completePowerUp (true);
								destroySelected ();
							}
						}
						else
						{
							if (cell.cellType == Cell.EType.OBSTACLE_LETTER && OnPowerupOverObstacleLetter != null) 
							{
								OnPowerupOverObstacleLetter ();
							}

							returnSelectedToInitialState(delaySpeed);
							completePowerUp (false);
						}
					}
					else
					{
							if (cell.cellType == Cell.EType.OBSTACLE_LETTER && OnPowerupOverObstacleLetter != null) 
							{
								OnPowerupOverObstacleLetter ();
							}

						returnSelectedToInitialState(delaySpeed);
						completePowerUp (false);
					}

					DOTween.Kill("InputBlock_Dragging",false);
				}
			}
			break;
		}
	}

	bool isWildCard(Cell cell)
	{
		if(cell.content != null)
		{
			Letter lett = cell.content.GetComponent<Letter> ();
	
			if (lett != null) 
			{
				if (lett.wildCard) 
				{
					return true;
				}
			}
		}

		return false;
	}

	void activateRayCasters(bool activate)
	{
		InputBase.activateAllRayCasters(activate);
		/*for(int i = 0; i < rayCasters.Length; i++)
		{
			rayCasters[i].SetActive(activate);	
		}*/		
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(!enabled)
		{
			return;
		}
		
		if (!currentSelected && gesture.Raycast.Hits2D != null) 
		{
			currentSelected = gesture.Raycast.Hit2D.transform.gameObject;
			offsetPositionOverFinger.y = Mathf.Round ((gesture.Raycast.Hit2D.collider.bounds.size.y - 0.15f) * 15) * .10f;
			//currentSelected.transform.DOMove(overFingerPosition,.1f).SetId("Input_SelectedPosition");

		}
	}

	void OnFingerUp()
	{
		if(!enabled)
		{
			return;
		}

		if(!somethingDragged && currentSelected != null)
		{				
			returnSelectedToInitialState (0.2f);
			completePowerUp (false);
		}

		somethingDragged = false;
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputBlock_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputBlock_Dragging");
	}

	protected void insertWildcard(Cell cell)
	{
		Letter letter = wordManager.getNewEmptyGridLetter ();
		Letter contentLetter = null;

		wordManager.setValuesToWildCard (letter, ".");

		if(cell.content)
		{
			contentLetter = cell.content.GetComponent<Letter> ();

			if(contentLetter != null)
			{
				if(contentLetter.selected)
				{
					wordManager.removeLetter(contentLetter.letterReference);
					wordManager.arrangeSortingOrder ();
				}
				gameManager.getGridCharacters ().Remove(contentLetter);
			}
		}

		//cell.destroyCell ();

		keyBoardManager.setSelectedWildCard (letter);
		cellsManager.occupyAndConfigureCell (cell,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE,true);
		gameManager.getGridCharacters ().Add(letter);
	}

	public void reset()
	{
		currentSelected = null;
		isOnPlayer (false);
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("InputBlock_SelectedPosition",true);
		DOTween.Kill("InputBlock_SelectedScale",true);

		if(delay == 0)
		{
			currentSelected.transform.position = butonPowerUpPosition;
		}
		else
		{
			currentSelected.transform.DOMove (butonPowerUpPosition, delay).SetId("InputBlock_InitialPosition");
			currentSelected.transform.DOScale (new Vector3(0,0,0), delay).SetId("InputBlock_ScalePosition").OnComplete (() => {
				DestroyImmediate(currentSelected);
				reset();//Se manda reset despues de destruirla
			});
		}
	}

	public void destroySelected()
	{
		DOTween.Kill("InputBlock_SelectedPosition",true);
		DOTween.Kill("InputBlock_SelectedScale",true);

		{
			currentSelected.transform.DOScale (new Vector3(0,0,0), delaySpeed).SetId("InputBlock_ScalePosition").OnComplete (() => {
				DestroyImmediate(currentSelected);
				reset();//Se manda reset despues de destruirla
			});
		}
	}

	protected void completePowerUp(bool finish)
	{
		DOTween.Kill ("InputBlock_Dragging");
		if (finish) 
		{
			OnPowerupCompleted ();
		}
		else 
		{
			OnPowerupCanceled();
		}
	}
	protected void completePowerUpNoGems()
	{
		OnPowerupCompletedNoGems ();
	}

	protected void isOnPlayer(bool isOn)
	{
		if(OnPlayer != null)
		{
			OnPlayer (isOn);
		}
	}

	void OnLongPress(LongPressGesture gesture)
	{
		if(!enabled)
		{
			return;
		}

		if (currentSelected != null) 
		{
			/*DOTween.Kill ("InputRotate_InitialPosition", true);
			DOTween.Kill ("InputRotate_SelectedScale", true);*/
			/*selectedInitialPosition = currentSelected.transform.position;
			selectedInitialScale = currentSelected.transform.localScale;*/

			Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
			posOverFinger.z = 0;
			posOverFinger += offsetPositionOverFinger;

			moveTo(currentSelected,posOverFinger,pieceSpeed);
			currentSelected.transform.DOScale(initialScale,.1f);

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("pieceSelected");
				AudioManager.GetInstance().Play("pieceSelected");
			}

			isLongPressed = true;
		}
	}
}