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
	public DPowerUpNotification OnPowerupUsed;
	public DPowerUpNotification OnPowerupCompletedNoGems;

	protected PowerupBase powerup;

	void Start()
	{
		allowPowerUps = true;

		foreach(PowerupBase powerup in powerups)
		{
			if (powerup != null) {
				powerup.OnPowerupCanceled += cancelPowerup;
				powerup.OnPowerupCompleted += completePowerup;
				powerup.OnPowerupUsed += usedPowerup;
				powerup.OnPowerupCompletedNoGems += completePowerupNoGems;
				powerup.priceText.text = getPowerUpPrice (powerup.type).ToString();
			}
		}
	}

	public void activatePowerUp(PowerupBase.EType wichOne,bool canUse)
	{
		if (allowPowerUps) 
		{
			powerup = getPowerupByType (wichOne);

			if (powerup == null) 
			{
				cancelPowerup ();
			} 
			else 
			{
				powerup.activate(canUse);
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

	private void completePowerupNoGems()
	{
		if(OnPowerupCompletedNoGems != null)
		{
			OnPowerupCompletedNoGems(powerup.type);
		}
	}

	private void usedPowerup()
	{
		if(OnPowerupUsed != null)
		{
			OnPowerupUsed(powerup.type);
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

	public int getPowerUpPrice(PowerupBase.EType powerUptype)
	{
		switch (powerUptype) {
		case PowerupBase.EType.HINT_WORD:
			return 	5;
			case PowerupBase.EType.BLOCK:
			return 	30;
			case PowerupBase.EType.BOMB: 
			return 	15;
			case PowerupBase.EType.DESTROY:
			return 	70;
			case PowerupBase.EType.ROTATE:
			return 	50;
			case PowerupBase.EType.WILDCARD:
			return 	100;
		}

		return 0;
	}
}
