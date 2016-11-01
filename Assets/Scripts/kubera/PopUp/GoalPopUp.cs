﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;
using Kubera.Data.Sync;

public class GoalPopUp : PopUpBase {

	public Text goalTextABA;
	public Text goalTextABB;
	public Text goalTextA;
	public Text goalTextLetters;

	public Text LevelNumber;
	public Text LevelNumberShadow;
	public Text LevelText;
	public Text LevelTextShadow;

	public Text inviteFriendsText;
	public Text playText;

	public Text feedback;

	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject ABObjective;
	public GameObject AObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

	public GameObject[] stars;
	public GameObject[] starsGray;

	protected int objective;

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

	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public GridLayoutGroup FriendsgridLayoutGroup;

	public Sprite[] worldTopBackground;
	public Sprite[] worldIcon;
	public Image topLevelImage;
	public Image topIcon;
	public Image topIconShadow;
	protected bool pressed;

	void Start()
	{
		FriendsgridLayoutGroup.cellSize = new Vector2 (Screen.width * 0.225f, Screen.height * 0.15f);

		//TODO checar login a facebook
		fbLogin ();
		//FBLoggin.GetInstance().onLoginComplete += fbLogin;

		setStartingPlaces ();

		int maxSize = 5;
		if(((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-gridLayoutGroup.padding.left) < goalLettersContainer.GetComponent<RectTransform> ().rect.height *.5f)
		{
			gridLayoutGroup.cellSize = new Vector2((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-5
				,(goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-5);
		}
		else
		{
			gridLayoutGroup.cellSize = new Vector2(goalLettersContainer.GetComponent<RectTransform> ().rect.height *.5f,goalLettersContainer.GetComponent<RectTransform> ().rect.height *.5f);

		}

		inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVE_POPUP_FACEBOOK);
		playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVE_POPUP_PLAY);
		LevelText.text = LevelTextShadow.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVES_NAME_TEXT_ID);

		feedback.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FEEDBACK_TEXT);
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.V))
		{
			startAnimation ();
		}
	}

	protected void fbLogin()
	{
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>() && KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			//TODO HARCODING
			inviteFriendsText.text = "invita Amigos";
		}
		else
		{
			inviteFriendsText.text = "Conectate";
		}
	}

	public override void activate()
	{
		popUp.SetActive (true);
		startAnimation ();
	}

	public void setGoalPopUpInfo(string textA,string textB,int starsReached, List<string> letters = null, string levelName = "" , int aABLetterObjectives = 0,int currentWorld =0)
	{
		this.LevelNumber.text = LevelNumberShadow.text = levelName;
		ABObjective.SetActive (false);
		AObjective.SetActive (false);
		lettersObjective.SetActive (false);
		resetStars ();

		topLevelImage.sprite = worldTopBackground [currentWorld-1];
		topIcon.sprite = topIconShadow.sprite = worldIcon [currentWorld-1];

		objective = aABLetterObjectives;
		switch (objective) {
		case 0:
			goalTextABA.text = textA;
			goalTextABB.text = textB;
			ABObjective.SetActive (true);
			break;
		case 1:
			goalTextA.text = textA;
			AObjective.SetActive (true);
			break;
		case 2:
			goalTextLetters.text = textA;
			lettersObjective.SetActive (true);

			break;
		default:
			break;
		}
		if(letters.Count != 0)
		{
			destroyLetersOnContainer ();
			//goalText.enabled = false;
			lettersObjective.SetActive (true);
			//goalLettersText.text = text;
			foreach (object val in letters) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = val.ToString();
				letter.GetComponentInChildren<Text> ().text = val.ToString();
				letter.transform.SetParent (goalLettersContainer.transform,false);
			}
		}

		showStars (starsReached);
	}

	protected void destroyLetersOnContainer()
	{
		for(int i=0; i<goalLettersContainer.transform.childCount;)
		{
			DestroyImmediate (goalLettersContainer.transform.GetChild (i).gameObject);
		}
	}

	protected void showStars(int starsReached)
	{		
		for(int i=0; i<starsReached; i++)
		{
			stars [i].SetActive (true);
			starsGray [i].SetActive (false);
		}
	}

	protected void resetStars()
	{		
		for(int i=0; i<stars.Length; i++)
		{
			stars [i].SetActive (false);
			starsGray [i].SetActive (true);
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
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).currentUser.playerLifes > 0) 
		{
			OnComplete ("playGame",false);
		} 
		else 
		{
			pressed = false;
			OnComplete ("NoLifes",false);
		}
	}

	public void exit ()
	{
		soundButton ();
		setStartingPlaces ();
		OnComplete ("closeRetry");
	}

	protected void setStartingPlaces()
	{
		resetStars ();

		/*leftDoor.localScale = new Vector3 (1, 1.4f);
		rightDoor.localScale = new Vector3 (1, 1.4f);*/
		topLevel.anchoredPosition = new Vector3 (0, topLevel.rect.height*2, 0);
		leftDoor.anchoredPosition = new Vector3 (-leftDoor.rect.width, 0, 0);
		rightDoor.anchoredPosition = new Vector3 (rightDoor.rect.width, 0, 0);
		starPanel.localScale = Vector2.zero;
		objetives.localScale = Vector2.zero;
		play.localScale = Vector2.zero;
		close.localScale = Vector2.zero;
		facebookFriends.localScale = Vector2.zero;
		facebookInvite.localScale = Vector2.zero;
		//starPanel.anchoredPosition = new Vector3 (0, starPanel.rect.height*2, 0);
		//facebookInvite.anchoredPosition = new Vector3 (0, -facebookInvite.rect.height*2, 0);
		this.transform.localScale = new Vector2(2,2);

		leaderboardManager.showCurrentLeaderboard (false);
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
								objetives.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
									{
										play.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
											{
												facebookFriends.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
													{
														if(leaderboardManager != null)
														{
															LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(this.LevelNumber.text,slotParent);
															leaderboard.showSlots(true);
														}

														scrollRect.horizontalNormalizedPosition = 0;

														facebookInvite.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
															{
																icon.DOScale(new Vector2(1,1),tenth);
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

	public void fbAction()
	{
		soundButton ();
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			//TODO HARCODING
			inviteFriendsText.text = "invita Amigos";
			FacebookManager.GetInstance ().requestNewFriends ();
		}
		else
		{
			inviteFriendsText.text = "Conectate";
			KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookLogin();
		}
	}
}
