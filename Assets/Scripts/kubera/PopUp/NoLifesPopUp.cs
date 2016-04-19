using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NoLifesPopUp : PopUpBase 
{
	public Text title;
	public Text lifeTimer;
	public Text flowText;
	public Text askButton;
	public Text rechargeButton;

	public Text price;

	public override void activate()
	{
		popUp.SetActive (true);

		title.text = "te quedaste sin vidas";

		lifeTimer.text = "Seguro lo lograras";
		flowText.text = "Seguro lo lograras";
		askButton.text = "Seguro lo lograras";
		rechargeButton.text = "Seguro lo lograras";

		price.text = "Seguro lo lograras";
	}

	public void closePressed()
	{}

	public void askForLifes()
	{}

	public void rechargeLifes()
	{}
}
