using UnityEngine;
using System;
using System.Collections;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class PFCreateUserRequest : BaseRequest {

		private HTTPRequest request;
		public PFResponseBase<PFCreateData> data;
		public string sendedJSON;

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public void initialize(string titleID,string jsonData, string sessionTicket)
		{
			request = new HTTPRequest(new System.Uri("https://"+titleID+".playfabapi.com/Client/UpdateUserData"), HTTPMethods.Post);
			request.AddHeader("X-Authentication",sessionTicket);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			//request.AddField(quotedString("Data"),jsonData.Replace("[","{").Replace("]","}"));
			request.AddField(quotedString("Data"),jsonData);
			//request.AddField(quotedString("KeysToRemove"),"["+quotedString("PlayFab_dataVersion")+"]");
			request.AddField(quotedString("Permission"), quotedString("Public"));
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;

			sendedJSON = jsonData;
		}

		private void OnCompleteCallback(HTTPRequest originalRequest, HTTPResponse response)
		{
			switch (originalRequest.State)
			{
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
					Debug.Log("Request Finished Successfully!\n" + response.DataAsText);

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
					Debug.LogError("Request Finished with Error! " +
						(originalRequest.Exception != null ?
							(originalRequest.Exception.Message + "\n" + originalRequest.Exception.StackTrace) :
							"No Exception"));
					OnRequestFailed();
				break;

					// The request aborted, initiated by the user.
				case HTTPRequestStates.Aborted:
					Debug.LogWarning("Request Aborted!");
				break;

					// Ceonnecting to the server timed out.
				case HTTPRequestStates.ConnectionTimedOut:
					Debug.LogError("Connection Timed Out!");
					OnRequestTimeout();
					originalRequest.Abort();
				break;

					// The request didn't finished in the given time.
				case HTTPRequestStates.TimedOut:
					Debug.LogError("Processing the request Timed Out!");
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