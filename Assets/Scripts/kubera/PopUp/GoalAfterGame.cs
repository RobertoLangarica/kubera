using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data.Sync;

public class GoalAfterGame : PopUpBase {

	public Text LevelNumberUnit;
	public Text LevelNumberDecimal;
	public Text LevelNumberHundred;
	public Text LevelText;
	public Text LifesText;
	protected string levelName;

	public Text PointsUpText;
	public Text Points;
	public Text inviteFriendsText;
	public Text playText;

	public Text feedback;

	public Image starsBarr;
	public GameObject[] stars;
	public GameObject[] balls;

	public float speedShowStars = 0.5f;

	public RectTransform leftDoor;
	public RectTransform rightDoor;
	public RectTransform topLevel;
	public RectTransform starPanel;
	public RectTransform objetives;
	public RectTransform play;
	public RectTransform close;
	public RectTransform facebookFriends;
	public RectTransform facebookInvite;
	public RectTransform icon;
	public RectTransform share;

	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public Transform goalPopUpSlotsParent;
	public GridLayoutGroup FriendsgridLayoutGroup;

	public Image shareImage;

	protected bool pressed;

	public ShareScore shareScore;
	protected int starsObtained;

	public Sprite iosShare;
	public Sprite androidShare;

	void Start()
	{
		//TODO checar login a facebook
		fbLogin ();
		//FBLoggin.GetInstance().onLoginComplete += fbLogin;

		setStartingPlaces ();
		FriendsgridLayoutGroup.cellSize = new Vector2 (Screen.width * 0.225f, Screen.height * 0.15f);

		inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_FACEBOOK);
		playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_NEXT);
		PointsUpText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_POINTS_UPER);

		LevelText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVES_NAME_TEXT_ID);

		feedback.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FEEDBACK_TEXT);

		LifesText.text = LifesManager.GetInstance ().currentUser.playerLifes.ToString ();

		#if UNITY_ANDROID
		shareImage.sprite = androidShare;
		#elif UNITY_IOS
		shareImage.sprite = iosShare;
		#endif
	}

	protected void fbLogin()
	{
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_FACEBOOK);
		}
		else
		{
			inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_CONNECT_FACEBOOK);
		}
	}

	public override void activate()
	{
		popUp.SetActive (true);
		Invoke ("startAnimation", 0.25f);

	}

	public void setGoalPopUpInfo(int starsReached, string levelName, string points,int currentWorld =0)
	{
		PersistentData.GetInstance().startLevel--;
		char[] lvl;

		this.levelName = levelName;
		switch (levelName.Length) 
		{
		case 1:
			LevelNumberUnit.text = levelName;
			LevelNumberDecimal.gameObject.SetActive (false);
			LevelNumberHundred.gameObject.SetActive (false);
			break;
		case 2:
			lvl = levelName.ToCharArray();
			LevelNumberUnit.text = lvl[1].ToString();
			LevelNumberDecimal.text = lvl[0].ToString();
			LevelNumberHundred.gameObject.SetActive (false);
			break;
		case 3:
			lvl = levelName.ToCharArray();
			LevelNumberUnit.text = lvl[2].ToString();
			LevelNumberDecimal.text = lvl[1].ToString();
			LevelNumberHundred.text = lvl[0].ToString();
			break;                                                                  
		}

		this.Points.text = points +" "+ MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_POINTS);

		starsObtained = starsReached;
		resetStars ();
		showStars (starsReached);
	}

	protected void showStars(int starsReached)
	{
		starsBarr.fillAmount = 0.06f;

		switch (starsReached) 
		{
		case 1:
			stars [0].SetActive(true);
			balls [0].SetActive(true);
			starsBarr.fillAmount = 0.349f;
			break;
		case 2:
			stars [0].SetActive(true);
			stars [1].SetActive(true);
			balls [0].SetActive(true);
			balls [1].SetActive(true);
			starsBarr.fillAmount = 0.604f;
			break;
		case 3:
			stars [0].SetActive(true);
			stars [1].SetActive(true);
			stars [2].SetActive(true);
			balls [0].SetActive(true);
			balls [1].SetActive(true);
			balls [2].SetActive(true);
			balls [3].SetActive(true);
			starsBarr.fillAmount = 1;
			break;
		}
	}

	protected void resetStars()
	{
		for(int i=0; i<stars.Length; i++)
		{
			stars [i].SetActive (false);
		}

		for(int i=0; i<balls.Length; i++)
		{
			balls [i].SetActive (false);
		}
	}

	public void playGame()
	{
		if(pressed)
		{
			return;
		}
		pressed = true;

		soundButton ();

		setStartingPlaces ();
		OnComplete ("continue");
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
	}

	public void retryGame()
	{
		if(pressed)
		{
			return;
		}
		pressed = true;

		soundButton ();

		OnComplete ("retry");
	}

	public void exit ()
	{
		soundButton ();

		setStartingPlaces ();
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
		OnComplete ("closeObjective");
	}

	public void sharingScore()
	{
		shareScore.sharePassedLevel (Points.text,starsObtained,levelName);
	}

	protected void setStartingPlaces()
	{

		/*leftDoor.localScale = new Vector3 (1, 1.4f);
		rightDoor.localScale = new Vector3 (1, 1.4f);*/
		topLevel.anchoredPosition = new Vector3 (0, topLevel.rect.height*2, 0);
		leftDoor.anchoredPosition = new Vector3 (-leftDoor.rect.width, 0, 0);
		rightDoor.anchoredPosition = new Vector3 (rightDoor.rect.width, 0, 0);
		starPanel.localScale = Vector2.zero;
		objetives.localScale = Vector2.zero;
		play.localScale = Vector2.zero;
		share.localScale = Vector2.zero;
		close.localScale = Vector2.zero;
		facebookFriends.localScale = Vector2.zero;
		facebookInvite.localScale = Vector2.zero;
		//starPanel.anchoredPosition = new Vector3 (0, starPanel.rect.height*2, 0);
		//facebookInvite.anchoredPosition = new Vector3 (0, -facebookInvite.rect.height*2, 0);
		icon.transform.localScale = Vector2.zero;
		this.transform.localScale = new Vector2(2,2);
	}

	protected void startAnimation()
	{
		float fullTime = 0.5f;
		float quarter = fullTime * 0.25f;
		float tenth = fullTime * 0.1f;

		leftDoor.DOAnchorPos (Vector2.zero, quarter);
		rightDoor.DOAnchorPos (Vector2.zero, quarter).OnComplete(()=>
			{
				topLevel.DOAnchorPos (Vector2.zero, quarter);
				facebookInvite.DOAnchorPos (Vector2.zero, quarter);

				this.transform.DOScale(new Vector2(1,1),quarter).OnComplete(()=>
					{
						close.DOScale(new Vector2(1,1),tenth);
						starPanel.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
							{
								icon.DOScale(new Vector2(1,1),tenth);
								objetives.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
									{
										share.DOScale(new Vector2(1,1),tenth);
										play.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
											{
												facebookFriends.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
													{
														LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(this.LevelText.text,slotParent);
														leaderboard.showSlots(true);

														scrollRect.horizontalNormalizedPosition = 0;

														facebookInvite.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
															{
															});
													});
											});
									});
							});

					});
			});
	}

	protected void soundButton()
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}
	}

	protected void winStar()
	{
		if(AudioManager.GetInstance())
		{
			//TODO: este audio no existe, pero existen star1, star2 y star3, que show?
			AudioManager.GetInstance().Stop("star");
			AudioManager.GetInstance().Play("star");
		}
	}

	public void fbAction()
	{
		soundButton ();
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_FACEBOOK);
			FacebookManager.GetInstance ().requestNewFriends ();
		}
		else
		{
			inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.AFTERGAME_POPUP_CONNECT_FACEBOOK);
			KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookLogin();
		}
	}
}
