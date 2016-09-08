﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using Data.Sync;
using Kubera.Data.Remote.PFResponseData;
using Kubera.Data.Remote;

namespace Kubera.Data.Sync
{
	public class KuberaSyncManger : SyncManager
	{
		public Action<PFLeaderboardData> OnLeaderboardObtained;

		public KuberaDataManager localData;

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
		protected override void logout ()
		{
			base.logout ();
			localData.setUserAsAnonymous();

			if(_mustShowDebugInfo)
			{
				Debug.Log("Logged OUT");
			}
		}

		/**
		 * Cuando se recibe el usuario (sin informacion solo sus identificaciones)
		 **/ 
		protected override void OnUserReceived (GameUser user)
		{
			base.OnUserReceived (user);

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

			if(user.newlyCreated)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Creating remote user.");
				}

				//Datos nuevos DEPRECATED
				//server.createUserData<KuberaUser>(currentUser.id, userToPlayFabJSON(localData.currentUser), localData.currentUser.clone());

				//Hacemos un update normal del usuario
				updateData(localData.getUserDirtyData());
			}
			else
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Getting data from remote user.");
				}
				//Nos traemos los datos de este usuario
				server.getUserData(currentUser.id, localData.getCSVKeysToQuery(), localData.currentUser.PlayFab_dataVersion);	
			}
		}


		/**
		 * Recibiendo la data del usuario
		 **/
		protected override void OnDataReceived (string fullData)
		{
			base.OnDataReceived (fullData);

			localData.diffUser(JsonUtility.FromJson<KuberaUser>(fullData), true);

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
				Debug.Log("To update: \n"+userToJSON(dirtyUser));
			}

			//Si no hay usuario remoto entonces no hay nada que actualizar
			if(existCurrentUser())
			{
				server.updateUserData(currentUser.id, userToJSON(dirtyUser), dirtyUser);
			}
		}

		/**
		 * Obtiene el leaderboard de nivel especificado
		 **/ 
		public void getLevelLeaderboard(string levelId, int maxResultsCount = 10)
		{
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

		/**
		 * Usuario en el formato JSON que necesite el servicio remoto
		 **/ 
		public string userToJSON(KuberaUser user)
		{
			return JsonUtility.ToJson(user);
		}
	}
		
}