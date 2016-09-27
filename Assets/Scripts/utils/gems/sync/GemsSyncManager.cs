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

		//TODO como se va manejar el logout
		/**
		 * Cuando se hace logout en local hay responder a ese cambio y hay que hacer logout remoto
		 **/ 
		protected override void logout ()
		{
			base.logout ();
			localData.setUserAsAnonymous();

			if(_mustShowDebugInfo)
			{
				Debug.Log("Logged OUT");
			}
		}

		public void getGems()
		{
			server.getUserData(currentUser.id, false);
		}
			
		/**
		 * Recibiendo la data del usuario
		 **/
		protected override void OnDataReceived (string fullData)
		{
			base.OnDataReceived (fullData);


			localData.diffUser(JsonUtility.FromJson<UserGem>(fullData), true);

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}
		}


		/**
		 * Se recibe despues de mandar consumir gemas
		 **/ 
		protected override void OnDataUpdated (string updatedData)
		{
			base.OnDataUpdated (updatedData);

			UserGem updatedUser = JsonUtility.FromJson<UserGem>(updatedData);

			//Se actualizo algo en el server
			localData.diffUser(updatedUser, true);//Ignoramos la version

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			//TODO nada se sube??
			/*
			//Necesita subirse?
			if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}*/
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
				//TODO mandar consumir gemmas en un update al server
				//server.updateUserData<KuberaUser>(currentUser.id, dirtyUser);
			}
		}
	}
}