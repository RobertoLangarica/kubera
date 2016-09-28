using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data.Remote;
using Data.Sync;
using Kubera.Data;
using Kubera.Data.Remote.PFResponseData;
using Kubera.Data.Remote.GSResponseData;

namespace Kubera.Data.Remote
{
	public class KuberaProvider : ServerProvider 
	{
		public bool _mustShowDebugInfo = false;
		public string GS_API_KEY = "f299147zFN10";
		/// <summary>
		/// preview or live
		/// </summary>
		public string GS_STAGE = "preview";
		public string GS_CREDENTIAL = "device";
		public string GS_SECRET = "smdB4Xb5uqfprRTaDNm8aquzW0ceptqF";

		public string GS_PATH = "https://{API_KEY}.{GS_STAGE}.gamesparks.net/rs/{GS_CREDENTIAL}/{GS_SECRET}";


		public Action<PFLeaderboardData> OnLeaderboardObtained;
		public string TITLE_ID = "74A";

		private bool isLogged = false;
		private GSLoginRequest loginRequest;
		private GSGetUserRequest mainUpdateRequest;
		private string currentFacebookId;

		public string getPath()
		{
			return GS_PATH.Replace("{API_KEY}",GS_API_KEY).Replace("{GS_STAGE}",GS_STAGE)
				.Replace("{GS_CREDENTIAL}",GS_CREDENTIAL).Replace("{GS_SECRET}",GS_SECRET);
		}
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
			loginRequest = queue.getComponentAttachedToGameObject<GSLoginRequest>("GameSparkFBLogin");
			loginRequest.showDebugInfo = _mustShowDebugInfo;
			loginRequest.id = "login_"+currentFacebookId;
			loginRequest.persistAfterFailed = true;
			loginRequest.facebookAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString; 
			loginRequest.initialize(getPath());
			loginRequest.OnComplete += OnRemoteLoginComplete;

			addRequest(loginRequest,true);
		}
			
		private void OnRemoteLoginComplete(string request_id)
		{
			isLogged = true;
				
			if(OnUserReceived != null)
			{
				//Creamos el usuario a manipular
				RemoteUser user = new RemoteUser();
				user.facebookId = currentFacebookId;
				user.customId = string.Empty;
				user.id = loginRequest.data.userId;
				user.newlyCreated = loginRequest.data.newPlayer;

				OnUserReceived(user);
			}
		}


		/**
		 * Para obtener los datos del usuario
		 **/ 
		public override void getUserData (string id, int aboveVersion, bool saveAsMainRequest = false)
		{
			GSGetUserRequest request = queue.getComponentAttachedToGameObject<GSGetUserRequest>("GS_GetUserData");

			if(saveAsMainRequest)
			{
				mainUpdateRequest = request;	
			}

			request.id = "get_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.playerId = id;
			request.persistAfterFailed = true;
			request.showDebugInfo = _mustShowDebugInfo;
			request.aboveVersion = aboveVersion;
			request.initialize(getPath());
			request.OnComplete += OnUserDataObtained;

			addDependantRequest(request,saveAsMainRequest);
		}

		private void OnUserDataObtained(string request_id)
		{
			if(mainUpdateRequest != null && mainUpdateRequest.id == request_id)
			{
				mainUpdateRequest = null;
			}

			if(OnDataReceived != null)
			{
				GSGetUserRequest request = (GSGetUserRequest)getRequestById(request_id);


				//Hacemos un usuario para el diff
				KuberaUser remoteUser = new KuberaUser(loginRequest.data.userId);
				remoteUser.remoteDataVersion = request.data.version;
				remoteUser.levels = request.data.levels;
				remoteUser.maxLevelReached = request.data.maxLevelReached;

				OnDataReceived(JsonUtility.ToJson(remoteUser));
			}
		}

		public override void updateUserData<T> (string id, T serializableData)
		{
			GSUpdateUserRequest request = queue.getComponentAttachedToGameObject<GSUpdateUserRequest>("GS_UpdateUserData");

			request.id = "update_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.showDebugInfo = _mustShowDebugInfo;
			request.persistAfterFailed = true;
			request.playerId = id;
			request.jsonToUpdate = convertUserToJSON(serializableData as KuberaUser);
			request.initialize(getPath());
			request.OnComplete += OnUserDataUpdated;

			addDependantRequest(request,false);


		}

		private void OnUserDataUpdated(string request_id)
		{
			if(OnDataUpdated != null)
			{
				GSUpdateUserRequest request = (GSUpdateUserRequest)getRequestById(request_id);

				//Para que se desmarquen como sucios los datos locales hacemos un diff
				KuberaUser remoteUser = new KuberaUser(request.playerId);
				remoteUser.remoteDataVersion = request.data.version;
				remoteUser.levels = request.data.levels;
				remoteUser.maxLevelReached = request.data.maxLevelReached;

				OnDataUpdated(JsonUtility.ToJson(remoteUser));
			}
		}

		private string convertUserToJSON(KuberaUser user)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("{");
			builder.Append("\"_id\":\""+user._id+"\"");
			builder.Append(",\"maxLevelReached\":"+user.maxLevelReached.ToString());

			builder.Append(",\"levels\":{");

			LevelData level;
			for(int i = 0; i < user.levels.Count; i++)
			{
				level = user.levels[i];
				if(i > 0)
				{
					builder.Append(",");
				}

				builder.Append("\""+level._id+"\":"+JsonUtility.ToJson(level));
			}

			builder.Append("}");
			builder.Append("}");

			return builder.ToString();
		}

		public override void getLeaderboardData (string id, string leaderboardName, int maxResultsCount)
		{
			/*PFGetLeaderboardRequest request = queue.getComponentAttachedToGameObject<PFGetLeaderboardRequest>("PF_LeaderboardRequest");
			request.id = "leaderboard_"+id+"_"+leaderboardName+"_"+UnityEngine.Random.Range(0,99999).ToString("00");
			request.showDebugInfo = _mustShowDebugInfo;
			request.persistAfterFailed = true;
			request.initialize(TITLE_ID, id, leaderboardName, maxResultsCount, sessionTicket);
			request.OnComplete += OnLeaderboardDataObtained;

			addLoginDependantRequest(request,false);*/
		}

		private void OnLeaderboardDataObtained(string request_id)
		{
			/*if(OnLeaderboardObtained != null)
			{
				PFGetLeaderboardRequest request = (PFGetLeaderboardRequest)getRequestById(request_id);
				request.data.data.name = request.statisticUsed;

				OnLeaderboardObtained(request.data.data);
			}*/
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

		private void addDependantRequest(BaseRequest request, bool isPriority = false)
		{
			if(isLogged)
			{
				//El usuario se esta actualizando por primera vez?
				if(mainUpdateRequest != null && mainUpdateRequest.id != request.id)
				{
					//request dependiente de que se termine de actualizar el usuario
					mainUpdateRequest.dependantRequests.Add(request);
				}
				else
				{
					//request normal
					addRequest(request, isPriority);
				}

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
			mainUpdateRequest = null;
		}
	}	
}