using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour 
{
	public List<PowerupBase> powerups;

	public bool allowPowerUps;

	public delegate void DPowerUpNotification(PowerupBase.EType type);
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;

	protected PowerupBase powerup;

	void Start()
	{
		foreach(PowerupBase powerup in powerups)
		{
			if (powerup != null) {
				powerup.OnPowerupCanceled += cancelPowerup;
				powerup.OnPowerupCompleted += completePowerup;
			}
		}
	}

	public void activatePowerUp(PowerupBase.EType wichOne)
	{
		if (allowPowerUps) 
		{
			powerup = getPowerupByType (wichOne);

			if (powerup == null) {
				cancelPowerup ();
			} else {
				powerup.activate ();
			}
		}
	}

	public PowerupBase getPowerupByType(PowerupBase.EType type)
	{
		foreach(PowerupBase powerup in powerups)
		{
			if (powerup != null) 
			{
				if (powerup.type == type) {
					return powerup;
				}
			}
		}

		return null;
	}

	private void cancelPowerup()
	{
		if(OnPowerupCanceled != null)
		{
			OnPowerupCanceled(powerup.type);
		}
	}

	private void completePowerup()
	{
		if(OnPowerupCompleted != null)
		{
			OnPowerupCompleted(powerup.type);
		}
	}

	public PowerupBase.EType getCurrentPowerUpType()
	{
		return powerup.type;
	}

	public void cancelCurrentPowerUp()
	{
		powerup.cancel ();
	}
}
