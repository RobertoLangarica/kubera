using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FullLifesPopUp : PopUpBase 
{
	public Text title;
	public Text descriptionText;
	public Text askButton;

	public override void activate()
	{
		popUp.SetActive (true);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FULL_LIFES_POPUP_TITLE);
		descriptionText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FULL_LIFES_POPUP_INFO);
		askButton.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FULL_LIFES_POPUP_BUTTON);
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnComplete ();
	}

	public void askForLifes()
	{
		//TODO: hacer animacion de cerrado
		popUp.SetActive (false);
		//cuando se cierre hacer stuff de facebook
		//TODO:Hacer stuff de facebook
		OnComplete ("needLifes");
	}
}
