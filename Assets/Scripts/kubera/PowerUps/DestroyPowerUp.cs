﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DestroyPowerUp : PowerupBase 
{
	public GameObject powerUpBlock;
	public Transform powerUpButton;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected GameManager gameManager;

	protected GameObject destroyGO;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		gameManager = FindObjectOfType<GameManager>();
	}

	public override void activate()
	{
		if (destroyGO != null) 
		{
			DestroyImmediate (destroyGO);
		}
		destroyGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		destroyGO.name = "DestroyPowerUp";
		destroyGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		bombInput.enabled = true;
		bombInput.setCurrentSelected(destroyGO);
		bombInput.OnDrop += powerUPPositionated;
	}

	public void powerUPPositionated()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(destroyGO.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.pieceType != EPieceType.LETTER 
				&& cellSelected.pieceType != EPieceType.LETTER_OBSTACLE
				&& cellSelected.occupied)
			{
				Cell[] selection = cellsManager.getCellsOfSameType(cellSelected);

				for(int i = 0;i < selection.Length;i++)
				{
					cellsManager.setCellContent(selection[i],gameManager.createLetterContent());
				}
				//cellsManager.destroyCells(selection);

				DestroyImmediate(destroyGO);
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
		destroyGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("DestroyPowerUP_Move");
		destroyGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("DestroyPowerUP_Scale").OnComplete (() => {

			DestroyImmediate(destroyGO);
		});

		bombInput.OnDrop -= powerUPPositionated;
		bombInput.enabled = false;

		OnCancel();
	}
}
