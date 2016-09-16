using UnityEngine;
using System;
using System.Collections;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class PFUpdateDataRequest : BaseRequest {

		private HTTPRequest request;
		public PFResponseBase<PFAfterUpdateData> data;
		public object dataSended;

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public void initialize(string titleID,string jsonData, string sessionTicket, object dataToSend)
		{
			request = new HTTPRequest(new System.Uri("https://"+titleID+".playfabapi.com/Client/ExecuteCloudScript"), HTTPMethods.Post);
			request.AddHeader("X-Authentication",sessionTicket);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			//request.AddField(quotedString("FunctionParameter"),jsonData.Replace("[","{").Replace("]","}"));
			request.AddField(quotedString("FunctionName"), quotedString("updateUserLevels"));
			request.AddField(quotedString("FunctionParameter"),jsonData);
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

					data = JsonUtility.FromJson<PFResponseBase<PFAfterUpdateData>>(response.DataAsText);

					//TODO: actulizar version del JSON local para el diff
					if(data.code == 200 && !data.data.hasError())
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