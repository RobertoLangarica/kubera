using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookConectMessage : MonoBehaviour {

	public Button conect;
	public Text textInfo;

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
