using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.GSResponseData
{

	/**
	 * Se reciben datos en el formato:
	{
	  "@class": ".AuthenticationResponse",
	  "error": JSON
	  "message": string
	  "scripData":
		 {
		    "_id" : "user2",
		    "version":int
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
	}
	**/
	public class GSGetUserResponse
	{
		public string @class;
		public string error;

		public List<LevelData> levels;
		public int version;
		public int maxLevelReached;
		public bool gemsUse;
		public bool gemsPurchase;
		public bool gemsUseAfterPurchase;
		public bool lifesAsked;
	}
}