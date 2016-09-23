using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputWildcardPowerUp : MonoBehaviour {

	Vector3 butonPowerUpPosition;

	public Vector3 offsetPositionOverFinger = new Vector3(0,0,0);
	public Vector3 initialScale = new Vector3(4,4,4);
	public float pieceSpeed = 0.3f;
	public float delaySpeed = 0.2f;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	public GameObject currentSelected = null;

	protected GameManager gameManager;
	protected CellsManager cellsManager;
	protected WordManager wordManager;
	protected KeyBoardManager keyBoardManager;

	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;
	public DPowerUpNotification OnPowerupCompletedNoGems;

	public delegate void DOnPlayer(bool onPlayer);
	public DOnPlayer OnPlayer;

	protected bool canUse;
	public Transform parent;

	void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		pieceSpeed = FindObjectOfType<InputPiece> ().pieceSpeed;
		cellsManager = FindObjectOfType<CellsManager> ();
		wordManager = FindObjectOfType<WordManager> ();
		keyBoardManager = FindObjectOfType<KeyBoardManager> ();
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
					Cell cell = cellsManager.getCellUnderPoint (currentSelected.transform.position);

					if(!canUse)
					{
						returnSelectedToInitialState(delaySpeed);
						completePowerUpNoGems ();
					}
					else if (cell != null && cell.cellType != Cell.EType.EMPTY) 
					{
						insertWildcard (cellsManager.getCellUnderPoint (currentSelected.transform.position));
						completePowerUp (true);
						destroySelected ();
					}
					else
					{
						returnSelectedToInitialState(delaySpeed);
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
}