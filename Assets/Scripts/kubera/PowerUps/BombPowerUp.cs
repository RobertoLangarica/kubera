using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	public GameObject powerUpBlock;
	public Transform powerUpButton;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;

	protected GameObject bombGO;

	void Start () 
	{
		cellsManager = GameObject.FindObjectOfType<CellsManager>();
		bombInput = GameObject.FindObjectOfType<InputBombAndDestroy>();
	}

	public override void activate()
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
		bombInput.OnDrop += powerUPPositionated;
	}

	public void powerUPPositionated()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(bombGO.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.pieceType != EPieceType.LETTER 
				&& cellSelected.pieceType != EPieceType.LETTER_OBSTACLE
				&& cellSelected.occupied)
			{
				Cell[] selection =  cellsManager.getCellNeighborsOfSameType(cellSelected);

				cellsManager.destroyCells(selection);

				DestroyImmediate(bombGO);
				bombInput.OnDrop -= powerUPPositionated;
				bombInput.enabled = false;
			}
			else 
			{
				powerUPCanceled();
			}
		}
		else
		{
			powerUPCanceled();
		}
	}

	public void powerUPCanceled()
	{
		bombGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		bombGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (bombGO);
		});

		bombInput.OnDrop -= powerUPPositionated;
		bombInput.enabled = false;

		OnCancel();
	}
}