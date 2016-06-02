using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RetryPopUp : PopUpBase
{
	public Text title;
	public Text content;

	protected PopUpManager popUpManager;
	protected LifesManager lifesManager;

	public override void activate()
	{
		popUp.SetActive (true);

		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesManager = FindObjectOfType<LifesManager> ();

		PersistentData.GetInstance().startLevel--;
		title.text = "Nivel" + PersistentData.GetInstance().startLevel;

		content.text = "Seguro lo lograras";
	}

	public void closeRetry()
	{
		OnPopUpCompleted (this);
		SceneManager.LoadScene ("Levels");
	}

	public void retryLevel()
	{
		if (UserDataManager.instance.playerLifes > 0) 
		{
			OnPopUpCompleted (this);

			//lifesManager.takeALife ();

			SceneManager.LoadScene ("Game");
		} 
		else 
		{
			popUpManager.activatePopUp ("NoLifesPopUp");
		}
	}
}