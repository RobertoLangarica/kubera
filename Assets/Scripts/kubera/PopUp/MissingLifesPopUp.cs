﻿using UnityEngine;
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

		title.text = "te quedaste sin vidas";

		lifeTimer.text = "Seguro lo lograras";
		descriptionText.text = "Seguro lo lograras";
		askButton.text = "Seguro lo lograras";
	}

	public void closePressed()
	{
		OnPopUpCompleted ();
	}

	public void askForLifes()
	{
		//TODO:Hacer stuff de facebook
	}
}
