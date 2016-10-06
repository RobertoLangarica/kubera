using UnityEngine;
using System;
using System.Collections;
using Data.Remote;

using BestHTTP;

using utils.gems.remote.response;

public class GMRequestUsers : RemoteRequest<SPGetGemsResponse> 
{

	public string playerId;
	public string acces_token;

	public override string getCompletePath (string path)
	{
		return path+"/api/users";
	}

	public override void initialize (string path)
	{
		request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Post);
		request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
		request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);

		request._showDebugInfo = showDebugInfo;


		request.AddField(quotedString("username"),quotedString("roberto"));
		request.AddField(quotedString("email"),quotedString("b@b.com"));
		request.AddField(quotedString("password"),quotedString("123456"));
		request.AddHeader("key","gamers_mutual");

		request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;
	}

	public override bool hasError ()
	{
		return (data == null || !data.error.isEmpty());
	}
}
