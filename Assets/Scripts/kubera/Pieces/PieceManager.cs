using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	protected List<GameObject> safeList = new List<GameObject>();

	public List<GameObject> piecesInBar = new List<GameObject>();
	public Transform[] firstPos;

	protected int sizeOfBar = 3;

	protected Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);
	protected int piecesAvailable;
	protected Transform piecesStock;

	public delegate void DShowMoney(bool show, int money);
	public DShowMoney DOnShowMoney;

	[HideInInspector]
	public bool isRotating = false;

	void Start () 
	{
		GameObject temp = new GameObject();
		piecesStock = temp.transform;
		temp.name = "PiecesStock";

		piecesAvailable = sizeOfBar;
	}

	/**
	 * inicializa las piezas disponibles
	 **/
	protected void initializeAvailablePieces()
	{
		for(int i= 0; i<sizeOfBar; i++)
		{
			if(piecesList.Count==0)
			{
				fillList();
			}
			GameObject go = Instantiate(piecesList[0]);
			go.name = piecesList[0].name;

			piecesList.RemoveAt(0);
			go.transform.position= new Vector3(firstPos [i].position.x,firstPos [i].position.y,1);

			go.GetComponent<Piece>().myFirstPosInt = i;
			go.GetComponent<Piece>().myFirstPos=firstPos[i];
			go.transform.localScale = new Vector3 (0, 0, 0);
			go.transform.DOScale(initialPieceScale, 0.25f);
			go.transform.SetParent (piecesStock);
			piecesInBar.Add(go);
		}
	}

	public void setPiecesPoolList(List<GameObject> piecesPoolList)
	{
		safeList = piecesPoolList;

		initializeAvailablePieces ();
	}

	protected void fillList()
	{
		List<GameObject> newList = new List<GameObject>();
		piecesList = safeList;

		while(piecesList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;
	}

	public void checkPiecesToPosisionate(GameObject piecesSelected)
	{
		piecesAvailable--;
		int toDelete = 0;
		for(int i=0; i<piecesInBar.Count; i++)
		{
			if(piecesSelected.name == piecesInBar[i].name)
			{
				toDelete=i;
				break;
			}
		}
		piecesInBar.RemoveAt (toDelete);

		if(piecesAvailable ==0)
		{
			initializeAvailablePieces();
			piecesAvailable = sizeOfBar;
		}
	}

	//Se cambian las piezas por las rotadas
	public void setRotatePiece(Piece piece)
	{
		if (isRotating)
		{
			return;
		}

		if (piece.rotateTimes > 0) {
			isRotating = true;

			piece.howManyHasBeenRotated += 1;
				
			piece.gameObject.transform.DOScale (new Vector3 (0, 0), 0.25f).OnComplete (() => {

				piece.transform.localRotation = Quaternion.Euler(new Vector3(0,0,piece.transform.rotation.eulerAngles.z+90));
			
				piece.transform.DOScale (initialPieceScale, 0.25f).OnComplete (() => {
					isRotating = false;
				});
			});

			if (piece.howManyHasBeenRotated > piece.rotateTimes) {
				piece.howManyHasBeenRotated = 0;
				//piece.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
				piece.howManyHasBeenRotated = 0;
			}

			checkIfExistRotatedPiezes ();
		}
	}

	//regresamos la rotacion inicial si no se concreto la rotacion
	public void returnRotatePiecesToNormalRotation()
	{
		if (isRotating)
		{
			return;
		}

		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{				
				int temp = piecesStock.GetChild (i).GetComponent<Piece>().rotateTimes - piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated;

				piecesStock.GetChild (i).transform.DOScale(new Vector3(0,0),.25f).OnComplete(()=>
					{
						piecesStock.GetChild (i).transform.localRotation = Quaternion.Euler(new Vector3(0,0,90*temp));

						piecesStock.GetChild (i).transform.DOScale (initialPieceScale, .25f);
					});
			}
		}
		checkIfExistRotatedPiezes ();
	}

	//checamos si una pieza esta rotada
	public void checkIfExistRotatedPiezes()
	{
		bool OneWasRotated = false;
		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{	
				OneWasRotated =  true;
			}
		}
			
		if(OneWasRotated)
		{
			DOnShowMoney (true, 50);
		}
		else
		{
			DOnShowMoney (true, 0);
		}
	}

	public bool setRotationPiecesAsNormalRotation()
	{
		bool OneWasRotated = false;
		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{
				piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated = 0;
				OneWasRotated = true;
			}
		}
		return OneWasRotated;
	}

	public void activateRotation(bool activate)
	{
		if(activate)
		{
			//Activar las imagenes de rotar
			for (int i = 0; i < piecesInBar.Count; i++) 
			{
				print (piecesInBar [i].GetComponent<Piece> ().myFirstPosInt);
				firstPos [piecesInBar[i].GetComponent<Piece>().myFirstPosInt].GetComponent<Image> ().enabled = true;
			}
		}
		else
		{
			for (int i = 0; i < firstPos.Length; i++) 
			{
				firstPos [i].GetComponent<Image> ().enabled = false;
			}
			returnRotatePiecesToNormalRotation ();
		}
	}
}