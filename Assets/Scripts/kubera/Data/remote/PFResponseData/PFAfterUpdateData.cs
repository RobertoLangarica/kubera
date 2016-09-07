using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{
	[Serializable]
	public class PFAfterUpdateData:PFCloudDataBase
	{
		public PFAfterUpdateResultObject FunctionResult;

		public override bool hasError ()
		{
			return base.hasError();
		}
	}

	[Serializable]
	public class PFAfterUpdateResultObject
	{
		public int	DataVersion;
	}
}