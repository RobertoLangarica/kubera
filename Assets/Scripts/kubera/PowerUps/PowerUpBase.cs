using UnityEngine;
using System.Collections;

public class PowerupBase : MonoBehaviour 
{
	public EType type;

	public enum EType
	{
		BOMB,BLOCK,ROTATE,DESTROY,WILDCARD	
	}

	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;

	public virtual void activate()
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
