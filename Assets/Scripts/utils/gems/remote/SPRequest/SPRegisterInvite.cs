using UnityEngine;
using System;
using System.Collections;
using Data.Remote;

using BestHTTP;

using utils.gems.remote.response;

namespace utils.gems.remote.request
{
	public class SPRegisterInvite : RemoteRequest<SPGetGemsResponse>  
	{
		public string playerId;
		public string acces_token;
		public string invitedEmail;
		public string invitedFacebookId;
		public string invitedId;
		public string invitedPhoneNumber;
		public string inviterFacebookId;
		public string inviterId;

		public override string getCompletePath (string path)
		{
			return path+"/Invites?access_token="+acces_token;
		}

	}
}