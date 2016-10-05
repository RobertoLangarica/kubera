using UnityEngine;
using System;
using System.Collections;
using Data.Remote;

using BestHTTP;

using utils.gems.remote.response;



public class GMRequest : RemoteRequest<SPGetGemsResponse> 
{
	public string playerId;
	public string acces_token;

	public override string getCompletePath (string path)
	{
		return path;
	}

	public override void initialize (string path)
	{
		request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Get);
		request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
		request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);

		request._showDebugInfo = showDebugInfo;
		request.FormUsage = BestHTTP.Forms.HTTPFormUsage.UrlEncoded;
		request.AddHeader("key","gamers_mutual");
	}

	public override bool hasError ()
	{
		return (data == null || !data.error.isEmpty());
	}
}	

