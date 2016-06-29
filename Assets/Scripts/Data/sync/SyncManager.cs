using UnityEngine;
using System.Collections;

namespace Data.Sync
{
	public class SyncManager : Manager<SyncManager>
	{
		public LoginProvider customProvider;
		public LoginProvider facebookProvider;

		protected GameUser currentUser;


		void Awake()
		{
			customProvider.OnLoginSuccessfull += OnCustomLogin;
			facebookProvider.OnLoginSuccessfull += OnFacebookLogin;

			customProvider.OnLogoutSuccessfull += OnCustomLogout;
			facebookProvider.OnLogoutSuccessfull += OnFacebookLogout;
		}

		public void facebookLogin()
		{
			facebookProvider.login();
		}

		public void facebookLogout()
		{
			facebookProvider.logout();
		}

		public void customLogin()
		{
			customProvider.login();
		}

		public void customLogout()
		{
			customProvider.logout();
		}

		protected void OnCustomLogin(string message)
		{
			if(!existCurrentUser())
			{
				//TODO: Nos traemos el usuario si existe o uno nuevo si es necesario
				//TODO: Si ya hizo login con facebook y no a llegado el usuario hay que usar este id tambien
			}
			else
			{
				if(!customIdBelongsToCurrentUser())
				{
					//Este id no pertenece al current user
					//TODO: Ver si existe otro usuario con este id y si no ligar el qur tenemos, si ya existe y no tiene facebook id ligarlos
				}
			}
		}

		protected void OnFacebookLogin(string message)
		{
			if(!existCurrentUser())
			{
				//TODO: Nos traemos el usuario si existe o uno nuevo si es necesario
				//TODO: Si ya hizo login con custom y no a llegado el usuario hay que usar este id tambien
			}
			else
			{
				if(!facebookIdBelongsToCurrentUser())
				{
					//Este id no pertenece al current user

					//TODO: Ver si existe otro usuario con este id y si no ligar el que tenemos, si ya existe y no tiene facebook id ligarlos
				}
			}
		}

		public bool existCurrentUser()
		{
			return currentUser != null;
		}

		public bool customIdBelongsToCurrentUser()
		{
			return currentUser.customId == customProvider.userId;
		}

		public bool facebookIdBelongsToCurrentUser()
		{
			return currentUser.facebookId == facebookProvider.userId;
		}

		protected void OnCustomLogout(string message)
		{
			if(!facebookProvider.isLoggedIn)
			{
				
			}
		}

		protected void OnFacebookLogout(string message)
		{

		}
	}
}