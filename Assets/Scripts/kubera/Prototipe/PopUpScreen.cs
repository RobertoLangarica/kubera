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
		ScreenManager.instance.GoToScene("Intro");
	}

	protected void goToGame()
	{
		ScreenManager.instance.GoToScene("Game");
	}
}