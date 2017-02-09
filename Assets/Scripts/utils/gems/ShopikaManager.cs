using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using utils.gems.sync;

namespace utils.gems
{
	public class ShopikaManager : LocalDataManager<MultipleUserGem>
	{
		public int gemsToGiveLocally = 25;//TODO eliminar porque es un parche para SHOPIKA
		public float localGemsWaitMinutes = 0.5f;//TODO eliminar porque es un parche para SHOPIKA
		protected DateTime localGemsLastDate;//TODO eliminar porque es un parche para SHOPIKA

		public bool _freeTestMode = false;

		public Action<int> OnGemsUpdated;
		public ShopikaSyncManager syncManager;


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

			//TODO esto se va porque es un parche para SHOPIKA
			if(!string.IsNullOrEmpty(currentData.lastTimeLocalGemsGranted))
			{
				localGemsLastDate = DateTime.ParseExact (currentData.lastTimeLocalGemsGranted,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
			}

			if(!string.IsNullOrEmpty(currentData.lastUsedId))
			{
				UserGem prevUser = currentData.getUserById(currentData.lastUsedId);

				if(prevUser != null && !string.IsNullOrEmpty(prevUser.accesToken) && !string.IsNullOrEmpty(prevUser._id) && !string.IsNullOrEmpty(prevUser.shareCode))
				{
					OnUserLoggedIn(prevUser._id, prevUser.accesToken, prevUser.shareCode);
				}
			}
		}

		/**
		 * Cuando se recibe la data desde el web-component
		 **/ 
		public void OnUserLoggedIn(string id, string token, string shareCode)
		{
			//Cambio de usuario
			changeCurrentuser(id);
			currentUser.accesToken = token;
			currentUser.shareCode = shareCode;
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

			//HARDCODING para ue este cambio este en android e iOS
			//TODO cambio temporal para SHOPIKA
			/*#if UNITY_IOS || UNITY_EDITOR
			return (currentUser.gems + currentData.localGems) >= amount;
			#endif*/
			return (currentUser.gems + currentData.localGems) >= amount;

			//Descomentar esta linea cuando se eliminen las gemas locales
			//return currentUser.gems >= amount;
		}

		public void tryToConsumeGems(int amount)
		{
			if(_freeTestMode)
			{
				return;	
			}

			//HARDCODING para ue este cambio este en android e iOS
			//TODO cambio temporal para SHOPIKA
			/*#if UNITY_IOS || UNITY_EDITOR
			if(currentData.localGems > 0)
			{
				currentData.localGems -= amount;

				//suficientes
				if(currentData.localGems >= 0)
				{
					//suficientes y no hay nada mas que hacer
					afterGemsModified();
					saveLocalData(false);
					return;
				}
				else
				{
					//insuficientes y consumimos las restantes
					amount = -currentData.localGems;
				}
			}
			#endif*/
			//TODO cambio temporal para SHOPIKA
			if(currentData.localGems > 0)
			{
				currentData.localGems -= amount;

				//suficientes
				if(currentData.localGems >= 0)
				{
					//suficientes y no hay nada mas que hacer
					afterGemsModified();
					saveLocalData(false);
					return;
				}
				else
				{
					//insuficientes y consumimos las restantes
					amount = -currentData.localGems;
				}
			}

			//En local
			currentUser.gems -= amount;

			//TODO guardar request en el disco para que se hagan si no son exitosos

			//remoto
			syncManager.consumeGems(amount);

			afterGemsModified();

			saveLocalData(false);
		}


		public void giveLocalAnonymousGems()
		{
			//TODO esta funcion no debe existir y hay que quitarla
			currentData.localGems += gemsToGiveLocally;
			localGemsLastDate = DateTime.UtcNow;
			currentData.lastTimeLocalGemsGranted = localGemsLastDate.ToString ("dd-MM-yyyy HH:mm:ss");


			saveLocalData(false);

			afterGemsModified();
		}
			
		public string remainingTimeToGiveGemsString()
		{
			//TODO esta funcion no debe existir y hay que quitarla
			if(!string.IsNullOrEmpty(currentData.lastTimeLocalGemsGranted))
			{
				TimeSpan elapsedSpan = DateTime.UtcNow - localGemsLastDate;

				double waitTimeInSeconds = (double)(localGemsWaitMinutes*60);
				if(elapsedSpan.TotalSeconds < waitTimeInSeconds)
				{
					double remainingSeconds = waitTimeInSeconds-elapsedSpan.TotalSeconds;
					int minutes = ((int)remainingSeconds/60);
					int seconds = (int)remainingSeconds - (minutes*60);

					return minutes.ToString("00")+":"+seconds.ToString("00");
				}
			}
				
			return "00:00";
		}

		public bool canGiveLocalGems()
		{
			//TODO esta funcion no debe existir y hay que quitarla
			if(!string.IsNullOrEmpty(currentData.lastTimeLocalGemsGranted))
			{
				TimeSpan elapsedSpan = DateTime.UtcNow - localGemsLastDate;

				int waitTimeInSeconds = (int)(localGemsWaitMinutes*60);
				return elapsedSpan.TotalSeconds >= waitTimeInSeconds;
			}

			return true;
		}



		public void registerInvite(string invitedFacebookId,string inviterFacebookId, string invitedEmail = "", string invitedPhoneNumber = "", string invitedId = "", string inviterId = "")
		{
			syncManager.registerInvite(invitedFacebookId,inviterFacebookId,invitedEmail,invitedPhoneNumber,invitedId,inviterId);
		}

		public int currentGems
		{
			get
			{
				return currentUser.gems + currentData.localGems;
			}
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
				OnGemsUpdated(currentUser.gems + currentData.localGems);
				//OnGemsUpdated(currentUser.gems);
			}
		}
	}
}