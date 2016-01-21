using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	public GameObject letterCase;
	public ETYPEOFPIECE_ID typeOfPiece;

	public Cell cellIndex;
	protected GameManager gameManager;
	protected CellsManager cellsManager;
	
	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		cellsManager = FindObjectOfType<CellsManager>();
	}

	/*
	 * Esta funcion se ejecuta cuando se ha seleccionado una pieza para ser destruida
	 */
	public void selectPieceColorToDestroy()
	{
		if(typeOfPiece != ETYPEOFPIECE_ID.LETTER)
		{
			cellsManager.selectCellsOfColor(gameObject);
	
			cellsManager.destroySelectedCells();
		}

		cellsManager.deactivatePositionedPiecesCollider();

		gameManager.destroyByColor = false;
	}
}