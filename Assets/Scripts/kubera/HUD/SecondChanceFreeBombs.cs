using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecondChanceFreeBombs: MonoBehaviour
{
	public GameObject FreeBombs;
	public Text FreeBombsText;

	void Start()
	{
		FreeBombs.SetActive (false);
	}

	public void activateFreeBombs(bool activate)
	{
		FreeBombs.SetActive (activate);
	}

	public void actualizeFreeBombs(int bombs)
	{
		FreeBombsText.text = bombs.ToString ();
	}
}