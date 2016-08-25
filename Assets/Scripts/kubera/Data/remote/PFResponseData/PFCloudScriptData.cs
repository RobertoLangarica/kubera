using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.PFResponseData
{
	/**
	 {
	 	"FunctionName": "function name"
		"Revision": 3,
		"FunctionResult": {},
		"Logs": [LogStatement],
		"ExecutionTimeSeconds": 0.03670790046453476,
		"MemoryConsumedBytes": 38904,
		"APIRequestsIssued": 2,
		"HttpRequestsIssued": 0
		"Error": ScriptExecutionError
	}
	 **/ 
	[Serializable]
	public class PFCloudScriptData<A>
	{
		public int code;
		public string status;
		public string error;
		public string errorCode;
		public string errorMessage;
		public int CallBackTimeMS;
		public CloudResponse<A> data;
	}

	[Serializable]
	public class CloudResponse<A>
	{
		public string FunctionName;
		public int Revision;
		public A FunctionResult;
		//public string FunctionResult;
		public List<LogStatement> Logs;
		public float ExecutionTimeSeconds;
		public int MemoryConsumedBytes;
		public int APIRequestsIssued;
		public int HttpRequestsIssued;
		public ScriptExecutionError Error;
	}

	[Serializable]
	public class LogStatement
	{
		public string Level;
		public string Message;
		//public Object Data;
	}

	[Serializable]
	public class ScriptExecutionError
	{
		public string Error;
		public string Message;
		public string StackTrace;
	}
}