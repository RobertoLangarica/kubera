using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// Use this for initialization
	public string myLeterCase;
	protected bool used;

	public GameObject letterCase;

	public Cell cellIndex;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShootLetter()
	{
		//print ("S");
		if(!used)
		{
			GameObject.Find("WordManager").GetComponent<WordManager>().addCharacter(myLeterCase,gameObject);
			used=true;
			gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,.2f);
		}
	}

	public void backToNormal()
	{
		used=false;
		gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
	}
}
