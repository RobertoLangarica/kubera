using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Kubera.Data;
using utils.gems;

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
		OnComplete ("noLifesClose");
		if (SceneManager.GetActiveScene ().name != "Levels") 
		{
			SceneManager.LoadScene ("Levels");
		}
	}

	public void askForLifes()
	{
		//TODO:Hacer stuff de facebook
		OnComplete ("needLifes");
	}

	public void rechargeLifes()
	{
		if(ShopikaManager.GetCastedInstance<ShopikaManager>().isPossibleToConsumeGems(price))
		{
			ShopikaManager.GetCastedInstance<ShopikaManager>().tryToConsumeGems(price);
			LifesManager.GetInstance ().giveLifesToUser (LifesManager.GetInstance().maximumLifes);

			KuberaAnalytics.GetInstance ().registerGemsUsedOnLifes (PersistentData.GetInstance().lastLevelReachedName);

			popUp.SetActive (false);
			OnComplete ();
		}
		else 
		{
			//TODO mandar a shopika o mostrar popus de gemas o algo
			Debug.Log ("Fondos insuficientes");
		}
	}
}
