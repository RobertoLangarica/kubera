using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.GSResponseData
{

	/**
	{
	  "@class": ".AuthenticationResponse",
	  "authToken": "82e3b9ce-63e3-4e87-955a-90c2a1a2675d",
	  "displayName": "Roberto Langarica",
	  "newPlayer": false,
	  "userId": "57d08ecc184168049e427571"
	  "error": JSON
	}
	**/
	[Serializable]
	public class GSLoginResponse
	{
		public string @class;
		public string authToken;
		public string displayName;
		public bool newPlayer;
		public string requestId;
		public string error;
		//public object scriptData; //JSON
		public string userId;
	}
}