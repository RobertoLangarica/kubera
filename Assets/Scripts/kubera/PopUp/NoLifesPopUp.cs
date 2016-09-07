using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Kubera.Data;

public class NoLifesPopUp : PopUpBase 
{
	public Text title;
	public Text lifeTimer;
	public Text descriptionText;
	public Text askButton;
	public Text rechargeButton;

	public Text priceText;

	protected int price = 10;

	public override void activate()
	{
		popUp.SetActive (true);


		title.text = "te quedaste sin vidas";

		lifeTimer.text = "Seguro lo lograras";
		descriptionText.text = "Seguro lo lograras";
		askButton.text = "Seguro lo lograras";
		rechargeButton.text = "Seguro lo lograras";

		priceText.text = price.ToString ();
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnPopUpCompleted (this);
		if (SceneManager.GetActiveScene ().name != "Levels") 
		{
			SceneManager.LoadScene ("Levels");
		}
	}

	public void askForLifes()
	{
		//TODO:Hacer stuff de facebook
	}

	public void rechargeLifes()
	{
		if(TransactionManager.GetInstance().tryToUseGems(price))
		{
			(KuberaDataManager.GetInstance () as KuberaDataManager).giveUserLifes (
				(KuberaDataManager.GetInstance () as KuberaDataManager).currentUser.maximumLifes);

			LifesManager.GetInstance().takeALife ();

			popUp.SetActive (false);
			OnComplete ("needLifes");
		}

		Debug.Log("Fondos insuficientes");
	}
}
