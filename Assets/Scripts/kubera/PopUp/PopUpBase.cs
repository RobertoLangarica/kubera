using UnityEngine;
using System.Collections;

public class PopUpBase : MonoBehaviour {

	new public string name;
	public GameObject popUp;

	public delegate void DPopUpNotification(PopUpBase thisPopUp, string action ="",bool deActivate = true);
	public DPopUpNotification OnPopUpCompleted;



	public virtual void activate()
	{
		popUp.SetActive (true);
	}

	public virtual void deactivate()
	{
		popUp.SetActive (false);
	}

	protected void OnComplete(string action ="",bool deActivate = true)
	{
		if(deActivate)
		{
			popUp.SetActive (false);
		}
		if(OnPopUpCompleted != null)
		{
			OnPopUpCompleted(this,action,deActivate);
		}	
	}
}
