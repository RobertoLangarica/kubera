using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.PFResponseData
{

	/**
	{
		"Data": {
			"preferences": {
				"Value": "alpha",
				"LastUpdated": "2014-08-20T12:30:45Z",
				"Permission": "Public"
			},
			"progress": {
				"Value": "level_twenty",
				"LastUpdated": "2014-09-01T10:12:30Z",
				"Permission": "Private"
			}
		},
		"DataVersion": 0
	}
	**/
	[Serializable]
	public class PFUserData
	{
		[NonSerialized]public Dictionary<string,object> Data;
		public int	DataVersion;
	}
}