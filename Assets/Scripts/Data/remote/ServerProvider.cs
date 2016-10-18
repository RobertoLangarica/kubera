using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data.Sync;

namespace Data.Remote
{
	public class ServerProvider : MonoBehaviour 
	{
		public bool _mustShowDebugInfo = false;

		public RequestQueue queue;

		public Action<RemoteUser> OnUserReceived;//string: Only id's data
		public Action<string> OnDataReceived;//string: full json
		public Action<string> OnDataUpdated;//string:  Updated data
		public Action OnGetDataFailed;

		protected List<BaseRequest> requests;

		protected int getDataFailCount;//Conteo de fallos el hacer getUserData
		protected int getDataMaxFailCountAllowed = 2;//Maximos fallos permitidos

		void Start()
		{
			requests = new List<BaseRequest>();
		}

		protected void getDataFailed(string requestId)
		{
			getDataFailCount++;

			if(getDataFailCount >= getDataMaxFailCountAllowed)
			{
				//Maximo de intentos alcanzado
				maxFailCountReached(getRequestById(requestId));
			}
		}

		protected virtual void maxFailCountReached(BaseRequest request)
		{
			if(_mustShowDebugInfo)
			{
				Debug.Log("Failed to retreive user data.");
			}

			//reallity check
			if(request.isRequesting)
			{request.stop();}

			//Que no se vuelva a ejecutar (el ciclo para eliminarla ya se hace fuera de aqui)
			request.persistAfterFailed = false;

			if(OnGetDataFailed != null)
			{
				OnGetDataFailed();
			}
		}

		public virtual void getUniqueUser(string customId, string facebookId){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Todos los datos del usuario
		 **/ 
		public virtual void getUserData(string id, bool saveAsMainRequest = false){ Debug.LogError("No existe implementación de esta función");}
		public virtual void getUserData(string id, int aboveVersion, bool saveAsMainRequest = false){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Sube el json indicado.
		 * El json debe tener la estructura adecuada pero solo contener los datos a subir,
		 * asi usamos menos ancho de banda
		 **/ 
		public virtual void updateUserData(string id, string data){Debug.LogError("No existe implementación de esta función");}
		public virtual void updateUserData<T>(string id, T data){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Sube el json indicado.
		 * El json debe tener la estructura adecuada pero solo contener los datos a subir,
		 * asi usamos menos ancho de banda
		 **/ 
		public virtual void createUserData(string id, string jsonData){Debug.LogError("No existe implementación de esta función");}
		public virtual void createUserData<T>(string id, string jsonData,T objectToSave){Debug.LogError("No existe implementación de esta función");}


		public virtual void getLeaderboardData(string id, string leaderboardName, int maxResultsCount){Debug.LogError("No existe implementación de esta función");}

		public virtual void stopAndRemoveCurrentRequests()
		{
			foreach(BaseRequest request in requests)
			{
				queue.removeAndStopRequest(request, true);
			}

			requests.Clear();
		}

		protected BaseRequest getRequestById(string id)
		{
			return requests.Find(item => item.id.Equals(id));
		}

		protected void addRequest(BaseRequest request, bool isPriority = false)
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
	}
}