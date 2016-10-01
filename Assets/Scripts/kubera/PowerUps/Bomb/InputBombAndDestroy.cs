using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputBombAndDestroy : MonoBehaviour 
{
	public Vector3 offsetPositionOverFinger = new Vector3(0,0.3f,0);
	public bool allowInput = true;
	public GameObject[] rayCasters;

	public delegate void DOnDragNotification();
	public delegate void DOnPlayer(bool onPlayer);
	public delegate void DOnCellNotification(Cell nCell);
	public DOnDragNotification OnDrop;
	public DOnPlayer OnPlayer;
	public DOnCellNotification OnCellSelected;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;

	protected CellsManager cellsManager;
	protected Piece.EColor selectedCellColor;

	public float pieceSpeed = 0.3f;

	void Start()
	{
		enabled = false;

		cellsManager = FindObjectOfType<CellsManager> ();
		pieceSpeed = FindObjectOfType<InputPiece> ().pieceSpeed;

		if(rayCasters != null)
		{
			InputBase.registerRayCasters(rayCasters);
		}
	}

	void OnDestroy()
	{
		InputBase.clearRaycasters();
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
					activateRayCasters(false);
					somethingDragged = true;

					Vector3 posOverFinger = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					posOverFinger.z = currentSelected.transform.position.z;
					posOverFinger += offsetPositionOverFinger;
					moveTo(currentSelected,posOverFinger,pieceSpeed);
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

					if (OnCellSelected != null) 
					{
						Cell cellSelected = cellsManager.getCellUnderPoint (currentSelected.transform.position);
						if (cellSelected != null) 
						{
							if (cellSelected.content != null) 
							{
								if (cellSelected.contentColor != Piece.EColor.LETTER_OBSTACLE &&
								   cellSelected.contentColor != Piece.EColor.NONE) 
								{
									//if (selectedCellColor != null) 
									if (selectedCellColor != Piece.EColor.NONE) 
									{
										if (selectedCellColor != cellSelected.contentColor) 
										{
											OnCellSelected (cellSelected);
										}
									} 
									else 
									{
										selectedCellColor = cellSelected.contentColor;
										OnCellSelected (cellSelected);	
									}
								} 
								else 
								{
									OnCellSelected (null);	
								}
							}
							else 
							{
								OnCellSelected (null);	
							}
						}
					}
				}
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if(currentSelected)
				{
					activateRayCasters(true);
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

	void activateRayCasters(bool activate)
	{
		InputBase.activateAllRayCasters(activate);
		/*for(int i = 0; i < rayCasters.Length; i++)
		{
			rayCasters[i].SetActive(activate);	
		}*/	
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
		isOnPlayer (false);
	}

	public void setCurrentSelected(GameObject selected)
	{
		currentSelected = selected;
	}

	protected void isOnPlayer(bool isOn)
	{
		if(OnPlayer != null)
		{
			OnPlayer (isOn);
		}
	}
}