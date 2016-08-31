using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data.Remote
{
	public class RequestQueue : MonoBehaviour 
	{
		public int simultaneousRequests = 1;
		public int waitTimeForFailedRequest = 30;//Tiempo antes de intentar una request fallida
		public int pingTimeout = 15;//Tempo entre pings para probar la conexion
		public string trustedIPAddress = "148.245.203.89";

		protected int activeRequestCount = 0;
		protected int simultaneousFailures = 0;
		protected float failRatioToNoConnection = .3f;//Ratio(entre 0 y 1) de fallo antes de declarar conexion perdida
		protected bool searchingConnection = false;//Indica si se esta buscando conexion
		protected List<BaseRequest> queue;
		protected Ping ping;
		protected float pingStartTime = 0;

		protected void Start()
		{
			queue = new List<BaseRequest>();
			startSearchingForConnection();
		}

		/**
		 * Remueve la request de la cola y la detiene si es necesario
		 **/ 
		public void removeAndStopRequest(BaseRequest request, bool destroy = true)
		{
			if(request.isRequesting)
			{
				request.stop();
			}

			request.OnComplete -= OnRequestComplete;
			request.OnFailed -= OnRequestFailed;
			request.OnTimeout -= OnRequestTimeout;

			if(destroy)
			{
				removeAndDestroyRequest(request);
			}
			else
			{
				queue.Remove(request);	
			}
		}

		protected void removeAndDestroyRequest(BaseRequest request)
		{
			queue.Remove(request);
			GameObject.Destroy(request.gameObject);
		}

		void Update()
		{
			if(!searchingConnection)
			{
				activeRequestCount = 0;
				for(int i = queue.Count-1; i >= 0; i--)
				{
					BaseRequest request = queue[i];

					if(activeRequestCount >= simultaneousRequests)
					{
						//Ya nadie mas puede hacer request
						break;
					}

					if(request.isRequesting)
					{
						activeRequestCount++;
					}
					else
					{
						//Necesita esperar para no saturar de llamadas fallidas sin internet
						if(!request.failed || request.getTimeSinceFailed() >= waitTimeForFailedRequest)
						{
							//Activamos la request
							request.OnComplete	+= OnRequestComplete;
							request.OnFailed	+= OnRequestFailed;
							request.OnTimeout	+= OnRequestTimeout;
							request.start();
							activeRequestCount++;
						}
					}
				}
			}
			else
			{
				if(ping.isDone)
				{
					alreadyConnected();
				}
				else
				{
					//Al parecer no importa si se recupero la conexion el ping no lo detecta
					//Le damos un timeout para la deteccion
					if(Time.realtimeSinceStartup - pingStartTime > pingTimeout)
					{
						//Iniciamos de nuevo
						ping.DestroyPing();
						ping = new Ping(trustedIPAddress);
						pingStartTime = Time.realtimeSinceStartup;
					} 
				}
			}
		}
			
		protected void OnRequestComplete(string requestId)
		{
			BaseRequest request = getRequestById(requestId);

			//se rompe la cadena de fallos
			simultaneousFailures = 0;

			//add dependent requests
			foreach(BaseRequest dependant in request.dependantRequests)
			{
				addRequest(dependant);
			}

			//Removemos la request solo de manera logica
			queue.Remove(request);
			//removeAndDestroyRequest(request);
		}

		protected void OnRequestFailed(string requestId)
		{
			BaseRequest request = getRequestById(requestId);

			if(request.persistAfterFailed)
			{
				moveRequestToLast(request);	
			}
			else
			{
				queue.Remove(request);//se remueve de manera logica
				//removeAndDestroyRequest(request);
			}

			afterRequestFailedOrTimeout();
		}

		protected void OnRequestTimeout(string requestId)
		{
			BaseRequest request = getRequestById(requestId);

			if(request.persistAfterFailed)
			{
				moveRequestToLast(request);	
			}
			else
			{
				queue.Remove(request);//se remueve de manera logica
				//removeAndDestroyRequest(request);
			}

			afterRequestFailedOrTimeout();
		}

		protected BaseRequest getRequestById(string id)
		{
			return queue.Find(item => item.id == id);
		}

		protected void moveRequestToLast(BaseRequest request)
		{
			//se recorre de atras hacia adelante
			queue.Remove(request);
			queue.Insert(0,request);
		}

		protected void afterRequestFailedOrTimeout()
		{
			simultaneousFailures++;

			if(!isConnected())
			{
				//se perdio la conexion
				//Iniciamos procedimiento sin conexion
				startSearchingForConnection();
			}
		}

		public void addPriorityRequest(BaseRequest request)
		{
			queue.Add(request);
		}

		public void addRequest(BaseRequest request)
		{
			queue.Insert(0,request);
		}

		/**
		 * Indica si se tiene conexion basado en el ratio de exito de las llamadas
		 **/ 
		protected bool isConnected()
		{
			return ((simultaneousFailures*1.0f)/(queue.Count*1.0f)) < failRatioToNoConnection;
		}

		/**
		 * Comienza a buscar conexion usando una ip confiable para hacer ping
		 **/ 
		protected void startSearchingForConnection()
		{
			searchingConnection = true;
			ping = new Ping(trustedIPAddress);
			pingStartTime = Time.realtimeSinceStartup;
		}

		/**
		 * Se manda llamara en cuanto se recupera la conexion
		 **/ 
		public void alreadyConnected()
		{
			if(searchingConnection)
			{
				searchingConnection = false;	
				ping.DestroyPing();
				ping = null;
			}

			//Marcamos las request fallidas como no fallidas
			removeFailedStatusFromRequests();
		}

		public void removeFailedStatusFromRequests()
		{
			foreach(BaseRequest request in queue)
			{
				//De fallida a esperando
				if(request.failed)
				{
					request.status = BaseRequest.EStatus.WAITING;
				}

				//Se marca como que nunca a fallado
				if(request.hasFailed)
				{
					request.hasFailed = false;
				}
			}
		}

		public T getComponentAttachedToGameObject<T>(string name) where T:BaseRequest
		{
			GameObject go = new GameObject(name);
			T component = go.AddComponent<T>();
			go.transform.SetParent(this.transform);

			return component;
		}
	}
}