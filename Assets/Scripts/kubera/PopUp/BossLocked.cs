using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kubera.Data;

public class BossLocked : PopUpBase {

	protected PopUpManager popUpManager;

	public Text friendsText;
	public Text starsText;
	public Text starsNumber;
	public Text gemsNumber;
	public Text gemsText;

	[HideInInspector]public int gemsNeeded;
	[HideInInspector]public string lvlName;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
	}

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void initializeValues(int friendsNeeded,int gems,int starsNeeded)
	{
		//TODO: estrellas
		starsNumber.text = (LevelsDataManager.GetInstance () as LevelsDataManager).getAllEarnedStars ().ToString() + " / " + starsNeeded.ToString();
		gemsNumber.text = gems.ToString ();
		friendsText.text = friendsNeeded.ToString ();
		gemsNeeded = gems;
	}

	public void facebookHelp()
	{
		popUpManager.activatePopUp ("fbFriendsRequestPanel");
	}

	public void gemsCharge()
	{
		//TODO: abrir popUp de enviar a shopika
		if (TransactionManager.GetInstance().tryToUseGems (gemsNeeded)) 
		{
			FindObjectOfType<MapManager> ().unlockBoss (lvlName);
			closePressed ();
		}
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnComplete ();
	}
}
