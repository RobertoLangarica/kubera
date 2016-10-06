using UnityEngine;
using System;
using System.Collections;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class PFCreateUserRequest<T> : BaseRequest {

		private HTTPRequest request;
		public PFResponseBase<PFCreateData> data;
		public T dataSended;

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public void initialize(string titleID,string jsonData, string sessionTicket, T dataToSend)
		{
			request = new HTTPRequest(new System.Uri("https://"+titleID+".playfabapi.com/Client/UpdateUserData"), HTTPMethods.Post);
			request.AddHeader("X-Authentication",sessionTicket);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request.AddField(quotedString("Data"),jsonData);
			request.AddField(quotedString("Permission"), quotedString("Public"));
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;

			dataSended = dataToSend;
		}

		private void OnCompleteCallback(HTTPRequest originalRequest, HTTPResponse response)
		{
			switch (originalRequest.State)
			{
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
					secureLog("Request Finished Successfully!\n" + response.DataAsText);

					data = JsonUtility.FromJson<PFResponseBase<PFCreateData>>(response.DataAsText);

					if(data.code == 200)
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

		public override void stop ()
		{
			base.stop ();
			request.Callback = null;
			request.Abort();
		}
	}
}