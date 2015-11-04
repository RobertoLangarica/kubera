using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour {

	protected List<GameObject> piecesList = new List<GameObject>();
	public Transform[] firstPos;

	protected int sizeOfBar =3;

	public static PieceManager instance;
	protected int figuresInBar;

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
			piecesList.RemoveAt(0);
			go.transform.position= firstPos [i].position;
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
		piecesList.Add ((GameObject)(Resources.Load ("A")));
		piecesList.Add ((GameObject)(Resources.Load ("B")));
		piecesList.Add ((GameObject)(Resources.Load ("C")));
		piecesList.Add ((GameObject)(Resources.Load ("D")));
		piecesList.Add ((GameObject)(Resources.Load ("E")));

		List<GameObject> newList = new List<GameObject>();

		while(piecesList.Count >0)
		{
			int val = Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;
	}

	public void checkBarr()
	{
		figuresInBar--;
		if(figuresInBar ==0)
		{
			fillBar();
			figuresInBar = sizeOfBar;
		}
	}
}
