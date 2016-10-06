﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kubera.Data;
using utils.gems;

public class BossLocked : PopUpBase {

	public PopUpManager popUpManager;

	public MapManager mapManager;
	public Text bossLockedUnlockText;
	public Text bossLockedOptionText;
	public Text starsText;
	public Text friendsText;
	public Text gemsText;

	public Text starsNumber;
	public Text gemsNumber;

	[HideInInspector]public int gemsNeeded;
	[HideInInspector]public string lvlName;
	[HideInInspector]public string fullLvlName;

	void Start()
	{
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

		friendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.BOSS_LOCKED_KEY_TEXT).Replace ("{{keyNumber}}",friendsNeeded.ToString ());
		gemsNeeded = gems;
	}

	public void facebookHelp()
	{
		OnComplete ("askKeys");
	}

	public void gemsCharge()
	{
		print (lvlName);
		if(GemsManager.GetCastedInstance<GemsManager>().isPossibleToConsumeGems(gemsNeeded))
		{
			GemsManager.GetCastedInstance<GemsManager>().tryToConsumeGems(gemsNeeded);
			mapManager.unlockBoss (fullLvlName);
			closePressed ();	
		}
		else
		{
			//TODO: abrir popUp de enviar a shopika
		}
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnComplete ();
	}
}
