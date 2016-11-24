using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopUpManager : MonoBehaviour {

	public List<PopUpBase> popups;

	public delegate void DPopUpNotification(string action ="");
	public DPopUpNotification OnPopUpCompleted;

	public List<PopUpBase> openPopUps = new List<PopUpBase>();

	public void DelayedStart()
	{
		/*print (FindObjectsOfType<PopUpBase> ().Length);
		if(FindObjectsOfType<PopUpBase> ().Length != popups.Count)
		{
			print ("FALTA AGREGAR ALGUN POPUP O BORRARLO");
			print ("popUpsFound " + FindObjectsOfType<PopUpBase> ().Length);
			print ("popUpsRegistered " + popups.Count);
		}*/
		foreach(PopUpBase popup in popups)
		{
			if (popup != null) {
				popup.OnPopUpCompleted += completePopUp;
			}
		}
	}

	public void activatePopUp(string popUpName)
	{
		PopUpBase popup = getPopupByName(popUpName);

		if(popup == null)
		{
			completePopUp(popup);
		}
		else
		{
			openPopUps.Add (popup);
			popup.gameObject.transform.SetAsLastSibling ();
			popup.activate();
		}
	}

	public PopUpBase getPopupByName(string name)
	{
		foreach(PopUpBase popup in popups)
		{
			if (popup != null) 
			{
				if (popup.name == name) {
					return popup;
				}
			}
		}

		return null;
	}

	private void completePopUp(PopUpBase popup = null, string action ="", bool deActivate = true)
	{
		if(popup != null && deActivate)
		{
			openPopUps.Remove (popup);
		}

		if(OnPopUpCompleted != null)
		{
			OnPopUpCompleted(action);
		}
	}

	public bool isPopUpOpen(string name)
	{
		PopUpBase popUp = getPopupByName (name);

		if(popUp != null)
		{
			for(int i=0; i<openPopUps.Count; i++)
			{
				if(openPopUps[i] == popUp)
				{
					return true;
				}
			}
		}
		return false;
	}
}
