﻿using UnityEngine;
using System;

namespace Data
{
	[Serializable]
	public class BasicData
	{
		public string _id;
		public int version;
		public bool isDirty;

		public BasicData(){}

		public virtual bool compareAndUpdate(BasicData remote, bool ignoreVersion = false)
		{
			if(remote.version != version || ignoreVersion)
			{
				updateFrom(remote, ignoreVersion);
				return true;
			}

			return false;
		}

		public virtual void updateFrom(BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			//Si se ignora la version entonces no la cambiamos
			if(ignoreVersion)
			{
				version = readOnlyRemote.version;
			}
		}
	}
}