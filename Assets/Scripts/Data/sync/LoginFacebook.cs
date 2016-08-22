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

			if(FB.IsLoggedIn)
			{
				isLoggedIn = true;

				if(OnLoginSuccessfull != null)
				{
					OnLoginSuccessfull("success");
				}
			}
		}

		public void InitCallback()
		{
			Debug.Log("FB->InitCallback");

			// App Launch events should be logged on app launch & app resume
			// See more: https://developers.facebook.com/docs/app-events/unity#quickstart
			FB.ActivateApp();

			if (FB.IsLoggedIn) 
			{
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


				//TODO: traernos el mail del usuario

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

			if(OnLogoutSuccessfull != null)
			{
				OnLogoutSuccessfull("success");
			}
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
}