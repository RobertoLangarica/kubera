using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

	public GameObject piece;

	void Start()
	{
		GameObject wM = GameObject.Find("WordManager");

		GetComponent<Button>().onClick.AddListener(
			() => wM.GetComponent<WordManager>().deleteCharFromSearch(gameObject.GetComponent<UIChar>())
												);
	}

	public void returnState()
	{
		piece.GetComponent<Letter> ().backToNormal ();
	}
}
