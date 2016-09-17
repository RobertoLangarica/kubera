using UnityEngine;
using System;
using System.Collections;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.GSResponseData;

namespace Kubera.Data.Remote
{
	public class GSLoginRequest :  RemoteRequest<GSLoginResponse>
	{
		public string facebookAccessToken;

		public override string getCompletePath (string path)
		{
			return path+"/FacebookConnectRequest";
		}

		public override void initialize (string path)
		{
			request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Post);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request.AddField(quotedString("@class"), quotedString(".FacebookConnectRequest"));
			request.AddField(quotedString("accessToken"), quotedString(facebookAccessToken));
			request.AddField(quotedString("doNotLinkToCurrentPlayer"),"false");
			request.AddField(quotedString("errorOnSwitch"),"false");
			request.AddField(quotedString("switchIfPossible"),"true");
			request.AddField(quotedString("requestId"),quotedString(this.id));
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;

		}



		public override bool hasError ()
		{
			return (data == null || data.error != null);
		}
	}

}