using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data.Remote
{
	public class PFGetLeaderboardRequest : BaseRequest {

		private HTTPRequest request;
		public PFResponseBase<PFLeaderboardData> data;
		public string statisticUsed;

		public override void start ()
		{
			base.start ();

			request.Callback = OnCompleteCallback;
			request.Send();
		}

		public void initialize(string titleID, string userPlayFabId, string statisticName, int maxResultsCount, string sessionTicket)
		{
			request = new HTTPRequest(new System.Uri("https://"+titleID+".playfabapi.com/Client/GetFriendLeaderboardAroundPlayer"), HTTPMethods.Post);
			request.AddHeader("X-Authentication",sessionTicket);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;

			request.AddField(quotedString("StatisticName"),quotedString(statisticName));
			request.AddField(quotedString("MaxResultsCount"), maxResultsCount.ToString());
			request.AddField(quotedString("PlayFabId"), quotedString(userPlayFabId));
			request.AddField(quotedString("IncludeFacebookFriends"), "true");

			statisticUsed = statisticName;
		}

		private void OnCompleteCallback(HTTPRequest originalRequest, HTTPResponse response)
		{
			switch (originalRequest.State)
			{
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
					string dataAsText = response.DataAsText;
					secureLog("Request Finished Successfully!\n" + response.DataAsText);

					data = JsonUtility.FromJson<PFResponseBase<PFLeaderboardData>>(response.DataAsText);

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