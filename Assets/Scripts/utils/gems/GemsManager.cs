using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using utils.gems.sync;

namespace utils.gems
{
	public class GemsManager : LocalDataManager<MultipleUserGem>
	{
		public bool _freeTestMode = false;

		public Action<int> OnGemsUpdated;
		public GemsSyncManager syncManager;


		public UserGem currentUser{get{return currentData.getUserById(currentUserId);}}

		protected override void Start ()
		{
			base.Start ();

			if(_freeTestMode)
			{
				Debug.Log("<color=red>Modo test: GEMAS GRATIS</color>");
			}
		}

		protected override void fillDefaultData ()
		{
			base.fillDefaultData ();

			//Usuario anonimo
			UserGem anon = new UserGem(ANONYMOUS_USER);
			cleanToAnonymousData(anon);
			currentData.users.Add(anon);
		}
			
		protected override void afterFirstRead ()
		{
			base.afterFirstRead ();

			if(!string.IsNullOrEmpty(currentData.lastUsedId))
			{
				UserGem prevUser = currentData.getUserById(currentData.lastUsedId);

				if(prevUser != null && !string.IsNullOrEmpty(prevUser.accesToken) && !string.IsNullOrEmpty(prevUser._id))
				{
					OnUserLoggedIn(prevUser._id, prevUser.accesToken);
				}
			}
		}

		/**
		 * Cuando se recibe la data desde el web-component
		 **/ 
		public void OnUserLoggedIn(string id, string token)
		{
			//Cambio de usuario
			changeCurrentuser(id);
			currentUser.accesToken = token;
			saveLocalData(false);

			//Avisamos al syncManager
			syncManager.SetCredentials(id,token);
			syncManager.getGems();
		}

		/**
		 * Cuando el web-component indica que las gemas se cambiaron
		 **/ 
		public void OnGemsRemotleyChanged()
		{
			syncManager.getGems();
		}

		public bool isPossibleToConsumeGems(int amount)
		{
			if(_freeTestMode)
			{
				return true;
			}

			return currentUser.gems >= amount;
		}

		public void tryToConsumeGems(int amount)
		{
			if(_freeTestMode)
			{
				return;	
			}

			//En local
			currentUser.gems -= amount;

			//TODO guardar request en el disco para que se hagan si no son exitosos

			//remoto
			syncManager.consumeGems(amount);

			afterGemsModified();
		}

		public int currentGems
		{
			get{return currentUser.gems;}
		}

		public override void changeCurrentuser (string newUserId)
		{
			if(currentUserId == newUserId)
			{
				//No hay cambios que hacer
				return;	
			}
				
			//no se guardan gemas locales
			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					//No existe
					//Creamos un nuevo usuario y lo agregamos
					currentData.users.Add(new UserGem(newUserId));
				}
				else
				{
					//usamos el existente y no hacemos diff con el anonimo
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					//No existe
					//Creamos un nuevo usuario y lo agregamos
					currentData.users.Add(new UserGem(newUserId));
				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
				}
			}

			//El usuario actual se limpia
			currentUser.clear();

			base.changeCurrentuser (newUserId);

			//El existente sin gemas porque las queremos remotas todo el tiempo
			currentUser.gems = 0;

			//Guardamos este como el ultimo usuario
			currentData.lastUsedId = newUserId;

			saveLocalData(false);
		}

		private void cleanToAnonymousData(UserGem user)
		{
			user.clear();
			user._id = ANONYMOUS_USER;
		}

		public void diffUser(UserGem remoteUser, bool ignoreVersion = false)
		{
			if(currentUserId != remoteUser._id)
			{
				Debug.Log("Se recibieron datos de otro usuario: "+currentUserId+","+ remoteUser._id);	
				return;
			}
				
			bool modified = currentUser.compareAndUpdate(remoteUser, ignoreVersion);
			saveLocalData(false);

			if(_mustShowDebugInfo)
			{
				Debug.Log("Sincronizadas: "+currentUser.gems + " para: "+currentUser._id);
			}

			if(modified)
			{
				afterGemsModified();
			}
		}

		private void afterGemsModified()
		{
			if(OnGemsUpdated != null)
			{
				OnGemsUpdated(currentUser.gems);
			}
		}
	}
}