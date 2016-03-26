using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	public int piecesToShow = 3;

	protected RandomPool<GameObject> piecesPrefab;
	public List<Piece> showingPieces = new List<Piece>();

	[HideInInspector]public int piecesShowedCount;

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
}