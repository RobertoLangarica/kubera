using UnityEngine;
using System.Collections;

public class TransactionManager : MonoBehaviour 
{
	public static TransactionManager instance = null;

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
		if(UserDataManager.instance.playerGems >= gemsPrice)
		{
			return true;
		}
		return false;
	}

	public bool tryToUseGems(int gems)
	{
		if (TransactionManager.instance.checkIfExistEnoughGems (gems)) 
		{
			UserDataManager.instance.giveGemsToPlayer (-gems);
			return true;
		}

		Debug.Log ("NOt enough gems on players acount");
		return false;
	}
}