using UnityEngine;
using System;
using System.Collections;

using BestHTTP;

namespace Data.Remote
{
	public class RemoteRequest<T>: BaseRequest 
	{
		protected HTTPRequest request;
		public T data;


		public virtual void initialize(string path)
		{
			Debug.LogError("Method not implemented");
		}

		public virtual string getCompletePath(string path)
		{
			return path;
		}

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public virtual bool hasError()
		{
			return false;	
		}

		private void OnCompleteCallback(HTTPRequest originalRequest, HTTPResponse response)
		{
			switch (originalRequest.State)
			{
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
					secureLog("Request Finished Successfully!\n" + response.DataAsText);

					formatData(response.DataAsText);

					if(!hasError())
					{
						OnRequestComplete();
					}	
					else
					{
						OnRequestFailed();	
					}

				break;
					// The request finished with an unexpected error.
					// The request's Exception property may contain more information about the error.
				case HTTPRequestStates.Error:
					secureLogError("Request Finished with Error! " +
						(originalRequest.Exception != null ?
							(originalRequest.Exception.Message + "\n" + originalRequest.Exception.StackTrace) :
							"No Exception"));
					OnRequestFailed();
				break;

					// The request aborted, initiated by the user.
				case HTTPRequestStates.Aborted:
					secureLogWarning("Request Aborted!");
				break;

					// Ceonnecting to the server timed out.
				case HTTPRequestStates.ConnectionTimedOut:
					secureLogError("Connection Timed Out!");
					OnRequestTimeout();
					originalRequest.Abort();
				break;

					// The request didn't finished in the given time.
				case HTTPRequestStates.TimedOut:
					secureLogError("Processing the request Timed Out!");
					OnRequestTimeout();
					originalRequest.Abort();
				break;
			}

		}

		protected virtual void formatData(string dataAsText)
		{
			data = JsonUtility.FromJson<T>(dataAsText);
		}

		public override void stop ()
		{
			base.stop ();
			request.Callback = null;
			request.Abort();
		}
	}
}