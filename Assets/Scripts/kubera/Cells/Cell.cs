using UnityEngine;
using System.Collections;

public enum ECOLORS_ID
{
	NONE,
	BLUE,
	RED,
	GREEN,
	LETER
}

public class Cell : MonoBehaviour 
{
	public ECOLORS_ID color;
	public bool available;
	public bool occupied;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
