using UnityEngine;
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
		bombInput.OnDrop += powerUpPositioned;
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(destroyGO.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.contentType != EPieceType.LETTER 
				&& cellSelected.contentType != EPieceType.LETTER_OBSTACLE
				&& cellSelected.contentType != EPieceType.NONE
				&& cellSelected.occupied)
			{
				Cell[] selection = cellsManager.getCellsOfSameType(cellSelected);

				for(int i = 0;i < selection.Length;i++)
				{
					cellsManager.occupyAndConfigureCell(selection[i],gameManager.createLetterFromInfo(gameManager.lettersPool.getNextRandomized()).gameObject,EPieceType.LETTER,true);
				}

				DestroyImmediate(destroyGO);
				bombInput.OnDrop -= powerUpPositioned;
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

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		OnCancel();
	}
}
