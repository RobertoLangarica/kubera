using UnityEngine;
using System;
using System.Collections;

namespace utils.gems.remote.response
{
	[Serializable]
	public class SPBaseResponse
	{
		public ShopikaError error;
	}

	[Serializable]
	public class SPGetGemsResponse:SPBaseResponse
	{
		/*
		{
			"error":ShopikaError,
			"gemBalance":int
		}*/
		public int gemBalance;


		public SPGetGemsResponse()
		{
			error = null;
		}
	}


	[Serializable]
	public class ShopikaError
	{
		/**
		 * "error": {
		    "name": "Error",
		    "status": 401,
		    "message": "Authorization Required",
		    "statusCode": 401,
		    "code": "AUTHORIZATION_REQUIRED",
		    "stack": "Error: Authorization Required\n    at /opt/shopikaadminapi/node_modules/loopback/lib/application.js:376:21\n    at /opt/shopikaadminapi/node_modules/loopback/lib/model.js:313:7\n    at /opt/shopikaadminapi/node_modules/loopback/common/models/acl.js:465:23\n    at /opt/shopikaadminapi/node_modules/loopback/node_modules/async/lib/async.js:251:17\n    at done (/opt/shopikaadminapi/node_modules/loopback/node_modules/async/lib/async.js:132:19)\n    at /opt/shopikaadminapi/node_modules/loopback/node_modules/async/lib/async.js:32:16\n    at /opt/shopikaadminapi/node_modules/loopback/node_modules/async/lib/async.js:248:21\n    at /opt/shopikaadminapi/node_modules/loopback/node_modules/async/lib/async.js:572:34\n    at /opt/shopikaadminapi/node_modules/loopback/common/models/acl.js:447:17\n    at /opt/shopikaadminapi/node_modules/loopback/common/models/role.js:186:9"
		  }
		 */

		public string name;
		public int status;
		public string message;
		public int statusCode;
		public string code;
		public string stack;

		public bool isEmpty()
		{
			return (name=="" || name == null) && (message=="" || message == null)
				&& (code=="" || code == null) && (stack=="" || stack == null) && status==0 && statusCode==0;
		}

		public bool isBadTokenError()
		{
			return (status == 401 || statusCode == 401 || code == "INVALID_TOKEN");
		}
	}
}