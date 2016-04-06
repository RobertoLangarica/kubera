using UnityEngine;
using System.Collections;

public class PopUpBase : MonoBehaviour {

	public string name;
	public GameObject popUp;

	public delegate void DPopUpNotification(string action ="");
	public DPopUpNotification OnPopUpCompleted;

	public virtual void activate()
	{

	}

	protected void OnComplete(string action ="")
	{
		if(OnPopUpCompleted != null)
		{
			OnPopUpCompleted(action);
		}	
	}
}
