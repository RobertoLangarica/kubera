using UnityEngine;
using System.Collections;

public class ConectFacebookMessage : MonoBehaviour {

	protected FBLoggin fbLog;

	public void Awake()
	{
		fbLog = FindObjectOfType<FBLoggin> ();
	}

	public void conectFacebook()
	{
		fbLog.OnLoginClick ();
	}
}
