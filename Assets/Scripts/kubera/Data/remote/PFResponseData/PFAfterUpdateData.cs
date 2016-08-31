using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{
	[Serializable]
	public class PFAfterUpdateData:PFCloudDataBase
	{
		public PFAfterUpdateResultObject FunctionResult;
	}

	//Este es el struct que hay que modificar para cada respuesta de funcion
	[Serializable]
	public class PFAfterUpdateResultObject
	{
		public int	DataVersion;
	}
}