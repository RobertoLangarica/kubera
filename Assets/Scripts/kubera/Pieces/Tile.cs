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
}