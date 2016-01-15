using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// Use this for initialization
	public string myLeterCase;
	protected bool used;

	public GameObject letterCase;
	public ECOLORS_ID color;

	public Cell cellIndex;
	protected GameManager gameManager;
	protected CellsManager cellsManager;
	
	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		cellsManager = FindObjectOfType<CellsManager>();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShootLetter()
	{
		//print ("S");
		if(!used)
		{
			GameObject.Find("WordManager").GetComponent<WordManager>().addCharacter(myLeterCase,gameObject);
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
		if(color != ECOLORS_ID.LETER)
		{
			cellsManager.selectCellsOfColor(gameObject);
	
			cellsManager.destroySelectedCells();
		}

		cellsManager.deactivatePositionedPiecesCollider();

		gameManager.destroyByColor = false;
	}
}