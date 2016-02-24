using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	protected List<GameObject> safeList;

	public List<Piece> piecesInBar = new List<Piece>();
	public Transform[] firstPos;

	protected int sizeOfBar = 3;

	protected Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);
	protected int piecesAvailable;
	protected Transform piecesStock;

	public delegate void DOnShowMoney(bool show, int money);
	public DOnShowMoney OnShowMoney;

	public delegate void DOnRotate(bool rotating);
	public DOnRotate OnRotate;

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
			piecesInBar.Add(go.GetComponent<Piece>());
		}
	}

	public void setPiecesPoolList(List<GameObject> piecesPoolList)
	{
		safeList = new List<GameObject>();
		safeList = piecesPoolList;

		initializeAvailablePieces ();
	}

	protected void fillList()
	{
		List<GameObject> newList = new List<GameObject>();
		piecesList = new List<GameObject>(safeList);

		while(piecesList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;

	}

	public void checkPiecesToPosisionate(GameObject pieceSelected)
	{
		piecesAvailable--;
		int toDelete = 0;
		for(int i=0; i<piecesInBar.Count; i++)
		{
			if(pieceSelected.name == piecesInBar[i].name)
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

	public void setRotatePiece(GameObject go)
	{
		if (go.GetComponent<Piece> ()) 
		{
			setRotatePiece(go.GetComponent<Piece>());
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
			OnRotate (isRotating);

			piece.howManyHasBeenRotated += 1;
				
			piece.gameObject.transform.DOScale (new Vector3 (0, 0), 0.25f).OnComplete (() => {

				piece.transform.localRotation = Quaternion.Euler(new Vector3(0,0,piece.transform.rotation.eulerAngles.z+90));
			
				piece.transform.DOScale (initialPieceScale, 0.25f).OnComplete (() => {
					isRotating = false;
					OnRotate (isRotating);
				});
			});

			if (piece.howManyHasBeenRotated > piece.rotateTimes) {
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

		for (int i = 0; i < piecesInBar.Count; i++) 
		{
			if (piecesInBar[i].howManyHasBeenRotated != 0) 
			{				
				int temp = piecesInBar[i].howManyHasBeenRotated;

				piecesInBar [i].transform.DOScale (new Vector3 (0.1f, 0.1f), 0.25f);
				StartCoroutine(pos(i,temp));
			}
		}
		checkIfExistRotatedPiezes ();
	}

	IEnumerator pos(int i,int rotateTimes)
	{
		yield return new WaitForSeconds (0.25f);
		piecesInBar [i].transform.localRotation = Quaternion.Euler(new Vector3(0,0,piecesInBar [i].transform.rotation.eulerAngles.z-(90 * rotateTimes)));
		piecesInBar [i].transform.DOScale (initialPieceScale, 0.25f);
	}

	//checamos si una pieza esta rotada
	public bool checkIfExistRotatedPiezes()
	{
		bool OneWasRotated = false;
		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{	
				OneWasRotated =  true;
				return true;
			}
		}
			
		return false;
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