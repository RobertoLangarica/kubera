using UnityEngine;
using System.Collections;
using Facebook.Unity;

namespace Data.Sync
{
	public class LoginFacebook : LoginProvider 
	{
		void Awake()
		{
			id = "FacebookLoginProvider";	
		}

		void Start()
		{
			if(!FB.IsInitialized)
			{
				FB.Init(InitCallback);
			}
			else if(FB.IsLoggedIn)
			{
				isLoggedIn = true;
				userId = AccessToken.CurrentAccessToken.UserId;

				if(OnLoginSuccessfull != null)
				{
					OnLoginSuccessfull("success");
				}
			}
		}

		public void InitCallback()
		{
			//Debug.Log("FB->InitCallback");

			// App Launch events should be logged on app launch & app resume
			// See more: https://developers.facebook.com/docs/app-events/unity#quickstart
			FB.ActivateApp();

			if (FB.IsLoggedIn) 
			{
				isLoggedIn = true;
				userId = AccessToken.CurrentAccessToken.UserId;

				if(OnLoginSuccessfull != null)
				{
					OnLoginSuccessfull("success");
				}
			}
		}

		public override void login ()
		{
			FBPermissions.PromptForLogin();
			FBPermissions.PromptForPublish(OnLoginAttempComplete);
		}
			

		private void OnLoginAttempComplete()
		{
			if (FB.IsLoggedIn) 
			{
				userId = AccessToken.CurrentAccessToken.UserId;
				isLoggedIn = true;

				if(OnLoginSuccessfull != null)
				{
					OnLoginSuccessfull("success");
				}
			}
			else
			{
				if(OnLoginFail != null)
				{
					OnLoginFail("login failed");
				}
			}
		}

		public override void logout()
		{
			FB.LogOut();
			isLoggedIn = false;
			if(OnLogoutSuccessfull != null)
			{
				OnLogoutSuccessfull("success");
			}
		}


		void OnApplicationPause (bool pauseStatus)
		{
			#if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
			// Check the pauseStatus to see if we are in the foreground
			// or background
			if (!pauseStatus) 
			{
				//Solo nos interesa cuando ya lo inicializamos nosotros
				if (FB.IsInitialized) 
				{
					FB.ActivateApp();
				} 
			}	
			#endif
		}
	}
}