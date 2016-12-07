using DG.Tweening;
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
	protected string levelName;

	public Text LevelNumberUnit;
	public Text LevelNumberDecimal;
	public Text LevelNumberHundred;
	public Text LevelText;

	public Text inviteFriendsText;
	public Text playText;
	public Text playTextWithPowers;

	public Text feedback;

	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject ABObjective;
	public GameObject AObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

	public Image starsBarr;
	public GameObject[] stars;
	public GameObject[] balls;

	protected int objective;

	public RectTransform leftDoor;
	public RectTransform rightDoor;
	public RectTransform topLevel;
	public RectTransform objetives;
	public RectTransform play;
	public RectTransform playWithPowers;
	public RectTransform close;
	public RectTransform facebookFriends;
	public RectTransform facebookInvite;
	public RectTransform icon;

	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public GridLayoutGroup FriendsgridLayoutGroup;

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
		playTextWithPowers.text = playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVE_POPUP_PLAY);
		LevelText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVES_NAME_TEXT_ID);

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
		startAnimation ();
	}

	public void setGoalPopUpInfo(string textA,string textB,int starsReached, List<string> letters = null, string levelName = "" , int aABLetterObjectives = 0,int currentWorld =0)
	{
		ABObjective.SetActive (false);
		AObjective.SetActive (false);
		lettersObjective.SetActive (false);
		this.levelName = levelName;
		char[] lvl;

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

		resetStars ();
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
		print ("resetStars");
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
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).currentUser.playerLifes > 0) 
		{
			OnComplete ("playGame",false);
		} 
		else 
		{
			pressed = false;
			OnPopUpCompleted (this,"NoLifes",false);
			//OnComplete ("NoLifes",false);
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

		/*leftDoor.localScale = new Vector3 (1, 1.4f);
		rightDoor.localScale = new Vector3 (1, 1.4f);*/
		topLevel.anchoredPosition = new Vector3 (0, topLevel.rect.height*2, 0);
		leftDoor.anchoredPosition = new Vector3 (-leftDoor.rect.width, 0, 0);
		rightDoor.anchoredPosition = new Vector3 (rightDoor.rect.width, 0, 0);
		objetives.localScale = Vector2.zero;
		playWithPowers.localScale = play.localScale = Vector2.zero;
		close.localScale = Vector2.zero;
		facebookFriends.localScale = Vector2.zero;
		facebookInvite.localScale = Vector2.zero;
		icon.localScale = Vector2.zero;
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

						objetives.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
							{
								icon.DOScale(new Vector2(1,1),tenth);
								close.DOScale(new Vector2(1,1),tenth);

								playWithPowers.DOScale(new Vector2(1,1),tenth);
								play.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
									{
										facebookFriends.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
											{
												if(leaderboardManager != null)
												{
													LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(this.levelName,slotParent);
													leaderboard.showSlots(true);
												}

												scrollRect.horizontalNormalizedPosition = 0;

												facebookInvite.DOScale(new Vector2(1,1),tenth).OnComplete(()=>
													{
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
