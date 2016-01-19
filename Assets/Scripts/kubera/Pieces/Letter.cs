using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Letter : MonoBehaviour {

	public GameObject piece;
	public bool isColliding;

	void Start()
	{
		GameObject wordManager = GameObject.Find("WordManager");

		//GetComponent<Button>().onClick.AddListener(
		//	() => wM.GetComponent<WordManager>().deleteCharFromSearch(gameObject.GetComponent<UIChar>())
		//										);
	}

	public void returnState()
	{
		//piece.GetComponent<Tile> ().backToNormal ();
	}

	public void DestroyPiece()
	{
		Destroy (piece);
	}

}
