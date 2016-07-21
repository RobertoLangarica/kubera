using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WordHintPowerUp : PowerupBase {

	protected CellsManager cellsManager;
	protected InputBombAndDestroy powerUpInput;
	protected GameManager gameManager;
	protected WordManager wordManager;

	protected bool canUse;
	protected GameObject powerUpGO;

	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		wordManager = FindObjectOfType<WordManager>();
		cellsManager = FindObjectOfType<CellsManager>();
		powerUpInput = FindObjectOfType<InputBombAndDestroy>();
	}

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
		}
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(powerUpGO.transform.position);

		if(cellSelected != null)
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


		OnCompletedNoGems ();

	}
}