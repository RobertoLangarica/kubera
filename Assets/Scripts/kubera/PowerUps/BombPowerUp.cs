using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	public GameObject powerUpBlock;
	public Transform powerUpButton;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected GameManager gameManager;

	protected GameObject bombGO;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		gameManager = FindObjectOfType<GameManager>();
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

				for(int i = 0;i < selection.Length;i++)
				{
					cellsManager.occupyAndConfigureCell(selection[i],gameManager.getAndRegisterNewLetter("normal"),EPieceType.LETTER,true);
				}
				//cellsManager.destroyCells(selection);

				DestroyImmediate(bombGO);
				bombInput.OnDrop -= powerUPPositionated;
				bombInput.enabled = false;
				OnComplete ();
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