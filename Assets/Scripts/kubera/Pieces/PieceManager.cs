using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	//CHANGE: pieces en plural es redundante con List asi que puede cambiar el nombre
	//CHANGE: una lista de piezas tiene significado para PieceManager y no me queda claro porque son GameObject
	//TODO: al parecer piecesList es la lista de las piezas que quedan, puede ser remainingPieces o availablePieces
	//DONE: cambio de nombre al sugerido
	protected List<Piece> availablePieces = new List<Piece>();
	//TODO: safeList es la lista de todas las piezas y podria llamarse asi pieces
	//DONE: cambio de nombre 
	protected List<Piece> allPieces;

	//TODO: PieceManager no conoce una barra, puede tener una lista de piezas siendo mostradas showingPieces
	//DONE: cambio de nombre al sugerido
	public List<Piece> showingPieces = new List<Piece>();

	//TODO: Un nombre que indique que estas son las posiciones
	//DONE: cambio de nombre
	public Transform[] piecesInitialPosition;

	//TODO: Hay que poner un nombre que indique que esta es la cantidad de piezas que se pueden mostrar
	//CHANGE: Que sea publica para poder editar esas piezas
	//DONE: cambio de nombre
	public int quantityOfPiecesCanShow = 3;

	//CHANGE: Cantidad de piezas que se estan mostrando, hay que ponerle un nombre que indique eso
	//DONE: cambio de nombre
	[HideInInspector]
	public int quantityOfPiecesShowing;


	[HideInInspector]public bool isRotating = false;

	void Start () 
	{
		quantityOfPiecesShowing = quantityOfPiecesCanShow;
	}
		
	//TODO: Esto inicializa las piezas que se van a mostrar, un mejor nombre
	//DONE: cambio de nombre
	public void initializeShowingPieces()
	{
		for(int i= 0; i<quantityOfPiecesCanShow; i++)
		{
			if(availablePieces.Count==0)
			{
				randomizelist(allPieces);
			}

			showingPieces.Add((availablePieces [i]));
		}
		quantityOfPiecesShowing = quantityOfPiecesCanShow;
	}

	public List<Piece> getShowingPieceList()
	{
		return showingPieces;
	}

	public void setPiecesInList(List<GameObject> piecesList)
	{
		List<Piece> gameObjectListToPiecesList = new List<Piece>();

		for (int i = 0; i < piecesList.Count; i++) 
		{
			gameObjectListToPiecesList.Add (piecesList [i].GetComponent<Piece> ());
		}

		setPiecesInList (gameObjectListToPiecesList);
	}

	public void setPiecesInList(List<Piece> piecesList)
	{
		allPieces = new List<Piece>();
		allPieces = piecesList;
	}

	//CHANGE: Al parecer randomizelist no llena la lista, randomiza la lista de piezas
	//TODO: Que sea un randomizelist y que reciba que lista randomizar para que sea reusable
	//DONE: cambio de nombre al sugerido y recibe lista
	protected void randomizelist(List<Piece> pieces)
	{
		List<Piece> newList = new List<Piece>();
		availablePieces = new List<Piece>(pieces);

		while(availablePieces.Count >0)
		{
			int val = UnityEngine.Random.Range(0,availablePieces.Count);
			newList.Add(availablePieces[val]);
			availablePieces.RemoveAt(val);
		}

		availablePieces = newList;
	}

	public void removeFromListPieceUsed(GameObject pieceSelected)
	{
		quantityOfPiecesShowing--;
		int toDelete = 0;

		for(int i=0; i<showingPieces.Count; i++)
		{
			if(pieceSelected.name == showingPieces[i].gameObject.name)
			{
				toDelete=i;
				break;
			}
		}
		showingPieces.RemoveAt (toDelete);

		/*if(getShowingPieceList().Count == 0)
		{
			//initializeShowingPieces();
		}*/
	}
}