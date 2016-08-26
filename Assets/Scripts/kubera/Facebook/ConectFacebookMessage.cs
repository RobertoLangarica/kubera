using UnityEngine;
using System.Collections;
using Kubera.Data.Sync;

public class ConectFacebookMessage : MonoBehaviour 
{
	public void conectFacebook()
	{
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookLogin();
	}
}
