using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPOWERUPS
{
	DESTROY_POWERUP,
	WILDCARD_POWERUP,
	ROTATE_POWERUP,
	BLOCK_POWERUP,
	DESTROY_NEIGHBORS_POWERUP
}

public class PowerUpManager : MonoBehaviour 
{
	public List<PowerUpBase> powersUpOnEditor = new List<PowerUpBase>();

	public void giveUsesToPowerUP(EPOWERUPS powerUp,int uses = 1)
	{
		PowerUpBase selectePower = getPowerUp(powerUp);

		if(selectePower != null)
		{
			selectePower.uses++;
			selectePower.numberUses.text = uses.ToString ();
			switch(powerUp)
			{
			case(EPOWERUPS.BLOCK_POWERUP):
			{
				UserDataManager.instance.onePiecePowerUpUses++;
			}
				break;
			case(EPOWERUPS.DESTROY_POWERUP):
			{
				UserDataManager.instance.destroyPowerUpUses++;
			}
				break;
			case(EPOWERUPS.ROTATE_POWERUP):
			{
				UserDataManager.instance.rotatePowerUpUses++;
			}
				break;
			case(EPOWERUPS.WILDCARD_POWERUP):
			{
				UserDataManager.instance.wildCardPowerUpUses++;
			}
				break;
			}
		}
	}

	protected PowerUpBase getPowerUp(EPOWERUPS powerUp)
	{
		for(int i = 0;i < powersUpOnEditor.Count;i++)
		{
			if(powersUpOnEditor[i].typeOfPowerUp == powerUp)
			{
				return powersUpOnEditor[i];
			}
		}
		return null;
	}
}