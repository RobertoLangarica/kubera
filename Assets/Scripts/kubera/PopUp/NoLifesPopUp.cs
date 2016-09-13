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

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_LIFES_POPUP_TITLE);

		lifeTimer.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_LIFES_POPUP_INFO1);
		descriptionText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_LIFES_POPUP_INFO2);
		askButton.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_LIFES_POPUP_BUTTON1);
		rechargeButton.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_LIFES_POPUP_BUTTON2);

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
		if (TransactionManager.GetInstance ().tryToUseGems (price)) 
		{
			LifesManager.GetInstance ().giveLifesToUser ((KuberaDataManager.GetInstance () as KuberaDataManager).initialLifes);

			popUp.SetActive (false);
			OnComplete ("needLifes");
		} 
		else 
		{
			Debug.Log ("Fondos insuficientes");
		}
	}
}
