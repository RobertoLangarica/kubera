using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	public int piecesToShow = 3;

	protected RandomPool<Piece> pieces;
	public List<Piece> showingPieces = new List<Piece>();

	[HideInInspector]	public int piecesShowedCount;

	public void initializePiecesToShow()
	{
		for(int i= 0; i<piecesToShow; i++)
		{
			showingPieces.Add(pieces.getNextRandomized());
		}

		piecesShowedCount = piecesToShow;
	}

	public List<Piece> getShowingPieces()
	{
		return showingPieces;
	}

	public void setPieces(List<GameObject> value)
	{
		List<Piece> pieces = new List<Piece>();

		for (int i = 0; i < value.Count; i++) 
		{
			pieces.Add (value [i].GetComponent<Piece> ());
		}
		setPieces (pieces);
	}

	public void setPieces(List<Piece> value)
	{
		pieces = new RandomPool<Piece>(value);
	}
		
	public void removeFromShowedPieces(Piece piece)
	{
		for(int i=0; i<showingPieces.Count; i++)
		{
			if(piece.id == showingPieces[i].id)
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
			if(piece.id == showingPieces[i].id)
			{
				return true;
			}
		}

		return false;
	}
}