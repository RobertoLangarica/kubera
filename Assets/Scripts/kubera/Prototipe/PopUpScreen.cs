using UnityEngine;
using System.Collections;

public class PopUpScreen : MonoBehaviour 
{
	PopUp popup;
	// Use this for initialization
	void Start () 
	{
		popup = FindObjectOfType<PopUp>();
		popup.redBDelegate += goToIntro;
		popup.greenBDelegate += goToGame;

		popup.showUp();
	}

	protected void goToIntro()
	{
		//ScreenManager.instance.GoToScene("Intro");
		popup.closePopUp();
		//ScreenManager.GetInstance().GoToSceneAsync("Intro",0.5f);
		ScreenManager.GetInstance().GoToSceneAsync("Intro");
	}

	protected void goToGame()
	{
		//ScreenManager.instance.GoToScene("Game");
		popup.closePopUp();
		ScreenManager.GetInstance().GoToSceneAsync("Game");
	}
}