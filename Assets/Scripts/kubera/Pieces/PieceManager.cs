using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public List<GameObject> piecesInBar = new List<GameObject>();
	public List<GameObject> piecesToRotate = new List<GameObject>();
	public List<ABCChar> listChar = new List<ABCChar>();

	public Transform[] firstPos;
	public Transform[] rotatePos;

	protected int sizeOfBar =3;

	public static PieceManager instance;
	protected int figuresInBar;
	public GameManager gameManager;

	protected List<string> poolLeters = new List<string>();

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
		string[] myPieces = gameManager.data.levels[0].pieces.Split(new char[1]{','});

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
		string[] myPieces = gameManager.data.levels[0].pool.Split(new char[1]{','});

		for(int i =0; i<myPieces.Length; i++)
		{
			//print(myPieces[i]);
			poolLeters.Add (myPieces[i]);
		}
	
		List<string> newList = new List<string>();
		
		while(poolLeters.Count >0)
		{
			int val = UnityEngine.Random.Range(0,poolLeters.Count);
			newList.Add(poolLeters[val]);
			poolLeters.RemoveAt(val);
		}

		poolLeters = newList;
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
		string letter = poolLeters[0];
		poolLeters.RemoveAt (0);
		if(poolLeters.Count==0)
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

	public void setPieceInLetter(GameObject letter)
	{

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
