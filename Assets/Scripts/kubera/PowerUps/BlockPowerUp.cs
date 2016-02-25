using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BlockPowerUp : PowerupBase
{
	protected PieceManager pieceManager;
	protected GameManager gameManager;
	public GameObject powerUpBlock;
	public Transform butonPowerUpBlock;

	public Vector3 offsetPositionOverFinger = new Vector3(0,1.5f,0);
	public float pieceSpeed = 0.3f;

	protected bool somethingDragged = false;
	protected int lastTimeDraggedFrame;
	protected GameObject currentSelected = null;

	void Start()
	{
		pieceManager = FindObjectOfType<PieceManager> ();
		gameManager = FindObjectOfType<GameManager> ();
		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		createBlock ();
		this.gameObject.SetActive( true);
	}

	protected void createBlock()
	{
		Vector3 newPos = new Vector3 ();
		GameObject blockGO = Instantiate (powerUpBlock,butonPowerUpBlock.position,Quaternion.identity) as GameObject;

		newPos = butonPowerUpBlock.position + offsetPositionOverFinger;
		newPos.z = 0;
		currentSelected = blockGO;
		moveTo(currentSelected,newPos,pieceSpeed);
		blockGO.name = "Block";
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
					if (gameManager.dropPieceOnGrid (currentSelected.GetComponent<Piece> ())) 
					{
						pieceManager.setRotationPiecesAsNormalRotation ();
						completePowerUp (true);
					}
					else
					{
						returnSelectedToInitialState(0.2f);
						reset();
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
		{				print ("S");
			returnSelectedToInitialState (0.2f);
			reset ();
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
	}

	public void returnSelectedToInitialState(float delay = 0)
	{
		DOTween.Kill("InputBlock_SelectedPosition",true);
		DOTween.Kill("InputBlock_SelectedScale",true);

		if(delay == 0)
		{
			currentSelected.transform.position = butonPowerUpBlock.position;
		}
		else
		{
			currentSelected.transform.DOMove (butonPowerUpBlock.position, delay).SetId("InputBlock_InitialPosition");
			currentSelected.transform.DOScale (new Vector3(0,0,0), delay).SetId("InputBlock_ScalePosition");
		}
	}

	protected void completePowerUp(bool finish)
	{
		DOTween.Kill ("InputBlock_Dragging");
		if (finish) 
		{
			OnComplete ();
		}
		else 
		{
			OnCancel ();
		}
		this.gameObject.SetActive( false);
	}
}
