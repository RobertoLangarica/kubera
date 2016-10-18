using UnityEngine;
using System;
using System.Collections;
using Data.Remote;

using BestHTTP;

using utils.gems.remote.response;

namespace utils.gems.remote.request
{
	public class SPGetGemsRequest : RemoteRequest<SPGetGemsResponse> 
	{
		public string playerId;
		public string acces_token;

		public override string getCompletePath (string path)
		{
			return path+"/Shoppers/"+playerId+"/gems?access_token="+acces_token;
		}

		public override void initialize (string path)
		{
			request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Get);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);

			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.UrlEncoded;
		}

		public override bool hasCredentialError ()
		{
			return (data.error != null && data.error.isBadTokenError());
		}

		public override bool hasError ()
		{
			return (data == null || !data.error.isEmpty());
		}
	}	
}