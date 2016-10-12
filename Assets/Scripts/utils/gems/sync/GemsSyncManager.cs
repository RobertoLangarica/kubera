using UnityEngine;
using System.Collections;
using Data.Sync;

using utils.gems.remote;

namespace utils.gems.sync
{
	public class GemsSyncManager : SyncManager<GemsSyncManager>
	{

		/**
		 * NOTA: Este manager no hace login solo recibe las credenciales
		 * el login es web.
		 **/
		public GemsManager localData;

		protected override void Awake()
		{
			base.Awake();

			((ShopikaProvider)server).OnBadToken = OnBadCredentials;
		}


		public void SetCredentials(string id, string token)
		{
			currentUser = new RemoteUser();
			currentUser.id = id;

			((ShopikaProvider)server).userId = id;
			((ShopikaProvider)server).token = token;

			//Cambio de usuario
			server.stopAndRemoveCurrentRequests();
		}

		private void OnBadCredentials()
		{

			if(isGettingData)
			{
				isGettingData = false;

				if(OnDataRetrievedFailure)
				{
					OnDataRetrievedFailure();	
				}
			}

			afterLogout();
		}
			
		/**
		 * Cuando se hace logout en local hay responder a ese cambio y hay que hacer logout remoto
		 **/ 
		protected override void afterLogout ()
		{
			//Olvidamos las credenciales y limpiamos el usuairo con malas credenciales
			localData.currentUser.clear();
			localData.getCurrentData().lastUsedId = "";
			localData.saveLocalData(false);

			//Detenemos lo que este haciendo el server provider y quitamos el currentUser
			base.afterLogout ();

			//La local data se va en modo anonimo
			localData.setUserAsAnonymous();

			if(_mustShowDebugInfo)
			{
				Debug.Log("Logged OUT");
			}
		}

		public void getGems()
		{
			isGettingData = true;
			server.getUserData(currentUser.id, false);
		}
			
		/**
		 * Recibiendo la data del usuario
		 **/
		protected override void OnDataReceived (string fullData)
		{
			base.OnDataReceived (fullData);

			localData.diffUser(JsonUtility.FromJson<UserGem>(fullData), true);
			isGettingData = false;

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			if(OnDataRetrieved != null)
			{
				OnDataRetrieved();
			}
		}


		/**
		 * Se recibe despues de mandar consumir gemas
		 **/ 
		protected override void OnDataUpdated (string fullData)
		{
			base.OnDataUpdated (fullData);

			//Se actualizo algo en el server
			localData.diffUser(JsonUtility.FromJson<UserGem>(fullData), true);//Ignoramos la version

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}
		}

		/**
		 * Manda consumir gemas
		 **/ 
		public void consumeGems(int gemsToConsume)
		{
			if(_mustShowDebugInfo)
			{
				Debug.Log("To consume: "+gemsToConsume.ToString());
			}
				
			//Si no hay usuario remoto entonces no hay nada que actualizar
			if(existCurrentUser())
			{
				server.updateUserData(currentUser.id, gemsToConsume.ToString());
			}
		}
	}
}