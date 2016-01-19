using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public List<GameObject> piecesInBar = new List<GameObject>();
	public List<GameObject> piecesToRotate = new List<GameObject>();
	//Lista de las letras que estan actualmente en la escena
	public List<ABCChar> listChar = new List<ABCChar>();

	public Transform[] firstPos;
	public Transform[] rotatePos;

	protected int sizeOfBar =3;

	public static PieceManager instance;
	protected int figuresInBar;
	public GameManager gameManager;

	//Lista de las posibles listas que pueden salir en orden aleatorio
	protected List<ScriptableABCChar> randomizedPoolLeters = new List<ScriptableABCChar>();
	//Lista de las letras que se leyeron del XML
	protected List<ScriptableABCChar> XMLPoolLeters = new List<ScriptableABCChar>();

	//texturas de las letras en juego
	protected UnityEngine.Object[] textureObject;

	protected string[] names;

	// Use this for initialization
	void Start () {
		instance = this;

		textureObject = Resources.LoadAll("Letters");
		names = new string[textureObject.Length];
		readTextures ();
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
			//print("barra "+i);
			GameObject go = Instantiate(piecesList[0]);
			go.name = piecesList[0].name;
			piecesInBar.Add(piecesList[0]);
			piecesList.RemoveAt(0);
			go.transform.position= new Vector3(firstPos [i].position.x,firstPos [i].position.y,1);
			go.GetComponent<Piece>().myFirstPosInt = i;
			go.GetComponent<Piece>().myFirstPos=firstPos[i];
			go.transform.localScale = new Vector3(4,4,4);
			if(piecesList.Count==0)
			{
				fillList();
			}
		}
	}

	protected void fillList()
	{
		//Debug.Log ("LLenando la lista");
		string[] myPieces = gameManager.currentLevel.pieces.Split(new char[1]{','});

		for(int i =0; i<myPieces.Length; i++)
		{
			piecesList.Add ((GameObject)(Resources.Load (myPieces[i])));
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
		string[] myPieces = gameManager.currentLevel.pool.Split(new char[1]{','});
		string[] piecesInfo;

		/*Aqui diseccionar el XML****************/
		int amout = 0;
		ScriptableABCChar newLetter = null;

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
				XMLPoolLeters.Add(newLetter);
			}
		}
		/*****/

		randomizedPoolLeters.Clear();
		randomizedPoolLeters = new List<ScriptableABCChar>(XMLPoolLeters);
		XMLPoolLeters.Clear();

		while(randomizedPoolLeters.Count >0)
		{
			int val = UnityEngine.Random.Range(0,randomizedPoolLeters.Count);
			XMLPoolLeters.Add(randomizedPoolLeters[val]);
			randomizedPoolLeters.RemoveAt(val);
		}

		randomizedPoolLeters = XMLPoolLeters;
		Debug.Log (randomizedPoolLeters.Count);
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

	public string putLeter()
	{
		string letter = randomizedPoolLeters[0].character;
		letter = letter.ToLower();
		randomizedPoolLeters.RemoveAt (0);
		if(randomizedPoolLeters.Count==0)
		{
			fillPoolLetter();
		}
		return letter;
	}

	public Sprite changeTexture(string nTextureName)
	{
		Sprite sprite;

		sprite = (Sprite)textureObject[Array.IndexOf(names, nTextureName)];		
		return sprite;
	}

	protected void readTextures()
	{
		for(int i=0; i< names.Length; i++)
		{
			names[i] = textureObject[i].name;
		}
	}

	//Se ponen las pieces que se rotaran
	public void setRotatePieces(Piece piece)
	{
		for(int k=0; k<piecesToRotate.Count; k++)
		{
			Destroy(piecesToRotate[k]);
			//piecesToRotate.RemoveAt(0);
		}
		piecesToRotate.Clear();
		int j=0;
		for(int i=piece.myFirstPosInt*3; i<piece.rotatePieces.Length+piece.myFirstPosInt*3; i++)
		{
			GameObject go = Instantiate(piece.rotatePieces[j]);
			go.name = piece.name;
			go.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
			go.transform.position = new Vector3(rotatePos[i].position.x,rotatePos[i].position.y,1);
			go.GetComponent<Piece>().myFirstPos = rotatePos[i];
			go.GetComponent<Piece>().firstPiece =false;
			go.GetComponent<Piece>().parent = piece.gameObject;
			piecesToRotate.Add(go);
			j++;
		}
	}

	public void destroyRotatePieces(Piece piece,bool destroyParent = true)
	{
		if(destroyParent)
		{
			Destroy(piece.parent);
		}
		for(int k=0; k<piecesToRotate.Count; k++)
		{
			if(piece != piecesToRotate[k].GetComponent<Piece>())
			{
				Destroy(piecesToRotate[k]);
			}
		}
		piecesToRotate.Clear();
	}
}
