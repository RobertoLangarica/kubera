using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Kubera.Data;
using Kubera.Data.Sync;

public class RetryPopUp : PopUpBase
{
	public Text LevelNumberUnit;
	public Text LevelNumberDecimal;
	public Text LevelNumberHundred;
	public Text LevelText;

	public Text inviteFriendsText;
	public Text playText;

	public Text feedback;

	public RectTransform leftDoor;
	public RectTransform rightDoor;
	public RectTransform topLevel;
	public RectTransform retry;
	public RectTransform closeButton;
	public RectTransform facebookFriends;
	public RectTransform facebookInvite;
	public RectTransform icon;
	public RectTransform ContentRT;
	public Text content;
	public RectTransform leftHeart;
	public RectTransform rightHeart;
	public RectTransform askFriendsButton;

	public Text textHeart;
	public Text textHeartShadow;
	public Text numberNegative;
	public Text numberNegativeShadow;

	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public Transform goalPopUpSlotsParent;
	public GridLayoutGroup FriendsgridLayoutGroup;

	protected bool pressed;

	public AnimationCurve heartAnimation;

	void Start()
	{
		//TODO checar login a facebook
		//fbLogin ();
		//FBLoggin.GetInstance().onLoginComplete += fbLogin;

		setStartingPlaces ();
		FriendsgridLayoutGroup.cellSize = new Vector2 (Screen.width * 0.225f, Screen.height * 0.15f);

		inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_FACEBOOK);
		content.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_TEXT);
		LevelText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVES_NAME_TEXT_ID);

		feedback.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FEEDBACK_TEXT);
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

		PersistentData.GetInstance().startLevel--;
		char[] lvl;

		switch (PersistentData.GetInstance().lastLevelPlayedName.Length) 
		{
		case 1:
			LevelNumberUnit.text = PersistentData.GetInstance ().lastLevelPlayedName;
			LevelNumberDecimal.gameObject.SetActive (false);
			LevelNumberHundred.gameObject.SetActive (false);
			break;
		case 2:
			lvl = PersistentData.GetInstance ().lastLevelPlayedName.ToCharArray();
			LevelNumberUnit.text = lvl[1].ToString();
			LevelNumberDecimal.text = lvl[0].ToString();
			LevelNumberHundred.gameObject.SetActive (false);
			break;
		case 3:
			lvl = PersistentData.GetInstance ().lastLevelPlayedName.ToCharArray();
			LevelNumberUnit.text = lvl[2].ToString();
			LevelNumberDecimal.text = lvl[1].ToString();
			LevelNumberHundred.text = lvl[0].ToString();
			break;                                                                  
		}

		if((DataManagerKubera.GetInstance () as DataManagerKubera).currentUser.playerLifes == 0) 
		{
			playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_RETRY_NOLIFES);
		}
		else
		{
			playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_NEXT);
		}

		int currentLifes = LifesManager.GetInstance ().currentUser.playerLifes + 1;
		textHeart.text = textHeartShadow.text = currentLifes.ToString();

		Invoke ("startAnimation", 0.25f);
	}

	public void close()
	{
		soundButton ();
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
		setStartingPlaces ();

		DOTween.Kill ("askFriendsButtonAnimation");
		OnComplete ("closeRetry");
	}

	public void retryLevel()
	{
		if(pressed)
		{
			return;
		}
		pressed = true;

		soundButton ();
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).currentUser.playerLifes > 0) 
		{
			setStartingPlaces ();
			DOTween.Kill ("askFriendsButtonAnimation");
			OnComplete ("retry",false);
		} 
		else 
		{
			pressed = false;
			DOTween.Kill ("askFriendsButtonAnimation");
			askForLifes ();
		}
	}

	protected void setStartingPlaces()
	{
		/*leftDoor.localScale = new Vector3 (1, 1.4f);
		rightDoor.localScale = new Vector3 (1, 1.4f);*/
		topLevel.anchoredPosition = new Vector3 (0, topLevel.rect.height*2, 0);
		leftDoor.anchoredPosition = new Vector3 (-leftDoor.rect.width, 0, 0);
		rightDoor.anchoredPosition = new Vector3 (rightDoor.rect.width, 0, 0);
		retry.localScale = Vector2.zero;
		ContentRT.localScale = Vector2.zero;
		closeButton.localScale = Vector2.zero;
		facebookFriends.localScale = Vector2.zero;
		facebookInvite.localScale = Vector2.zero;
		//starPanel.anchoredPosition = new Vector3 (0, starPanel.rect.height*2, 0);
		//facebookInvite.anchoredPosition = new Vector3 (0, -facebookInvite.rect.height*2, 0);
		this.transform.localScale = new Vector2(2,2);

		numberNegative.color = new Color (1, 1, 1, 0);
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
						closeButton.DOScale(new Vector2(1,1),tenth);
						icon.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
							{
								ContentRT.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
									{
										retry.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
											{
												facebookFriends.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
													{
														/*LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(PersistentData.GetInstance().lastLevelPlayedName,slotParent);
														leaderboard.showSlots(true);

														scrollRect.horizontalNormalizedPosition = 0;*/

														facebookInvite.DOScale(new Vector2(1,1),tenth);

														leftHeart.DORotate(new Vector3(0,0,5),0.2f).SetEase(heartAnimation);
														rightHeart.DORotate(new Vector3(0,0,-5),0.2f).SetEase(heartAnimation).OnComplete(()=>
															{
																numberNegative.DOColor(new Color(1,1,1,1),quarter).OnComplete(()=>
																	{
																		numberNegative.DOColor(new Color(1,1,1,0),1f);
																		numberNegativeShadow.DOColor(new Color(1,1,1,0),1f);
																		numberNegative.rectTransform.DOLocalMoveY(numberNegative.rectTransform.localPosition.y*3,1f);

																		textHeart.DOText(LifesManager.GetInstance().currentUser.playerLifes.ToString(),0.4f);
																		textHeartShadow.DOText(LifesManager.GetInstance().currentUser.playerLifes.ToString(),0.4f);
																		askFriendsButtonAnimation();
																	});
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

	protected void askFriendsButtonAnimation()
	{
		askFriendsButton.DOPunchRotation (new Vector3 (0, 0, 5), 0.5f).OnComplete (() => 
			{
				askFriendsButtonAnimation();
			}).SetId("askFriendsButtonAnimation");
	}

	public void askForLifes()
	{
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).currentUser.playerLifes == 0)
		{
			OnComplete ("askLifes",false);
		}
		else
		{
			OnComplete ("askLifes",false);
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