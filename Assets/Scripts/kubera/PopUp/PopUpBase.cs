﻿using UnityEngine;
using System.Collections;

public class PopUpBase : MonoBehaviour {

	new public string name;
	public GameObject popUp;

	public delegate void DPopUpNotification(PopUpBase thisPopUp, string action ="");
	public DPopUpNotification OnPopUpCompleted;

	public virtual void activate()
	{
		popUp.SetActive (true);
	}

	protected void OnComplete(string action ="")
	{
		if(OnPopUpCompleted != null)
		{
			OnPopUpCompleted(this,action);
		}	
		popUp.SetActive (false);
	}
}
