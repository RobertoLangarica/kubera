using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecondChancePopUp : PopUpBase 
{
	public delegate void DSecondChanceNotification();

	public DSecondChanceNotification OnSecondChanceAquired;
	public DSecondChanceNotification OnGiveUp;

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

		movementsText.text = "+" + movements.ToString ();
		bombsText.text = bombs.ToString ();

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
		//Debug.Log (UserDataManager.instance.playerGems);
		if (!TransactionManager.instance.checkIfExistEnoughGems (price)) 
		{
			price = (int)(price * 0.5f);
			discountDisplay.SetActive (true);
		}
		priceText.text = price.ToString ();
	}

	public void buyASecondChance()
	{
		Debug.Log (UserDataManager.instance.playerGems);
		if(TransactionManager.instance.tryToUseGems(price))
		{
			secondChanceTimes++;

			if (OnSecondChanceAquired != null) 
			{
				OnSecondChanceAquired ();
			}

			popUp.SetActive (false);
			OnComplete ();

			Debug.Log (UserDataManager.instance.playerGems);

			OnComplete ();
		}

		Debug.Log("Fondos insuficientes");
	}

	public void giveUp()
	{
		popUp.SetActive (false);

		if (OnGiveUp != null) 
		{
			OnGiveUp ();
		}
			
		OnComplete ();
	}
}
