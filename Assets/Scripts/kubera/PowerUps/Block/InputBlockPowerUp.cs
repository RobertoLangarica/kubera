using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputBlockPowerUp : MonoBehaviour
{
	Vector3 butonPowerUpBlockPosition;

	public Vector3 offsetPositionOverFinger = new Vector3(0,0,0);
	public Vector3 initialScale = new Vector3(4,4,4);
	public float pieceSpeed = 0.3f;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;

	protected GameManager gameManager;

	public delegate void DPowerUpBlockNotification();
	public DPowerUpBlockNotification OnPowerupCanceled;
	public DPowerUpBlockNotification OnPowerupCompleted;
	public DPowerUpBlockNotification OnPowerupCompletedNoGems;
	public delegate void DOnPlayer(bool onPlayer);
	public DOnPlayer OnPlayer;

	protected bool canUse;
	public Transform parent;

	void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		pieceSpeed = FindObjectOfType<InputPiece> ().pieceSpeed;

	}

	public void createBlock(GameObject block, Vector3 bottonPosition,bool canUse)
	{
		GameObject blockGO = Instantiate (block,bottonPosition,Quaternion.identity) as GameObject;
		if (currentSelected) 
		{
			DestroyImmediate (currentSelected);
		}

		butonPowerUpBlockPosition = bottonPosition;
		butonPowerUpBlockPosition.z = 0;
		currentSelected = blockGO;
		blockGO.transform.localScale = initialScale;
		moveTo(currentSelected,butonPowerUpBlockPosition,pieceSpeed);
		blockGO.name = "Block";
		blockGO.transform.SetParent (parent, false);
		blockGO.GetComponentInChildren<Square> ().oneSquare = false;
		blockGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";
		this.canUse = canUse;
	}

	public void setPieceColor()
	{
		currentSelected.GetComponent<Piece> ().currentColor = Piece.EColor.RED;
	}

	public GameObject getCurrentSelected()
	{
		return currentSelected;
	}

	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (multifinger puede llamarlo mas de una vez)
		if(lastTimeDraggedFrame == Time.frameCount)
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
					if(!canUse && gameManager.canDropOnGrid (currentSelected.GetComponent<Piece> ()))
					{
						returnSelectedToInitialState(0.2f);
						completePowerUpNoGems ();
					}
					else if (gameManager.dropOnGrid (currentSelected.GetComponent<Piece> ())) 
					{
						reset();
						completePowerUp (true);
					}
					else
					{
						returnSelectedToInitialState(0.2f);
						completePowerUp (false);
					}

					DOTween.Kill("InputBlock_Dragging",false);
				}
			}
			break;
		}
	}

	void OnFingerUp()
	{
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
			currentSelected.transform.position = butonPowerUpBlockPosition;
		}
		else
		{
			currentSelected.transform.DOMove (butonPowerUpBlockPosition, delay).SetId("InputBlock_InitialPosition");
			currentSelected.transform.DOScale (new Vector3(0,0,0), delay).SetId("InputBlock_ScalePosition").OnComplete (() => {
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
}

