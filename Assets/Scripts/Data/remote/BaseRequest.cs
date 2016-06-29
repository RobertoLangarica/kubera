using UnityEngine;
using System;
using System.Collections;

namespace Data.Remote
{
	public class BaseRequest 
	{
		public Action<string> OnFailed;
		public Action<string> OnComplete;

		public enum EStatus
		{
			WAITING,
			REQUESTING,
			FAILED	
		}

		public string id;
		public EStatus status = EStatus.WAITING;
		public bool persistAfterFailed = false;

		protected float elapsedWhenRequested;

		public virtual void stop()
		{
			status = EStatus.WAITING;
		}

		public virtual void start()
		{
			status = EStatus.REQUESTING;
			elapsedWhenRequested = Time.realtimeSinceStartup;
		}

		public virtual void OnRequestFailed()
		{
			status = EStatus.FAILED;
		}

		public float getTimeSinceRequested()
		{
			return Time.realtimeSinceStartup - elapsedWhenRequested;
		}
	}
}