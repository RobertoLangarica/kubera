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
			showingPieces [i].createdIndex = i;
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
		List<Piece> pieces = new List<Piece>();

		Dictionary<string, Object> resources = new Dictionary<string, Object>();

		for(int i =0; i<piecesInfo.Length; i++)
		{
			info = piecesInfo[i].Split('_');
			amount = int.Parse(info[0]);

			for(int j=0; j<amount; j++)
			{
				if(!resources.ContainsKey(info[1]))
				{
					Debug.Log(info[1]);
					Debug.Log(Resources.Load(info[1]));
					resources.Add(info[1],Resources.Load(info[1]));
				}

				Debug.Log(resources.ContainsKey(info[1])+"__"+resources[info[1]]);
				pieces.Add( ((GameObject)resources[info[1]]).GetComponent<Piece>() );
			}
		}

		//Liberando resources usados
		foreach(KeyValuePair<string, Object> item in resources)
		{
			//GameObject.DestroyImmediate((GameObject)item.Value);
		}

		Resources.UnloadUnusedAssets();

		setPieces(pieces);
	}
}