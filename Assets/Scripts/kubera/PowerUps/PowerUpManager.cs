using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EPOWERUPS
{
	DESTROY_ALL_COLOR_POWERUP,
	WILDCARD_POWERUP,
	ROTATE_POWERUP,
	BLOCK_POWERUP,
	DESTROY_NEIGHBORS_POWERUP
}

public class PowerUpManager : MonoBehaviour 
{
	public List<PowerUpBase2> powersUpOnEditor = new List<PowerUpBase2>();

	public PowerUpBase2 getPowerUp(EPOWERUPS powerUp)
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

	public void activateAvailablePowers()
	{
		//UserDataManager.instance.rotatePowerUpAvailable= false;
		getPowerUp(EPOWERUPS.ROTATE_POWERUP).gameObject.SetActive(UserDataManager.instance.rotatePowerUpAvailable);

		//UserDataManager.instance.onePiecePowerUpAvailable = false;
		getPowerUp(EPOWERUPS.BLOCK_POWERUP).gameObject.SetActive(UserDataManager.instance.onePiecePowerUpAvailable);

		//UserDataManager.instance.destroyPowerUpAvailable = false;
		getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP).gameObject.SetActive(UserDataManager.instance.destroyPowerUpAvailable);

		//UserDataManager.instance.destroyNeighborsPowerUpAvailable = false;
		getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP).gameObject.SetActive(UserDataManager.instance.destroyNeighborsPowerUpAvailable);

		//UserDataManager.instance.wildCardPowerUpAvailable = false;
		getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gameObject.SetActive(UserDataManager.instance.wildCardPowerUpAvailable);

	}

	public void activatePower(EPOWERUPS powerUp)
	{
		switch(powerUp)
		{
		case(EPOWERUPS.ROTATE_POWERUP):
			getPowerUp(EPOWERUPS.ROTATE_POWERUP).gameObject.SetActive(true);
			break;

		case(EPOWERUPS.BLOCK_POWERUP):
			getPowerUp(EPOWERUPS.BLOCK_POWERUP).gameObject.SetActive(true);
			break;

		case(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP):
			getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP).gameObject.SetActive(true);
			break;

		case(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP):
			getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP).gameObject.SetActive(true);
			break;

		case(EPOWERUPS.WILDCARD_POWERUP):
			getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gameObject.SetActive(true);
			break;
		}
	}
}