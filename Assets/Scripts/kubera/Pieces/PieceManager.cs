using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	protected List<Piece> availablePieces = new List<Piece>();
	protected List<Piece> allPieces;

	public List<Piece> showingPieces = new List<Piece>();
	public int piecesToShow = 3;
	[HideInInspector]	public int piecesShowedCount;

	public void initializePiecesToShow()
	{
		for(int i= 0; i<piecesToShow; i++)
		{
			if(availablePieces.Count==0)
			{
				randomizelist(allPieces);
			}
			showingPieces.Add((availablePieces [0]));
			availablePieces.RemoveAt (0);
		}
		piecesShowedCount = piecesToShow;
	}

	protected void randomizelist(List<Piece> pieces)
	{
		List<Piece> newList = new List<Piece>();
		availablePieces = new List<Piece>(pieces);

		int val;
		while(availablePieces.Count >0)
		{
			val = Random.Range(0,availablePieces.Count);
			newList.Add(availablePieces[val]);
			availablePieces.RemoveAt(val);
		}

		availablePieces = newList;
	}

	public List<Piece> getShowingPieces()
	{
		return showingPieces;
	}

	public void setPieces(List<GameObject> gameObjects)
	{
		List<Piece> pieces = new List<Piece>();

		for (int i = 0; i < gameObjects.Count; i++) 
		{
			pieces.Add (gameObjects [i].GetComponent<Piece> ());
		}
		setPieces (pieces);
	}

	public void setPieces(List<Piece> pieces)
	{
		allPieces = pieces;
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
}