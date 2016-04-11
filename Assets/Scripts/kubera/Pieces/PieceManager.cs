using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	public GameObject singleSquarePrefab;

	public int piecesToShow = 3;

	protected RandomPool<GameObject> piecesPrefab;
	[HideInInspector]public List<Piece> showingPieces = new List<Piece>();

	[HideInInspector]public int piecesShowedCount;

	void Start()
	{
		//Para que las instancias ya lo traigan desactivado
		singleSquarePrefab.GetComponent<BoxCollider2D>().enabled = false;	
	}

	public void initializePiecesToShow()
	{
		for(int i= 0; i<piecesToShow; i++)
		{
			showingPieces.Add( GameObject.Instantiate(piecesPrefab.getNextRandomized()).GetComponent<Piece>() );
			showingPieces[i].gameObject.SetActive(true);
			showingPieces [i].createdIndex = i;
		}

		piecesShowedCount = piecesToShow;
	}

	public List<Piece> getShowingPieces()
	{
		return showingPieces;
	}

	/**
	 * Se guardan solo prefabs porque se instancian al momento de necesitarse
	 **/ 
	public void setPiecesPrefabs(List<GameObject> value)
	{
		piecesPrefab = new RandomPool<GameObject>(value);
	}
		
	public void removeFromShowedPieces(Piece piece)
	{
		for(int i=0; i<showingPieces.Count; i++)
		{
			if(piece.GetInstanceID() == showingPieces[i].GetInstanceID())
			{
				piecesShowedCount--;
				showingPieces.RemoveAt (i);
				return;
			}
		}
	}

	public bool isAShowedPiece(Piece piece)
	{
		for(int i=0; i<showingPieces.Count; i++)
		{
			if(piece.GetInstanceID() == showingPieces[i].GetInstanceID())
			{
				return true;
			}
		}

		return false;
	}

	public void initializePiecesFromCSV(string csv)
	{
		string[] info;
		int amount = 0;

		string[] piecesInfo = csv.Split(',');
		List<GameObject> prefabs = new List<GameObject>();

		for(int i =0; i<piecesInfo.Length; i++)
		{
			info = piecesInfo[i].Split('_');
			amount = int.Parse(info[0]);

			for(int j=0; j<amount; j++)
			{
				prefabs.Add((GameObject)Resources.Load(info[1]));
				prefabs[prefabs.Count-1].SetActive(false);
			}
		}

		setPiecesPrefabs(prefabs);
	}

	public Piece getSingleSquarePiece(int colorIndex)
	{
		GameObject go = GameObject.Instantiate (singleSquarePrefab) as GameObject;
		Piece piece = go.GetComponent<Piece>();
		piece.currentType = Piece.EType.PIECE;
		piece.currentColor = (Piece.EColor)colorIndex;

		return piece;
	}

	public void showingShadow(Piece piece,bool showing)
	{
		for(int i=0; i<piece.shadows.Length; i++)
		{
			piece.shadows [i].enabled = showing;
			if(showing)
			{
				//print (piece.squaresSprite[i].sortingOrder);
				piece.shadows [i].sortingOrder = piece.squaresSprite[i].sortingOrder ++;
			}
			else
			{
				piece.shadows [i].sortingOrder = piece.squaresSprite[i].sortingOrder --;
			}
		}
	}
}