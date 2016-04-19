using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RetryPopUp : PopUpBase
{
	public Text title;
	public Text content;

	public override void activate()
	{
		popUp.SetActive (true);

		title.text = "Nivel" + PersistentData.instance.levelNumber;

		content.text = "Seguro lo lograras";
	}

	public void closeRetry()
	{
		OnPopUpCompleted ();
	}

	public void retryLevel()
	{
		OnPopUpCompleted ();
		SceneManager.LoadScene ("Game");
	}
}