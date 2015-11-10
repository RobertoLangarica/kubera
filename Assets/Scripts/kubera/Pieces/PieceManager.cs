using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public List<GameObject> piecesInBar = new List<GameObject>();
	public List<ABCChar> listChar = new List<ABCChar>();

	public Transform[] firstPos;

	protected int sizeOfBar =3;

	public static PieceManager instance;
	protected int figuresInBar;
	public GameManager gameManager;

	protected List<string> poolLeters = new List<string>();

	//texturas de las letras
	protected UnityEngine.Object[] textures;
	protected string[] names;

	// Use this for initialization
	void Start () {
		instance = this;

		textures = Resources.LoadAll("Letters");
		names = new string[textures.Length];
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
			go.GetComponent<Piece>().myFirstPos=firstPos[i];

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

		sprite = (Sprite)textures[Array.IndexOf(names, nTextureName)];		
		return sprite;
	}

	protected void readTextures()
	{
		for(int i=0; i< names.Length; i++) 
		{
			names[i] = textures[i].name;
		}
	}

	public void setPieceInLetter(GameObject letter)
	{

	}
}
