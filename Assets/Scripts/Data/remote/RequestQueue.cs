using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data.Remote
{
	public class RequestQueue : Manager<RequestQueue> 
	{
		protected List<BaseRequest> queue;

		protected override void Awake ()
		{
			base.Awake ();
		}
	}
}