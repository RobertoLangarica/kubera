using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.GSResponseData;

namespace Kubera.Data.Remote
{
	public class GSGetUserRequest :  RemoteRequest<GSGetUserResponse>
	{
		public string playerId;
		public int aboveVersion;

		public override string getCompletePath (string path)
		{
			return path+"/LogEventRequest";
		}

		public override void initialize (string path)
		{
			request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Post);

			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request.AddField(quotedString("@class"), quotedString(".LogEventRequest"));
			request.AddField(quotedString("eventKey"), quotedString("GET_DATA"));
			request.AddField(quotedString("above_version"),aboveVersion.ToString());
			request.AddField(quotedString("requestId"),quotedString(this.id));
			request.AddField(quotedString("playerId"),quotedString(playerId));
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;
		}

		protected override void formatData(string dataAsText)
		{
			Dictionary<string,object> preData = MiniJSON.Json.Deserialize(dataAsText) as Dictionary<string,object>;

			data = new GSGetUserResponse();
			data.@class = preData["@class"].ToString();

			if(preData.ContainsKey("error"))
			{
				//data con error
				data.error = preData["error"].ToString();
			}
			else
			{
				//data sin error
				data.scriptData = preData["scriptData"] as Dictionary<string,object>;
				Debug.Log("UserData count:"+ (data.scriptData["userData"] as Dictionary<string,object>).Count);
			}

		}

		public override bool hasError ()
		{
			return (data == null || data.error != null);
		}
	}

}