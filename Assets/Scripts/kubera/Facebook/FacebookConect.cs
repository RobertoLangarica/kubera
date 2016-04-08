using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookConect : MonoBehaviour {

	public Button conect;

	protected FBLog fbLog;

	public void Awake()
	{
		fbLog = FindObjectOfType<FBLog> ();

		if(fbLog == null)
		{
			fbLog =  gameObject.AddComponent<FBLog> ();
		}
	}

	public void conectFacebook()
	{
		fbLog.OnLoginClick ();
	}
}
