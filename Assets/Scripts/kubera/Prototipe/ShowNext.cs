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

	public void isCompletedNotCompletedOrMoving(int imageValue)
	{
		switch (imageValue) {
		case 0:
			next.GetComponent<Image> ().sprite = image [0];//completed
			break;
		case 1:
			next.GetComponent<Image> ().sprite = image [1];//notCompleted
			break;
		case 2:
			next.GetComponent<Image> ().sprite = image [2];//Moving
			break;
		default:
			break;
		}
	}
}
