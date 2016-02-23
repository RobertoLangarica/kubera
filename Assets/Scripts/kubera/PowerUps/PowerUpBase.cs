using UnityEngine;
using System.Collections;

public class PowerupBase : MonoBehaviour 
{
	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;


	/**
	 * Activación e inicialización del powerup
	 **/ 
	public abstract void activate()
	{
		
	}


	protected void OnComplete()
	{
		if(OnPowerupCompleted != null)
		{
			OnPowerupCompleted();
		}	
	}

	protected void OnCancel()
	{
		if(OnPowerupCanceled != null)
		{
			OnPowerupCanceled();
		}	
	}
}
