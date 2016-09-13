using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MissingLifesPopUp : PopUpBase 
{
	public Text title;
	public Text lifeTimer;
	public Text descriptionText;
	public Text askButton;

	public override void activate()
	{
		popUp.SetActive (true);

		title.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.MISSING_LIFES_POPUP_TITLE);

		lifeTimer.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.MISSING_LIFES_POPUP_INFO1);
		descriptionText.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.MISSING_LIFES_POPUP_INFO2);
		askButton.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.MISSING_LIFES_POPUP_BUTTON);
	}

	public void closePressed()
	{
		//popUp.SetActive (false);
		OnComplete ();
	}

	public void askForLifes()
	{
		//TODO: hacer animacion de cerrado
		//popUp.SetActive (false);
		//cuando se cierre hacer stuff de facebook
		//TODO:Hacer stuff de facebook
		OnComplete ("needLifes");
	}
}
