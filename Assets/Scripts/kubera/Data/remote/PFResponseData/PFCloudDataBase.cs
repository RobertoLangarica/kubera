using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.PFResponseData
{
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
	public class PFCloudDataBase
	{
		public string FunctionName;
		public int Revision;
		//public T FunctionResult;
		public List<LogStatement> Logs;
		public float ExecutionTimeSeconds;
		public int MemoryConsumedBytes;
		public int APIRequestsIssued;
		public int HttpRequestsIssued;
		public ScriptExecutionError Error;

		public virtual bool hasError()
		{
			return (!string.IsNullOrEmpty(Error.Error));
		}
	}
}
