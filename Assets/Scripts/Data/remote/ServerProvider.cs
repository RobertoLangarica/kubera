﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data.Sync;

namespace Data.Remote
{
	public class ServerProvider : MonoBehaviour 
	{
		public RequestQueue queue;

		public Action<GameUser> OnUserReceived;//string: Only id's data
		public Action<string> OnDataReceived;//string: full json
		public Action<string> OnDataUpdated;//string:  Updated data

		protected List<BaseRequest> requests;

		void Start()
		{
			requests = new List<BaseRequest>();
		}

		public virtual void getUniqueUser(string customId, string facebookId){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Todos los datos del usuario
		 **/ 
		public virtual void getUserData(string id, string extraData){ Debug.LogError("No existe implementación de esta función");}
		public virtual void getUserData(string id, string extraData, int aboveVersion){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Sube el json indicado.
		 * El json debe tener la estructura adecuada pero solo contener los datos a subir,
		 * asi usamos menos ancho de banda
		 **/ 
		public virtual void updateUserData(string id, string jsonData){Debug.LogError("No existe implementación de esta función");}

		/**
		 * Sube el json indicado.
		 * El json debe tener la estructura adecuada pero solo contener los datos a subir,
		 * asi usamos menos ancho de banda
		 **/ 
		public virtual void createUserData(string id, string jsonData){Debug.LogError("No existe implementación de esta función");}

		public virtual void stopAndRemoveCurrentRequests()
		{
			foreach(BaseRequest request in requests)
			{
				queue.removeAndStopRequest(request, true);
			}

			requests.Clear();
		}
	}
}