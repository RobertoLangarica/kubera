using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NoLifesPopUp : PopUpBase 
{
	public Text title;
	public Text lifeTimer;
	public Text flowText;
	public Text askButton;
	public Text rechargeButton;

	public Text priceText;

	protected int price = 10;
	protected LifesManager lifesManager;

	public override void activate()
	{
		popUp.SetActive (true);

		lifesManager = FindObjectOfType<LifesManager> ();

		title.text = "te quedaste sin vidas";

		lifeTimer.text = "Seguro lo lograras";
		flowText.text = "Seguro lo lograras";
		askButton.text = "Seguro lo lograras";
		rechargeButton.text = "Seguro lo lograras";

		priceText.text = "Seguro lo lograras";
	}

	public void closePressed()
	{
		OnPopUpCompleted ();
		SceneManager.LoadScene ("Levels");
	}

	public void askForLifes()
	{
		//TODO:Hacer stuff de facebook
	}

	public void rechargeLifes()
	{
		if(TransactionManager.instance.tryToUseGems(price))
		{
			UserDataManager.instance.giveLifeToPlayer (UserDataManager.instance.maximumLifes);

			lifesManager.takeALife ();

			popUp.SetActive (false);
			OnComplete ();
		}

		Debug.Log("Fondos insuficientes");
	}
}
