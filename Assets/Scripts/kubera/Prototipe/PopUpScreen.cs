using UnityEngine;
using System.Collections;

public class PopUpScreen : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		FindObjectOfType<PopUp>().redBDelegate += goToIntro;
		FindObjectOfType<PopUp>().greenBDelegate += goToGame;

		FindObjectOfType<PopUp>().showUp();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void goToIntro()
	{
		//ScreenManager.instance.GoToScene("Intro");
		FindObjectOfType<PopUp>().closePopUp();
		ScreenManager.GetInstance().GoToSceneAsync("Intro",0.5f);
	}

	protected void goToGame()
	{
		//ScreenManager.instance.GoToScene("Game");
		FindObjectOfType<PopUp>().closePopUp();
		ScreenManager.GetInstance().GoToSceneAsync("Game",0.5f);
	}
}