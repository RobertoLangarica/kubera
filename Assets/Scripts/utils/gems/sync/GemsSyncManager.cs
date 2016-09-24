using UnityEngine;
using System.Collections;
using Data.Sync;

namespace utils.gems.sync
{
	public class GemsSyncManager : SyncManager
	{

		/**
		 * NOTA: Este manager no hace login solo recibe las credenciales
		 * el login es web.
		 **/
		//Credenciales
		private string accesToken;


		public GemManager localData;



		protected override void Awake()
		{
			base.Awake();
		}


		public void SetCredentials(string id, string token)
		{
			accesToken = token;
			currentUser = new RemoteUser();
			currentUser.id = id;

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
		 * Cuando se recibe el usuario (sin informacion solo sus identificaciones)
		 **/ 
		/*protected override void OnUserReceived (RemoteUser remoteUser)
		{
			base.OnUserReceived (remoteUser);


			localData.changeCurrentuser(currentUser.id);

			//TODO identificar cuando el usuario apenas se registro
			if(remoteUser.newlyCreated)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Creating remote user.");
				}

				//TODO las gemas no se suben
			}
			else
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Getting data from remote user.");
				}

				//Nos traemos las gemas del usuario
				//Las gemas no se sincronizan y la version es solo por compatibilidad con el codigo
				server.getUserData(currentUser.id, localData.currentUser.version, false);
			}
		}*/


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

			//TODO hay cosas que subir??
			//Necesita subirse?
			/*if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}*/
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