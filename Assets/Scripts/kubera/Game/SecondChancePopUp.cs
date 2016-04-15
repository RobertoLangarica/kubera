using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecondChancePopUp : PopUpBase 
{
	public delegate void DSecondChanceNotification();

	public DSecondChanceNotification OnSecondChanceAquired;

	public int movements = 5;
	public int bombs = 2;
	public Text priceText;
	public Text movementsText;
	public Text bombsText;

	public GameObject discountDisplay;

	protected int secondChanceTimes = 0;
	protected int price;

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
			price = 10;
			break;
		case(1):
			price = 15;
			break;
		case(2):
			price = 20;
			break;
		default:
			price = 30;
			break;
		}

		checkDiscount ();
	}

	protected void checkDiscount()
	{
		if (!TransactionManager.instance.checkIfExistEnoughGems (price)) 
		{
			price = (int)(price * 0.5f);
		}
		priceText.text = price.ToString ();
	}

	public void buyASecondChance()
	{
		if(TransactionManager.instance.tryToUseGems(price))
		{
			secondChanceTimes++;

			if (OnSecondChanceAquired != null) 
			{
				OnSecondChanceAquired ();
			}

			popUp.SetActive (false);
		}

		Debug.Log("Fondos insuficientes");
	}

	public void giveUp()
	{
		popUp.SetActive (false);
	}
}
