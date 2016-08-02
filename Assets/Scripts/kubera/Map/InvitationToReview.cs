using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InvitationToReview : MonoBehaviour {

	public delegate void DOnFinish();
	public DOnFinish OnFinish;

	public GameObject modal;
	public GameObject[] invitations;

	public bool isHappeningAReview(int currentLevelNumber)
	{
		switch (currentLevelNumber) 
		{
		case 11:
		case 21:
		case 28:
		case 32:
		case 40:
		case 44:
			return true;
		default:
			return false;
		}
	}

	public void showInvitationProcessByLevelNumber(int currentLevelNumber)
	{
		switch (currentLevelNumber) 
		{
		case 11:
			invitations [0].SetActive (true);
			break;
		case 21:
			invitations [1].SetActive (true);
			break;
		case 28:
			invitations [2].SetActive (true);
			break;
		case 32:
			invitations [3].SetActive (true);
			break;
		case 40:
			invitations [4].SetActive (true);
			break;
		case 44:
			invitations [5].SetActive (true);
			break;
		}
	}

	public void finish()
	{
		for(int i=0; i<invitations.Length; i++)
		{
			invitations [i].SetActive (false);
		}

		OnFinish ();
	}

}

