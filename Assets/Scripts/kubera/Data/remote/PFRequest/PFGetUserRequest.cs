﻿using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class PFGetUserRequest : BaseRequest {

		private HTTPRequest request;
		public PFResponseBase<PFUserData> data;

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public void initialize(string titleID,string CSVkeysToQuery,int aboveVersion, string sessionTicket)
		{
			request = new HTTPRequest(new System.Uri("https://"+titleID+".playfabapi.com/Client/GetUserData"), HTTPMethods.Post);
			request.AddHeader("X-Authentication",sessionTicket);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;

			StringBuilder sbuilder = new StringBuilder("");
			string[] strings = CSVkeysToQuery.Split(',');

			sbuilder.Append("[");
			for(int i = 0; i < strings.Length; i++)
			{
				if(i > 0)
				{
					sbuilder.Append(",");
				}	

				sbuilder.Append(quotedString(strings[i]));
			}
			sbuilder.Append("]");
			request.AddField(quotedString("keys"),sbuilder.ToString());
			request.AddField(quotedString("IfChangedFromDataVersion"), aboveVersion.ToString());
		}

		private void OnCompleteCallback(HTTPRequest originalRequest, HTTPResponse response)
		{
			switch (originalRequest.State)
			{
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
					string dataAsText = response.DataAsText;
					secureLog("Request Finished Successfully!\n" + response.DataAsText);

					data = JsonUtility.FromJson<PFResponseBase<PFUserData>>(response.DataAsText);

					if(data.code == 200)
					{
						Dictionary<string,object> obj = MiniJSON.Json.Deserialize(response.DataAsText) as Dictionary<string,object>;
						data.data.Data = ((Dictionary<string,object>)obj["data"])["Data"] as Dictionary<string,object>;

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