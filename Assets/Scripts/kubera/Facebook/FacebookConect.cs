using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookConect : MonoBehaviour {

	public GameObject conect;
	public GameObject logOut;

	protected FBLog fbLog;

	public void Awake()
	{
		fbLog = FindObjectOfType<FBLog> ();

		if(fbLog == null)
		{
			fbLog =  gameObject.AddComponent<FBLog> ();
		}

		changeButton (fbLog.isLoggedIn);

		fbLog.onLoginComplete += changeButton;
	}

	public void conectFacebook()
	{
		fbLog.OnLoginClick ();
	}

	public void changeButton(bool loggedIn)
	{
		if(loggedIn)
		{
			conect.SetActive (false);
			logOut.SetActive (true);
		}
		else
		{
			conect.SetActive (true);
			logOut.SetActive (false);
		}
	}

	public void logOutFacebook()
	{		
		fbLog.LogOut ();
	}
}
