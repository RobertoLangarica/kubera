using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kubera.Data;
using utils.gems;
using DG.Tweening;

public class BossLocked : PopUpBase {

	public PopUpManager popUpManager;

	public MapManager mapManager;
	public Text bossLockedUnlockText;
	public Text bossLockedOptionText;
	public Text unblockedText;
	public Text starsText;
	public Text friendsText;
	public Text gemsText;

	public Text starsNumber;
	public Text gemsNumber;

	public GameObject optionPanel;
	public RectTransform upLock;
	public Image upLockImage;
	public Image downLockImage;

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
		if(ShopikaManager.GetCastedInstance<ShopikaManager>().isPossibleToConsumeGems(gemsNeeded))
		{
			ShopikaManager.GetCastedInstance<ShopikaManager>().tryToConsumeGems(gemsNeeded);
			OnComplete ();
			mapManager.unlockBoss (fullLvlName);
		}
		else
		{
			OnComplete ("notMoney",false);
		}
	}

	public void unlockAnimation()
	{
		optionPanel.SetActive (false);
		bossLockedUnlockText.gameObject.SetActive (false);
		bossLockedOptionText.gameObject.SetActive (false);
		unblockedText.gameObject.SetActive (true);
		unblockedText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.BOSS_UNLOCKED_TEXT);
		upLock.DOLocalRotate (new Vector3 (0, 0, 45), 1.3f).OnComplete(()=>{OnComplete("afterBossAnimation");});
		/*upLockImage.DOFade (0, 1).SetDelay(0.75f);
		downLockImage.DOFade (0, 2).SetDelay(0.75f).OnComplete(()=>{OnComplete("afterBossAnimation");});*/
		OnComplete ("",false);
	}

	public void closePressed()
	{
		popUp.SetActive (false);
		OnComplete ();
	}
}
