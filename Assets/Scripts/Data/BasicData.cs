using UnityEngine;
using System;

namespace Data
{
	[Serializable]
	public class BasicData
	{
		public string id;
		public int version;
		public bool isDirty;

		public BasicData(){}

		public bool compareAndUpdate(BasicData remote)
		{
			if(remote.version > version)
			{
				updateFrom(remote);
				return true;
			}

			return false;
		}

		public virtual void updateFrom(BasicData readOnlyRemote)
		{
			version = readOnlyRemote.version;
		}
	}
}