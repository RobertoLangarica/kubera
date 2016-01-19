using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// Use this for initialization
	public string myLeterCase;
	protected bool used;

	public GameObject letterCase;
	public ETYPEOFPIECE_ID typeOfPiece;

	public Cell cellIndex;
	protected GameManager gameManager;
	protected CellsManager cellsManager;
	protected WordManager wordManager;
	
	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		cellsManager = FindObjectOfType<CellsManager>();
		wordManager = FindObjectOfType<WordManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShootLetter()
	{
		//print ("S");
		if(!used)
		{
			wordManager.addCharacter(myLeterCase,gameObject);
			used=true;
			gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,.2f);
		}
	}

	public void backToNormal()
	{
		used=false;
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
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