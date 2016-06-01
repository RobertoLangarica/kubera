using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossLocked : PopUpBase {

	protected PopUpManager popUpManager;

	public Text friendsText;
	public Text starsText;
	public Text starsNumber;
	public Text gemsNumber;
	public Text gemsText;

	[HideInInspector]public int starsNeeded;
	[HideInInspector]public int stars;
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

	public void initializeValues(int friendsNeeded,int gemsNeeded,int starsNeeded)
	{
		//TODO: estrellas
		starsNumber.text = starsNeeded.ToString() + " / " + "200";
		gemsNumber.text = gemsNeeded.ToString ();
	}

	public void facebookHelp()
	{
		popUpManager.activatePopUp ("fbFriendsRequestPanel");
	}

	public void gemsCharge()
	{
		//TODO: abrir popUp de enviar a shopika
		if (TransactionManager.instance.tryToUseGems (gemsNeeded)) 
		{
			FindObjectOfType<MapManager> ().unlockBoss (lvlName);
		}
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnComplete ();
	}
}
