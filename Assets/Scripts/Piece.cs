using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

	public GameObject[] pieces;

	[HideInInspector]
	public Transform myFirstPos;

	// Use this for initialization
	void Start () {
		selectAColor ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void selectAColor()
	{
		Color myColor = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));

		foreach(GameObject piece in pieces)
		{
			piece.GetComponent<SpriteRenderer>().color = myColor;
		}
	}
}
