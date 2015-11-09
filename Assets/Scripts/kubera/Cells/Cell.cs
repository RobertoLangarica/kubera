using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour 
{
	public ECOLORS_ID color;
	public bool available;
	public bool occupied;

	public GameObject piece;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clearCell()
	{
		color = ECOLORS_ID.NONE;
		occupied = false;
		piece = null;
	}
}
