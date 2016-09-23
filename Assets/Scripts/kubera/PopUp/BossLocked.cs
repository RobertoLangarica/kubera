using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kubera.Data;

public class BossLocked : PopUpBase {

	protected PopUpManager popUpManager;

	public Text bossLockedUnlockText;
	public Text bossLockedOptionText;
	public Text starsText;
	public Text friendsText;
	public Text gemsText;

	public Text starsNumber;
	public Text gemsNumber;

	[HideInInspector]public int gemsNeeded;
	[HideInInspector]public string lvlName;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();

		bossLockedOptionText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.BOSS_LOCKED_OPTION_TEXT);
		starsText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.BOSS_LOCKED_STAR_TEXT);
		gemsText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.BOSS_LOCKED_GEM_TEXT);
	}

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void initializeValues(int friendsNeeded,int gems,int starsNeeded, string levelNumber)
	{
		bossLockedUnlockText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.BOSS_LOCKED_UNLOCK_TEXT).Replace ("{{level}}",levelNumber);
		starsNumber.text = (DataManagerKubera.GetInstance () as DataManagerKubera).getAllEarnedStars ().ToString() + " / " + starsNeeded.ToString();
		gemsNumber.text = gems.ToString ();

		friendsText.text =MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.BOSS_LOCKED_KEY_TEXT).Replace ("{{keyNumber}}",friendsNeeded.ToString ());
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
