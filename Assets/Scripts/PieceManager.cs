using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public List<GameObject> piciesInBar = new List<GameObject>();
	public Transform[] firstPos;

	protected int sizeOfBar =3;

	public static PieceManager instance;
	protected int figuresInBar;
	public GameManager gameManager;

	// Use this for initialization
	void Start () {
		instance = this;
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
			piciesInBar.Add(piecesList[0]);
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
			int val = Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;
	}

	public void checkBarr(GameObject piciesSelected)
	{
		figuresInBar--;
		piciesInBar.Remove(piciesSelected);
		if(figuresInBar ==0)
		{
			fillBar();
			figuresInBar = sizeOfBar;
		}
	}
}
