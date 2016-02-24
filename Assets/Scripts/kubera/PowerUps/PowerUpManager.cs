using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupManager : MonoBehaviour 
{
	public List<PowerupBase> powerups;

	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;

	void Start()
	{
		foreach(PowerupBase powerup in powerups)
		{
			//powerup.OnPowerupCanceled += OnPowerupCanceled;
			//powerup.OnPowerupCompleted += OnPowerupCompleted;
		}
	}

	public void activatePowerUp(PowerupBase.EType wichOne)
	{
		PowerupBase powerup = getPowerupByType(wichOne);

		if(powerup != null)
		{
			OnPowerupCanceled();
		}
		else
		{
			powerup.activate();
		}
	}

	private PowerupBase getPowerupByType(PowerupBase.EType type)
	{
		foreach(PowerupBase powerup in powerups)
		{
			if(powerup.type == type)
			{
				return powerup;
			}
		}

		return null;
	}
}
