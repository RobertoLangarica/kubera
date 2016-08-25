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
	public class PFResponseUserData
	{
		public PFUserData Data;
		public int	DataVersion;
	}

	[Serializable]
	public class PFUserData
	{
		public PFLevelsData levels;
		public PFVersionData version;

		public PFUserData()
		{
			levels = new PFLevelsData();
			version = new PFVersionData();
		}
	}

	[Serializable]
	public class PFLevelsData
	{
		public List<LevelData> Value;
		public string LastUpdated;
		public string Permission;

		public PFLevelsData()
		{
			Value = new List<LevelData>();
		}
	}


	[Serializable]
	public class PFVersionData
	{
		public int Value;
		public string LastUpdated;
		public string Permission;
	}

}