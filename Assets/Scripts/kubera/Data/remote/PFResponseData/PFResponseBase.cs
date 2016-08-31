using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{

	/**
	{
 	"code": 400,
 	"status": "BadRequest",
 	"error": "InvalidFacebookToken",
 	"errorCode": 1013,
 	"errorMessage": "Invalid Facebook token",
 	"CallBackTimeMS": 484
	}
	**/
	[Serializable]
	public class PFResponseBase<T>
	{
		public int code;
		public string status;
		public string error;
		public string errorCode;
		public string errorMessage;
		public int CallBackTimeMS;
		public T data;
	}
}

