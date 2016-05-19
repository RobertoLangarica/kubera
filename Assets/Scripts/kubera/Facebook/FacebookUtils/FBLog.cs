using UnityEngine;
using System.Collections;
using Facebook.Unity;

public class FBLog : MonoBehaviour {

	public delegate void DOnLoginComplete(bool complete);

	public DOnLoginComplete onLoginComplete;

	public bool isLoggedIn;
	void Start()
	{
		print ("awake ");
		Debug.Log ("awake");
		// Initialize FB SDK
		if (!FB.IsInitialized)
		{
			print ("not IsInitialized ");
			FB.Init(InitCallback);
		}
		else
		{
			print ("IsInitialized");
		}

		if(FB.IsLoggedIn)
		{
			isLoggedIn = true;
		}
	}

	public void InitCallback()
	{
		onLoginComplete += foo;
		Debug.Log("InitCallback");

		// App Launch events should be logged on app launch & app resume
		// See more: https://developers.facebook.com/docs/app-events/unity#quickstart
		FB.ActivateApp();

		if (FB.IsLoggedIn) 
		{
			Debug.Log("Already logged in");
			OnLoginComplete();
		}
		else
		{
			onLoginComplete (false);
		}
	}

	protected void foo(bool foo){}

	private void OnLoginComplete()
	{
		Debug.Log("OnLoginComplete");

		if (!FB.IsLoggedIn)
		{
			onLoginComplete (false);
			//facebookBtn.SetActive (true);
		}
		else
		{
			onLoginComplete (true);
		}
	}

	public void OnLoginClick ()
	{
		Debug.Log("OnLoginClick");

		// hacer inactivo el boton para hacer login
		//facebookBtn.SetActive (false);

		// Call Facebook Login for Read permissions of 'public_profile', 'user_friends', and 'email'
		FBPermissions.PromptForLogin();
		FBPermissions.PromptForPublish(OnLoginComplete);
	}

	public void LogOut ()
	{
		FB.LogOut ();
	}

	void OnApplicationPause (bool pauseStatus)
	{
		#if UNITY_EDITOR
			return;
		#endif

		#if UNITY_ANDROID || UNITY_IOS
		// Check the pauseStatus to see if we are in the foreground
		// or background
		if (!pauseStatus) {
			//app resume
			if (FB.IsInitialized) 
			{
				FB.ActivateApp();
			} 
			else 
			{
				//Handle FB.Init
				FB.Init( () => {
					FB.ActivateApp();
				});
			}
		}	
		#endif
	}
}
