using UnityEngine;
using System.Collections;

public class ConectFacebookMessage : MonoBehaviour {

	protected FBLog fbLog;

	public void Awake()
	{
		fbLog = FindObjectOfType<FBLog> ();
	}

	public void conectFacebook()
	{
		fbLog.OnLoginClick ();
	}
}
