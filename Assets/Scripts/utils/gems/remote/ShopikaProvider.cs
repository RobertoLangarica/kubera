using UnityEngine;
using System;
using System.Collections;

using Data.Remote;

using utils.gems.remote.request;
using utils.gems.remote.response;

namespace utils.gems.remote
{
	public class ShopikaProvider : ServerProvider 
	{
		public bool _mustShowDebugInfo = false;
		[HideInInspector]public string userId;
		[HideInInspector]public string token;

		public Action OnBadToken;

		private SPGetGemsRequest mainUpdateRequest;

		private string SP_API = "http://api.shopika.net/api";

		public override void getUserData (string id, bool saveAsMainRequest = false)
		{
			SPGetGemsRequest request = queue.getComponentAttachedToGameObject<SPGetGemsRequest>("GS_GetUserData");

			if(saveAsMainRequest)
			{
				mainUpdateRequest = request;	
			}

			request.id = "get_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.playerId = id;
			request.acces_token = token;
			request.persistAfterFailed = true;
			request.showDebugInfo = _mustShowDebugInfo;
			request.initialize(SP_API);
			request.OnComplete += OnUserDataObtained;
			request.OnFailed += OnRequestFailed;


			addDependantRequest(request,true);
		}

		private void OnUserDataObtained(string request_id)
		{
			if(mainUpdateRequest != null && mainUpdateRequest.id == request_id)
			{
				mainUpdateRequest = null;
			}

			if(OnDataReceived != null)
			{
				SPGetGemsRequest request = (SPGetGemsRequest)getRequestById(request_id);


				//Hacemos un usuario para el diff
				UserGem remoteUser = new UserGem(userId);
				remoteUser.gems = request.data.gemBalance;

				OnDataReceived(JsonUtility.ToJson(remoteUser));
			}
		}

		public override void updateUserData(string id, string gemsToConsume)
		{
			SPConsumeGemsRequest request = queue.getComponentAttachedToGameObject<SPConsumeGemsRequest>("GS_GetUserData");

			request.id = "consume_"+id+"_"+UnityEngine.Random.Range(0,99999).ToString("0000");
			request.playerId = id;
			request.acces_token = token;
			request.gemsToConsume = gemsToConsume;
			request.persistAfterFailed = true;
			request.showDebugInfo = _mustShowDebugInfo;

			request.initialize(SP_API);
			request.OnComplete += OnUserDataUpdated;
			request.OnFailed += OnRequestFailed;

			addDependantRequest(request,true);
		}

		private void OnUserDataUpdated(string request_id)
		{
			if(OnDataUpdated != null)
			{
				SPConsumeGemsRequest request = (SPConsumeGemsRequest)getRequestById(request_id);


				//Hacemos un usuario para el diff
				UserGem remoteUser = new UserGem(userId);
				remoteUser.gems = request.data.gemBalance;

				OnDataUpdated(JsonUtility.ToJson(remoteUser));
			}
		}

		protected void OnRequestFailed(string requestId)
		{
			RemoteRequest<SPBaseResponse> request = getRequestById(requestId) as RemoteRequest<SPBaseResponse>;

			if(request.data.error != null && request.data.error.isBadTokenError())
			{
				if(OnBadToken != null)
				{
					OnBadToken();
				}
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

		private void addDependantRequest(BaseRequest request, bool isPriority = false)
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
		}

		public override void stopAndRemoveCurrentRequests ()
		{
			base.stopAndRemoveCurrentRequests ();
			mainUpdateRequest = null;
		}
	}
		
}