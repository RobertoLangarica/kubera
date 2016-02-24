using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	public GameObject powerUpBlock;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;

	void Start () 
	{
		cellsManager = GameObject.FindObjectOfType<CellsManager>();
		bombInput = GameObject.FindObjectOfType<InputBombAndDestroy>();

		bombInput.OnDrop += powerUPPositionated;
	}

	public override void activate()
	{
		GameObject bombGO = Instantiate (powerUpBlock,transform.position,Quaternion.identity) as GameObject;
		bombGO.name = "BombPowerUp";

		bombInput.gameObject.SetActive(true);
	}

	public void powerUPPositionated()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(powerUpBlock.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.pieceType != EPieceType.LETTER 
				&& cellSelected.pieceType != EPieceType.LETTER_OBSTACLE
				&& cellSelected.occupied)
			{
				cellsManager.selectCellsOfColor(cellSelected,true);

				cellsManager.destroySelectedCells();
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
		powerUpBlock.transform.DOMove (new Vector3 (transform.position.x, transform.position.y, 1), .2f).SetId("BombPowerUP_Move");
		powerUpBlock.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("BombPowerUP_Scale");

		bombInput.OnDrop -= powerUPPositionated;
		
		OnCancel();
	}
}