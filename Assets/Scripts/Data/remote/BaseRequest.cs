using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Data.Remote
{
	public class BaseRequest : MonoBehaviour
	{
		public Action<string> OnFailed;
		public Action<string> OnComplete;
		public Action<string> OnTimeout;

		public enum EStatus
		{
			WAITING,
			REQUESTING,
			FAILED,
			COMPLETE
		}

		public string id;
		public EStatus status = EStatus.WAITING;
		public bool hasFailed = false;
		public bool persistAfterFailed = false;
		public int timeBeforeTimeout = 20;
		public List<BaseRequest> dependantRequests = new List<BaseRequest>();

		protected float elapsedWhenRequested;
		protected float elapsedWhenFailed;

		public virtual void stop()
		{
			status = EStatus.WAITING;
		}

		public virtual void start()
		{
			//La request recuerda si antes fallo
			hasFailed = failed;
			status = EStatus.REQUESTING;
			elapsedWhenRequested = Time.realtimeSinceStartup;
		}

		protected virtual void OnRequestFailed()
		{
			status = EStatus.FAILED;
			elapsedWhenFailed = Time.realtimeSinceStartup;

			if(OnFailed != null)
			{
				OnFailed(id);
			}
		}

		protected virtual void OnRequestTimeout()
		{
			//La marcamos como fallida
			status = EStatus.FAILED;
			elapsedWhenFailed = Time.realtimeSinceStartup;

			if(OnTimeout != null)
			{
				OnTimeout(id);
			}
		}

		protected virtual void OnRequestComplete()
		{
			status = EStatus.COMPLETE;
			if(OnComplete != null)
			{
				OnComplete(id);
			}
		}

		public float getTimeSinceRequested()
		{
			return Time.realtimeSinceStartup - elapsedWhenRequested;
		}


		public float getTimeSinceFailed()
		{
			return Time.realtimeSinceStartup - elapsedWhenFailed;
		}

		public bool isRequesting
		{
			get{return (status == EStatus.REQUESTING);}	
		}

		public bool isWaiting
		{
			get{return (status == EStatus.WAITING);}	
		}

		public bool failed
		{
			get{return (status == EStatus.FAILED);}	
		}

		protected void Update()
		{
			if(isRequesting)
			{
				if(getTimeSinceRequested() > timeBeforeTimeout)
				{
					OnRequestTimeout();
				}
			}
		}

		public string quotedString(string input)
		{
			return "\""+input+"\"";
		}
	}
}