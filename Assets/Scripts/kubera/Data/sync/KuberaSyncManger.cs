using UnityEngine;
using System;
using System.Collections;
using Data.Sync;
using Kubera.Data.Remote.PFResponseData;
using Kubera.Data.Remote;

namespace Kubera.Data.Sync
{
	public class KuberaSyncManger : SyncManager<KuberaSyncManger>
	{
		public Action<PFLeaderboardData> OnLeaderboardObtained;

		public DataManagerKubera localData;
		public int freeLifesAfterSignIn = 2;

		protected override void Awake()
		{
			base.Awake();


			((KuberaProvider)server).OnLeaderboardObtained += OnLeaderboardDataObtained;
		}

		/**
		 * Login en local para que el usuario pueda comenzar a jugar aun cuando no se a hecho un login al servicio remoto 
		 **/
		protected override void prepareLocalLogin (string customUserId, string facebookUserId)
		{
			localData.temporalUserChangeWithFacebookId(facebookUserId);
		}

		/**
		 * Cuando se hace logout en local hay responder a ese cambio y hay que hacer logout remoto
		 **/ 
		protected override void afterLogout ()
		{
			base.afterLogout ();
			localData.setUserAsAnonymous();

			if(_mustShowDebugInfo)
			{
				Debug.Log("Logged OUT");
			}
		}

		/**
		 * Cuando se recibe el usuario (sin informacion solo sus identificaciones)
		 **/ 
		protected override void OnUserReceived (RemoteUser remoteUser)
		{
			base.OnUserReceived (remoteUser);

			//Exsitia un usuario creado con id de facebook temporal?
			KuberaUser kUser = localData.getCurrentData().getUserById(currentUser.facebookId);
			if(kUser != null)
			{
				//Le asignamos su id de verdad
				kUser._id = currentUser.id;
			}

			localData.changeCurrentuser(currentUser.id);

			//En caso de que se creo un nuevo usuario se le asigna su facebookId
			if(localData.currentUser.facebookId != currentUser.facebookId)
			{
				localData.currentUser.facebookId = currentUser.facebookId;
				localData.saveLocalData(false);
			}

			if(remoteUser.newlyCreated)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Creating remote user.");
				}

				KuberaAnalytics.GetInstance ().registerFaceBookLogin ();
				if(!localData.currentUser.remoteLifesGranted)
				{
					if(_mustShowDebugInfo)
					{
						Debug.Log("Granted Lifes: " + freeLifesAfterSignIn.ToString());	
					}

					localData.giveUserLifes(freeLifesAfterSignIn);
					localData.currentUser.isDirty = localData.currentUser.updateremoteLifesGranted(true) || localData.currentUser.isDirty;
				}
					
				//Hacemos un update normal del usuario
				//updateData(localData.getUserDirtyData());

				isGettingData = true;
				server.getUserData(currentUser.id, localData.currentUser.remoteDataVersion, true);
			}
			else
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Getting data from remote user.");
				}

				isGettingData = true;
				//Nos traemos los datos de este usuario
				server.getUserData(currentUser.id, localData.currentUser.remoteDataVersion, true);
			}
		}


		/**
		 * Recibiendo la data del usuario
		 **/
		protected override void OnDataReceived (string fullData)
		{
			base.OnDataReceived (fullData);

			if(_mustShowDebugInfo)
			{
				Debug.Log("BeforeDiff:\n"+fullData);
			}

			KuberaUser toDiff = JsonUtility.FromJson<KuberaUser>(fullData);

			//Solo se hace diff si no llego vacio del server
			if(toDiff.remoteDataVersion != -1)
			{
				localData.diffUser(toDiff, true);	
			}

			isGettingData = false;

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			if(!localData.currentUser.remoteLifesGranted)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Granted Lifes: " + freeLifesAfterSignIn.ToString());	
				}

				localData.giveUserLifes(freeLifesAfterSignIn);
				localData.currentUser.isDirty = localData.currentUser.updateremoteLifesGranted(true) || localData.currentUser.isDirty;
			}

			//Necesita subirse?
			if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}

 			if(OnDataRetrieved != null)
			{
				OnDataRetrieved();
			}
		}
			
		/**
		 * Una vez actualizados los datos hay que actualizar el estado local de los datos
		 **/ 
		protected override void OnDataUpdated (string updatedData)
		{
			base.OnDataUpdated (updatedData);

			KuberaUser updatedUser = JsonUtility.FromJson<KuberaUser>(updatedData);

			//Se actualizo algo en el server
			localData.diffUser(updatedUser, true);//Ignoramos la version

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			//Necesita subirse?
			if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}
		}

		/**
		 * Manda actualizar los datos del usuario
		 **/ 
		public void updateData(KuberaUser dirtyUser)
		{
			if(_mustShowDebugInfo)
			{
				Debug.Log("To update: \n"+JsonUtility.ToJson(dirtyUser));
			}

			//Si no hay usuario remoto entonces no hay nada que actualizar
			if(existCurrentUser())
			{
				server.updateUserData<KuberaUser>(currentUser.id, dirtyUser);
			}
		}

		/**
		 * Obtiene el leaderboard de nivel especificado
		 **/ 
		public void getLevelLeaderboard(string levelId, int maxResultsCount = 10)
		{
			return;

			//Si no hay usuario remoto entonces no hay nada que actualizar
			if(existCurrentUser())
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Leaderboard: level_"+levelId);	
				}

				server.getLeaderboardData(currentUser.id, "level_"+levelId, maxResultsCount);
			}
		}

		private void OnLeaderboardDataObtained(PFLeaderboardData leaderboard)
		{
			string result = "Leaderboard result: {";

			for(int i = 0; i < leaderboard.Leaderboard.Count; i++)
			{
				result += "\n" + JsonUtility.ToJson(leaderboard.Leaderboard[1]);
			}

			result+="\n}";

			Debug.Log(result);
		}
	}
		
}