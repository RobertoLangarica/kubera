using UnityEngine;
using System;
using System.Collections;

namespace Data.Sync
{
	public class LoginProvider : MonoBehaviour 
	{
		[HideInInspector] public bool isLoggedIn = false;
		[HideInInspector] public string userId = string.Empty;
		[HideInInspector] public string id;

		public Action<string> OnLoginSuccessfull;
		public Action<string> OnLogoutSuccessfull;
		public Action<string> OnLoginFail;
		public Action<string> OnServerOutOfReach;

		public virtual string getEmail()
		{
			return "";
		}

		public virtual void login(){}
		public virtual void logout(){}
	}
}