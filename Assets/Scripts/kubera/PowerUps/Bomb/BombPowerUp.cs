using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected BombAnimation bombAnimation;

	protected bool canUse;
	protected GameObject bombGO;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		bombAnimation = FindObjectOfType<BombAnimation> ();
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

		bombInput.enabled = true;
		bombInput.setCurrentSelected(bombGO);
		bombInput.OnDrop += powerUpPositioned;
		bombInput.OnCellSelected += onOverCellChanged;
		this.canUse = canUse;

		updateDragableObjectImage (bombGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHIghLightType.BOMB_POWERUP);
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(bombGO.transform.position);

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

					HighLightManager.GetInstance ().turnOffHighLights ();

					OnComplete ();
				}
				else
				{
					onCompletedNoGems ();
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

		HighLightManager.GetInstance ().turnOffHighLights ();

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

		HighLightManager.GetInstance ().turnOffHighLights ();

		OnCompletedNoGems ();

	}

	public void onOverCellChanged(Cell cellSelected)
	{
		HighLightManager.GetInstance ().turnOffHighLights ();

		if (cellSelected != null) 
		{
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHIghLightType.BOMB_SPECIFIC_COLOR
			, cellSelected);
		} 
		else 
		{
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHIghLightType.BOMB_POWERUP);
		}
	}
}