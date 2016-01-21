using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowNext : MonoBehaviour {

	public GameObject next;
	public Sprite[] image;

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

	public void isWordCompleted(bool isCompleted)
	{
		if(isCompleted)
		{
			//print("completed");
			next.GetComponent<Image>().sprite = image[0];
		}
		else
		{
			//print("NOTcompleted");
			next.GetComponent<Image>().sprite = image[1];
		}
	}
}
