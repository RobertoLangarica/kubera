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
	public Transform powerUpButton;

	public bool isFree;

	public virtual void activate()
	{
	}

	public virtual void cancel()
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
