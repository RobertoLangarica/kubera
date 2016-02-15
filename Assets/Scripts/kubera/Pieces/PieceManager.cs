using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ABC;
using DG.Tweening;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public List<GameObject> piecesInBar = new List<GameObject>();
	public List<GameObject> piecesToRotate = new List<GameObject>();
	//Lista de las letras que estan actualmente en la escena
	public List<ABCChar> listChar = new List<ABCChar>();

	public Transform[] firstPos;
	public Transform[] rotatePos;

	protected int sizeOfBar =3;

	protected int figuresInBar;
	public GameManager gameManager;

	//Lista de las posibles listas que pueden salir en orden aleatorio
	protected List<ScriptableABCChar> randomizedPoolLeters = new List<ScriptableABCChar>();
	//Lista de las letras que se leyeron del XML
	protected List<ScriptableABCChar> XMLPoolLeters = new List<ScriptableABCChar>();

	protected List<ScriptableABCChar> XMLPoolBlackLeters = new List<ScriptableABCChar>();

	protected Transform piecesStock;

	[HideInInspector]
	public bool isRotating = false;

	// Use this for initialization
	void Start () 
	{
		GameObject temp = new GameObject();
		piecesStock = temp.transform;
		temp.name = "PiecesStock";

		PowerUpBase.onRotateActive += activateRotation;
		//Debug.Log (textures.Length);

		fillPoolLetter ();
		figuresInBar = sizeOfBar;
		fillList ();
		fillBar ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	protected void fillBar()
	{
		for(int i= 0; i<sizeOfBar; i++)
		{
			GameObject go = Instantiate(piecesList[0]);
			go.name = piecesList[0].name;
			piecesInBar.Add(piecesList[0]);
			piecesList.RemoveAt(0);
			go.transform.position= new Vector3(firstPos [i].position.x,firstPos [i].position.y,1);
			go.GetComponent<Piece>().myFirstPosInt = i;
			go.GetComponent<Piece>().myFirstPos=firstPos[i];
			go.transform.localScale = new Vector3(2.5f,2.5f,2.5f);
			go.transform.SetParent (piecesStock);
			if(piecesList.Count==0)
			{
				fillList();
			}
		}
	}

	protected void fillList()
	{
		//Debug.Log ("LLenando la lista");
		string[] piecesInfo;
		int amout = 0;

		string[] myPieces = PersistentData.instance.currentLevel.pieces.Split(new char[1]{','});

		for(int i =0; i<myPieces.Length; i++)
		{
			piecesInfo = myPieces[i].Split(new char[1]{'_'});
			amout = int.Parse(piecesInfo[0]);

			for(int j=0; j<amout; j++)
			{
				piecesList.Add ((GameObject)(Resources.Load (piecesInfo[1])));
			}
		}

		List<GameObject> newList = new List<GameObject>();

		while(piecesList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;
	}

	protected void fillPoolLetter()
	{
		string[] myPieces = PersistentData.instance.currentLevel.lettersPool.Split(new char[1]{','});
		string[] piecesInfo;

		/*Aqui diseccionar el XML****************/
		int amout = 0;
		ScriptableABCChar newLetter = null;

		if(XMLPoolLeters.Count == 0)
		{
			for(int i =0; i<myPieces.Length; i++)
			{
				piecesInfo = myPieces[i].Split(new char[1]{'_'});
				amout = int.Parse(piecesInfo[0]);
				for(int j = 0;j < amout;j++)
				{
					newLetter = new ScriptableABCChar();
					newLetter.character = piecesInfo[1];
					newLetter.typeOfLetter = piecesInfo[2];
					newLetter.pointsValue = piecesInfo[3];
					XMLPoolLeters.Add(newLetter);
				}
			}

			if(PersistentData.instance.currentLevel.obstacleLettersPool.Length > 0)
			{
				myPieces = PersistentData.instance.currentLevel.obstacleLettersPool.Split(new char[1]{','});

				for(int i =0; i<myPieces.Length; i++)
				{
					piecesInfo = myPieces[i].Split(new char[1]{'_'});
					amout = int.Parse(piecesInfo[0]);
					for(int j = 0;j < amout;j++)
					{
						newLetter = new ScriptableABCChar();
						newLetter.character = piecesInfo[1];
						newLetter.pointsValue = piecesInfo[2];
						newLetter.typeOfLetter = piecesInfo[3];
						XMLPoolBlackLeters.Add(newLetter);
					}
				}
			}
		}
		/*****/

		randomizedPoolLeters = new List<ScriptableABCChar>();
		for(int i = 0;i < XMLPoolLeters.Count;i++)
		{
			randomizedPoolLeters.Add(XMLPoolLeters[i]);
		}
		XMLPoolLeters.Clear();

		while(randomizedPoolLeters.Count >0)
		{
			int val = UnityEngine.Random.Range(0,randomizedPoolLeters.Count);
			XMLPoolLeters.Add(randomizedPoolLeters[val]);
			randomizedPoolLeters.RemoveAt(val);
		}

		randomizedPoolLeters = XMLPoolLeters;
	}

	public void checkBarr(GameObject piecesSelected)
	{
		figuresInBar--;
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

		if(figuresInBar ==0)
		{
			fillBar();
			figuresInBar = sizeOfBar;
		}
	}

	public ScriptableABCChar giveLetterInfo()
	{
		ScriptableABCChar letter = randomizedPoolLeters[0];
		randomizedPoolLeters.RemoveAt (0);
		if(randomizedPoolLeters.Count==0)
		{
			fillPoolLetter();
		}
		return letter;
	}

	public ScriptableABCChar giveBlackLetterInfo()
	{
		int random = UnityEngine.Random.Range (0, XMLPoolBlackLeters.Count);
		ScriptableABCChar letter = XMLPoolBlackLeters[random];
		XMLPoolBlackLeters.RemoveAt (random);
		return letter;
	}

	//Se cambian las piezas por las rotadas
	public void setRotatePiece(Piece piece)
	{
		if (isRotating)
		{
			return;
		}

		if (piece.rotatePieces.Length > 0) {
			isRotating = true;

			GameObject go = Instantiate (piece.rotatePieces [0]) as GameObject;
			go.transform.localScale = new Vector3 (0, 0, 0);

			go.transform.position = piece.gameObject.transform.position;
			go.GetComponent<Piece> ().howManyHasBeenRotated = piece.howManyHasBeenRotated + 1;
			go.transform.SetParent (piecesStock);
			go.GetComponent<Piece> ().myFirstPos = piece.myFirstPos;

			if (go.GetComponent<Piece> ().howManyHasBeenRotated > piece.rotatePieces.Length) {
				go.GetComponent<Piece> ().howManyHasBeenRotated = 0;
				piece.howManyHasBeenRotated = 0;
			}
				
			piece.gameObject.transform.DOScale (new Vector3 (0, 0), 0.25f).OnComplete (() => {
				DestroyImmediate (piece.gameObject);
			
				go.transform.DOScale (new Vector3 (2.5f, 2.5f), 0.25f).OnComplete (() => {
					isRotating = false;
				});
			});

			checkIfExistRotatedPiezes ();
			//go.transform.DOScale (new Vector3 (2.5f, 2.5f), .5f);
			//DestroyImmediate (piece.gameObject);
		} 
		else 
		{
			print ("else");
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
				int temp = piecesStock.GetChild (i).GetComponent<Piece>().rotatePieces.Length- piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated;
				GameObject oldPiece = piecesStock.GetChild (i).gameObject;
				GameObject newPiece = Instantiate (piecesStock.GetChild (i).GetComponent<Piece>().rotatePieces[temp]);
				newPiece.transform.localScale = new Vector3 (0, 0, 0);
				
				newPiece.transform.position = piecesStock.GetChild (i).gameObject.transform.position;

				newPiece.GetComponent<Piece> ().howManyHasBeenRotated = 0;
				newPiece.transform.SetParent (piecesStock);

				newPiece.GetComponent<Piece> ().myFirstPos = oldPiece.GetComponent<Piece> ().myFirstPos;

				piecesStock.GetChild (i).transform.DOScale(new Vector3(0,0),.25f).OnComplete(()=>
					{
						newPiece.transform.DOScale (new Vector3 (2.5f, 2.5f), .25f).OnComplete(()=>
							{
								DestroyImmediate (oldPiece);
							});;
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
		//Prendemos el objeto de dinero
		if(OneWasRotated)
		{
			gameManager.activeMoney (true, 50);
		}
		else
		{
			gameManager.activeMoney (true, 0);
		}
	}

	//Seteamos las piezas rotadas como si fueran la original
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

	protected void activateRotation(bool activate)
	{
		if(activate)
		{
			//Activar las imagenes de rotar
			print("activateRotation");
		}
		else
		{
			returnRotatePiecesToNormalRotation ();
		}
	}
}
