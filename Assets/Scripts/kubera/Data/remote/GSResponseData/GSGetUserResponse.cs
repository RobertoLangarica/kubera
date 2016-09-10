using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.GSResponseData
{

	/**
	{
	  "@class": ".AuthenticationResponse",
	  "error": JSON
	  "message": string
	}
	**/
	public class GSGetUserResponse
	{
		public string @class;
		public string error;
		/**
		 {
		    "_id" : "user2",
		    "levels" : {
		        "5" : {
		            "points" : 4000
		        },
		        "4" : {
		            "stars" : 8,
		            "passed" : 1,
		            "locked" : 1
		        },
		        "6" : {
		            "points" : 6000,
		            "stars" : 6,
		            "passed" : 1
		        },
		        "7" : {
		            "points" : 4000
		        }
		    }
		}
		 **/ 
		public Dictionary<string, object> scriptData;
	}
}