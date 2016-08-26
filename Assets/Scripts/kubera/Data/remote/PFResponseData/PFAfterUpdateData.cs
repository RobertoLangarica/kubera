using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{
	[Serializable]
	public class PFAfterUpdateData:PFCloudDataBase
	{
		public FunctionResultObject FunctionResult;
	}

	//Este es el struct que hay que modificar para cada respuesta de funcion
	[Serializable]
	public class FunctionResultObject
	{
		public int	DataVersion;
	}
}