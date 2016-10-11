using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WordHintPowerUp : PowerupBase {

	public CellsManager cellsManager;
	public InputBombAndDestroy powerUpInput;
	public GameManager gameManager;
	public WordManager wordManager;

	protected bool canUse;
	protected bool canActivate;
	protected GameObject powerUpGO;


	public override void activate(bool canUse)
	{
		if (powerUpGO != null) 
		{
			DestroyImmediate (powerUpGO);
		}
		powerUpGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		powerUpGO.name = "WordHintPowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);
		powerUpGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";

		powerUpInput.enabled = true;
		powerUpInput.setCurrentSelected(powerUpGO);
		powerUpInput.OnDrop += powerUpPositioned;
		this.canUse = canUse;

		updateDragableObjectImage (powerUpGO);

		if(wordManager.checkIfAWordIsPossible(gameManager.getGridCharacters()))
		{
			canActivate = true;
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WORD_HINT);
		}
		else
		{
			canActivate = false;
		}
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(powerUpGO.transform.position);

		if(cellSelected != null && canActivate)
		{
			if (cellSelected.cellType != Cell.EType.EMPTY_VISIBLE_CELL && cellSelected.cellType != Cell.EType.EMPTY) 
			{
				if (canUse) 
				{
					//HACE
					gameManager.useHintWord (true);
					DestroyImmediate (powerUpGO);
					powerUpInput.OnDrop -= powerUpPositioned;
					powerUpInput.enabled = false;
					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT);
					if(AudioManager.GetInstance())
					{
						AudioManager.GetInstance().Stop("helpPositonated");
						AudioManager.GetInstance().Play("helpPositonated");
					}
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
		powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (powerUpGO);
		});

		powerUpInput.OnDrop -= powerUpPositioned;
		powerUpInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT);

		OnCancel();
	}

	public void onCompletedNoGems()
	{
		powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (powerUpGO);
		});

		powerUpInput.OnDrop -= powerUpPositioned;
		powerUpInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT);

		OnCompletedNoGems ();

	}
}