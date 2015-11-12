using UnityEngine;
using System.Collections;

public class ShowNext : MonoBehaviour {

	public GameObject next;

	void Start()
	{
		next.SetActive(false);
	}

	public void ShowingNext(bool showing)
	{
		if(showing)
		{
			next.SetActive(true);
		}
		else
		{
			next.SetActive(false);
		}
	}
}
