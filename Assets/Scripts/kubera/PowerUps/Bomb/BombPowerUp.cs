using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected BombAnimation bombAnimation;
	protected GameManager gameManager ;

	protected bool canUse;
	protected GameObject bombGO;

	protected Cell highLightCell;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		bombAnimation = FindObjectOfType<BombAnimation> ();
		gameManager = FindObjectOfType<GameManager> ();
		bombAnimation.OnCellFlipped += gameManager.OnCellFlipped;
		bombAnimation.OnAllAnimationsCompleted += gameManager.updatePiecesLightAndUpdateLetterState;
	}

	public override void activate(bool canUse)
	{
		if (bombGO != null) 
		{
			DestroyImmediate (bombGO);
		}
		bombGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		bombGO.name = "BombPowerUp";
		bombGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);
		bombGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";

		bombInput.enabled = true;
		bombInput.setCurrentSelected(bombGO);
		bombInput.OnDrop += powerUpPositioned;
		bombInput.OnCellSelected += onOverCellChanged;
		this.canUse = canUse;

		updateDragableObjectImage (bombGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_POWERUP);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("bomb");
		}
	}

	public void powerUpPositioned()
	{
		bombInput.OnCellSelected -= onOverCellChanged;
		Cell cellSelected = cellsManager.getCellUnderPoint(bombGO.transform.position);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("bomb");
		}

		if(cellSelected != null)
		{
			if(cellSelected.contentType == Piece.EType.PIECE && cellSelected.occupied)
			{
				if(canUse)
				{
					StartCoroutine (bombAnimation.startSameColorSearchAnimation(cellSelected));
					DestroyImmediate(bombGO);
					bombInput.OnDrop -= powerUpPositioned;
					bombInput.enabled = false;

					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_POWERUP);
					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR);

					OnComplete ();
				}
				else
				{
					onCompletedNoGems();
				}
			}
			else 
			{
				cancel();
			}
		}
		else
		{
			cancel();
		}
	}

	public override void cancel()
	{
		bombGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		bombGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (bombGO);
		});

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_POWERUP);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR);

		OnCancel();
	}

	public void onCompletedNoGems()
	{
		bombGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		bombGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (bombGO);
		});

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_POWERUP);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR);

		OnCompletedNoGems ();

	}

	public void onOverCellChanged(Cell cellSelected)
	{
		if (cellSelected != null) 
		{
			if (highLightCell == null) 
			{
				highLightCell = cellSelected;
			}

			if (cellSelected.contentColor != highLightCell.contentColor) 
			{
				HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR);
			}


			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR,cellSelected);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_POWERUP);

			highLightCell = cellSelected;
		} 
		else 
		{
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_POWERUP);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_SPECIFIC_COLOR);
		}
	}
}