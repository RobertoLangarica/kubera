using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data.Remote;
using Data.Sync;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class KuberaProvider : ServerProvider 
	{
		public Action<PFLeaderboardData> OnLeaderboardObtained;

		public string TITLE_ID = "74A";

		private bool isLogged = false;
		private PFLoginRequest loginRequest;
		PFCreateUserRequest<Kubera.Data.KuberaUser> creatingUserRequest;
		private string currentFacebookId;
		private string sessionTicket;

		/**
		 * Obtiene el usuario unico desde el servicio remoto (lo creo si es necesario)
		 **/ 
		public override void getUniqueUser (string customId, string facebookId)
		{
			currentFacebookId = facebookId;
			doRemoteLogin();
		}

		/**
		 * Login remoto con las credenciales de facebook del usuario
		 **/ 
		private void doRemoteLogin()
		{
			//Primero hay que hacer login
			loginRequest = queue.getComponentAttachedToGameObject<PFLoginRequest>("PlayFabFBLogin");
			loginRequest.id = "login_"+currentFacebookId;
			loginRequest.persistAfterFailed = true;
			loginRequest.initialize(TITLE_ID,Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
			loginRequest.OnComplete += OnRemoteLoginComplete;

			addRequest(loginRequest,true);
		}
			
		private void OnRemoteLoginComplete(string request_id)
		{
			isLogged = true;
			sessionTicket = loginRequest.data.data.SessionTicket;
				
			if(OnUserReceived != null)
			{
				//Creamos el usuario a manipular
				GameUser user = new GameUser();
				user.facebookId = currentFacebookId;
				user.customId = string.Empty;
				user.id = loginRequest.data.data.PlayFabId;
				user.newlyCreated = loginRequest.data.data.NewlyCreated;

				OnUserReceived(user);
			}
		}

		/**
		 * [DEPRECATED]
		 * En caso de que sea un usuario que no existe en el servidor hay que subir los datos locales por primera vez
		 **/ 
		public override void createUserData<KuberaUser>(string id, string jsonData, KuberaUser objectToSave)
		{
			//DEPRECATED en Kubera
			creatingUserRequest = queue.getComponentAttachedToGameObject<PFCreateUserRequest<Kubera.Data.KuberaUser>>("PF_UserCreate");
			creatingUserRequest.id = "create_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			creatingUserRequest.showDebugInfo = false;
			creatingUserRequest.persistAfterFailed = true;
			creatingUserRequest.initialize(TITLE_ID,jsonData, sessionTicket, objectToSave);
			creatingUserRequest.OnComplete += afterUserCreated;

			addLoginDependantRequest(creatingUserRequest,true);
		}

		/**
		 * [DEPRECATED]
		 **/ 
		private void afterUserCreated(string request_id)
		{
			//DEPRECATED en Kubera
			if(OnDataReceived != null)
			{
				PFCreateUserRequest<KuberaUser> request = (PFCreateUserRequest<KuberaUser>)getRequestById(request_id);

				//Hacemos un usuario para el diff
				KuberaUser remoteUser = creatingUserRequest.dataSended;
				remoteUser.id = loginRequest.data.data.PlayFabId;
				remoteUser.PlayFab_dataVersion = request.data.data.DataVersion;

				OnDataReceived(JsonUtility.ToJson(remoteUser));
			}

			//Para que se dejen de agregar dependencias
			creatingUserRequest = null;
		}

		/**
		 * Para obtener los datos del usuario
		 **/ 
		public override void getUserData (string id, string extraData, int aboveVersion)
		{
			PFGetUserRequest request = queue.getComponentAttachedToGameObject<PFGetUserRequest>("PF_GetUserData");
			request.id = "get_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.persistAfterFailed = true;
			request.showDebugInfo = false;
			request.initialize(TITLE_ID, extraData, aboveVersion, sessionTicket);
			request.OnComplete += OnUserDataObtained;

			addLoginDependantRequest(request,false);
		}

		private void OnUserDataObtained(string request_id)
		{
			if(OnDataReceived != null)
			{
				PFGetUserRequest request = (PFGetUserRequest)getRequestById(request_id);

				//Hacemos un usuario para el diff
				KuberaUser remoteUser = new KuberaUser(loginRequest.data.data.PlayFabId);


				if(request.data.data.Data.ContainsKey("version"))
				{
					remoteUser.version = int.Parse(((Dictionary<string, object>)request.data.data.Data["version"])["Value"].ToString());
				}

				remoteUser.PlayFab_dataVersion = request.data.data.DataVersion;

				foreach (string key in request.data.data.Data.Keys)
				{
					if(key.Contains("world_"))
					{
						//Debug.Log(((Dictionary<string, object>)request.data.data.Data[key])["Value"]);
						remoteUser.worlds.Add(JsonUtility.FromJson<WorldData>(((Dictionary<string, object>)request.data.data.Data[key])["Value"].ToString()));	
					}
				}

				OnDataReceived(JsonUtility.ToJson(remoteUser));
			}
		}

		public override void updateUserData<KuberaUser> (string id, string jsonData, KuberaUser objectToSave)
		{
			PFUpdateDataRequest request = queue.getComponentAttachedToGameObject<PFUpdateDataRequest>("PF_UpdateUserData");
			request.id = "update_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.showDebugInfo = false;
			request.persistAfterFailed = true;
			request.initialize(TITLE_ID,jsonData, sessionTicket, objectToSave);
			request.OnComplete += OnUserDataUpdated;

			addLoginDependantRequest(request,false);
		}

		private void OnUserDataUpdated(string request_id)
		{
			if(OnDataUpdated != null)
			{
				PFUpdateDataRequest request = (PFUpdateDataRequest)getRequestById(request_id);

				//Para que se desmarquen como sucios los datos locales hacemos un diff
				KuberaUser remoteUser = request.dataSended as KuberaUser;
				remoteUser.id = loginRequest.data.data.PlayFabId;
				remoteUser.PlayFab_dataVersion = request.data.data.FunctionResult.DataVersion;

				OnDataUpdated(JsonUtility.ToJson(remoteUser));
			}
		}

		public override void getLeaderboardData (string id, string leaderboardName, int maxResultsCount)
		{
			PFGetLeaderboardRequest request = queue.getComponentAttachedToGameObject<PFGetLeaderboardRequest>("PF_LeaderboardRequest");
			request.id = "leaderboard_"+id+"_"+leaderboardName+"_"+UnityEngine.Random.Range(0,99999).ToString("00");
			request.persistAfterFailed = true;
			request.initialize(TITLE_ID, id, leaderboardName, maxResultsCount, sessionTicket);
			request.OnComplete += OnLeaderboardDataObtained;

			addLoginDependantRequest(request,false);
		}

		private void OnLeaderboardDataObtained(string request_id)
		{
			if(OnLeaderboardObtained != null)
			{
				PFGetLeaderboardRequest request = (PFGetLeaderboardRequest)getRequestById(request_id);
				request.data.data.name = request.statisticUsed;

				OnLeaderboardObtained(request.data.data);
			}
		}

		protected BaseRequest getRequestById(string id)
		{
			return requests.Find(item => item.id == id);
		}

		private void addRequest(BaseRequest request, bool isPriority = false)
		{
			requests.Add(request);
			if(isPriority)
			{
				queue.addPriorityRequest(request);
			}
			else
			{
				queue.addRequest(request);	
			}
		}

		private void addLoginDependantRequest(BaseRequest request, bool isPriority = false)
		{
			if(isLogged)
			{
				//Creacion DEPRECATED para Kubera

				//El usuario se esta creando
				/*if(creatingUserRequest != null && creatingUserRequest.id != request.id)
				{
					//request dependiente de que se termine de crear el usuario
					creatingUserRequest.dependantRequests.Add(request);
				}
				else
				{
					//request normal
					addRequest(request, isPriority);
				}*/

				//request normal
				addRequest(request, isPriority);
			}
			else if(loginRequest != null)//Si no hay usuario logeado entonces no hay request remotas
			{
				//As dependant
				loginRequest.dependantRequests.Add(request);
			}
		}

		public override void stopAndRemoveCurrentRequests ()
		{
			base.stopAndRemoveCurrentRequests ();
			isLogged = false;
			loginRequest = null;
			creatingUserRequest = null;
		}
	}	
}