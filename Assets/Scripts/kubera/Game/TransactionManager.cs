using UnityEngine;
using System.Collections;

public class TransactionManager : MonoBehaviour 
{
	public static TransactionManager instance = null;

	protected bool isTest = true;

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
}