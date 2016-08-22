using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookConnectButton : MonoBehaviour {

	public Text fbText;
	public bool logged;

	protected FBLoggin fbLog;

	public void Awake()
	{
		fbLog = FBLoggin.GetInstance();

		if(fbLog == null)
		{
			fbLog =  gameObject.AddComponent<FBLoggin> ();
		}

		changeText (fbLog.isLoggedIn);

		fbLog.onLoginComplete += changeText;
	}

	public void conectFacebook()
	{
		fbLog.OnLoginClick ();
	}

	public void changeText(bool loggedIn)
	{
		//TODO tomar el string
		if(loggedIn)
		{
			fbText.text = "Desconectate";
			logged = false;
		}
		else
		{
			fbText.text = "Conectate";
			logged = true;
		}
	}

	public void logOutFacebook()
	{		
		fbLog.LogOut ();
	}
}
