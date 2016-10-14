using UnityEngine;
using System;
using System.Collections;
using Data.Remote;

using BestHTTP;

using utils.gems.remote.response;

namespace utils.gems.remote.request
{
	public class SPRegisterInvite : RemoteRequest<SPBaseResponse>  
	{
		//public string playerId;
		//public string acces_token;
		public string invitedEmail;
		public string invitedFacebookId;
		public string invitedId;
		public string invitedPhoneNumber;
		public string inviterFacebookId;
		public string inviterId;

		public override string getCompletePath (string path)
		{
			return path+"/Invites";
		}

		public override void initialize (string path)
		{
			request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Post);
			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);

			request.AddField(quotedString("invitedEmail"),invitedEmail);
			request.AddField(quotedString("invitedFacebookId"),invitedFacebookId);
			request.AddField(quotedString("invitedId"),invitedId);
			request.AddField(quotedString("invitedPhoneNumber"),invitedPhoneNumber);
			request.AddField(quotedString("inviterFacebookId"),inviterFacebookId);
			request.AddField(quotedString("inviterId"),inviterId);

			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;
		}
	}
}