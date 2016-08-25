using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{

	/**
	{
	    "DataVersion": 3
	}
	**/
	[Serializable]
	public class PFAfterUpdateData
	{
		public string messageValue;
		public int	DataVersion;
	}
}