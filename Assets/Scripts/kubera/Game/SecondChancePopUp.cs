using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecondChancePopUp : PopUpBase 
{
	public delegate void DSecondChanceNotification();

	public DSecondChanceNotification OnSecondChanceAquired;

	public int secondChanceMovements = 5;
	public int secondChanceBombs = 2;
	public Text price;

	protected int secondChanceTimes = 0;
	protected int secondChancePrice;

	public override void activate()
	{
		popUp.SetActive (true);

		setPrice ();
	}

	protected void setPrice()
	{
		switch(secondChanceTimes)
		{
		case(0):
			secondChancePrice = 10;
			break;
		case(1):
			secondChancePrice = 15;
			break;
		case(2):
			secondChancePrice = 20;
			break;
		default:
			secondChancePrice = 30;
			break;
		}

		price.text = secondChancePrice.ToString ();
	}

	protected void buyASecondChance()
	{
		if(TransactionManager.instance.tryToUseGems(secondChancePrice))
		{
			secondChanceTimes++;

			if (OnSecondChanceAquired != null) 
			{
				OnSecondChanceAquired ();
			}
		}

		Debug.Log("Fondos insuficientes");
	}
}
