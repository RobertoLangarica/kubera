using UnityEngine;
using System.Collections;

public class TransactionManager : MonoBehaviour 
{
	public static TransactionManager instance = null;

	protected bool isTest = false;

	void Awake()
	{
		instance = this;
	}

	//TODO: quien se traiga datos del usuario setearlos a local aqui
	public void setCurrentGemsToUserData(int gems)
	{
		UserDataManager.instance.setGems(gems);
	}

	public bool checkIfExistEnoughGems(int gemsPrice)
	{
		if (isTest) 
		{
			return true;
		}

		if(UserDataManager.instance.playerGems >= gemsPrice)
		{
			return true;
		}
		return false;
	}

	public bool tryToUseGems(int gems)
	{
		if (isTest) 
		{
			return true;
		}
	
		if (TransactionManager.instance.checkIfExistEnoughGems (gems)) 
		{
			UserDataManager.instance.giveGemsToPlayer (-gems);
			return true;
		}

		Debug.LogWarning ("NOt enough gems on players acount");
		return false;
	}

	public int powerUpPrices(PowerupBase.EType powerUptype)
	{
		switch (powerUptype) {
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