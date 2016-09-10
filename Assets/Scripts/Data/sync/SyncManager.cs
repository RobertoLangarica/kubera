using UnityEngine;
using System.Collections;
using Data;
using Data.Remote;

namespace Data.Sync
{
	public class SyncManager : Manager<SyncManager>
	{
		public LoginProvider customProvider;
		public LoginProvider facebookProvider;
		public ServerProvider server;

		protected RemoteUser currentUser;
		 


		protected override void Awake()
		{
			base.Awake();

			if(customProvider != null)
			{
				customProvider.OnLoginSuccessfull	+= OnCustomLogin;
				customProvider.OnLogoutSuccessfull	+= OnCustomLogout;
			}

			if(facebookProvider != null) 
			{
				facebookProvider.OnLoginSuccessfull += OnFacebookLogin;
				facebookProvider.OnLogoutSuccessfull+= OnFacebookLogout;
			}

			server.OnUserReceived += OnUserReceived;
			server.OnDataReceived += OnDataReceived;
			server.OnDataUpdated += OnDataUpdated;
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
				//Cambio de usuario
				server.stopAndRemoveCurrentRequests();
				prepareLocalLogin(customUserId, facebookUserId);
				server.getUniqueUser(customUserId, facebookUserId);
			}
			else
			{
				//Este id pertenece al current user??
				if(!customIdBelongsToCurrentUser())
				{
					if(!facebookProvider.isLoggedIn)
					{
						//Cambio de usuario
						server.stopAndRemoveCurrentRequests();
						prepareLocalLogin(customUserId, facebookUserId);
						server.getUniqueUser(customUserId, facebookUserId);
					}
				}
			}
		}

		protected void OnFacebookLogin(string message)
		{
			if(!existCurrentUser())
			{
				//Cambio de usuario
				server.stopAndRemoveCurrentRequests();
				prepareLocalLogin(customUserId, facebookUserId);
				server.getUniqueUser(customUserId, facebookUserId);
			}
			else
			{
				//Este id pertenece al current user?
				if(!facebookIdBelongsToCurrentUser())
				{
					//TODO: Checar cuando se relaciona el usuario de facebook con el custom
					//Cambio de usuario
					server.stopAndRemoveCurrentRequests();
					prepareLocalLogin(customUserId, facebookUserId);
					server.getUniqueUser(customUserId, facebookUserId);
				}
			}
		}

		/**
		 * Un login local que se hace antes de hacerlo al servicio remoto para permitir una experiencia fluida
		 **/ 
		protected virtual void prepareLocalLogin(string customUserId, string facebookUserId)
		{
			
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

		public string customUserId
		{
			get{return customProvider != null ? customProvider.userId : "";}
		}

		public string facebookUserId
		{
			get{return facebookProvider != null ? facebookProvider.userId : "";}
		}

		protected void OnCustomLogout(string message)
		{
			if(facebookProvider == null || !facebookProvider.isLoggedIn)
			{
				//Ya no hay usuario
				logout();
			}
		}

		protected void OnFacebookLogout(string message)
		{
			if(customProvider == null || !customProvider.isLoggedIn)
			{
				//Ya no hay usuario
				logout();
			}
		}

		protected virtual void logout()
		{
			server.stopAndRemoveCurrentRequests();
			currentUser = null;
		}

		protected virtual void OnUserReceived(RemoteUser user)
		{
			currentUser = user;
		}

		protected virtual void OnDataReceived(string fullData)
		{
			
		}

		protected virtual void OnDataUpdated(string updatedData)
		{
			
		}
	}
}