using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalAfterGame : PopUpBase {

	public Text LevelName;

	public Text PointsText;
	public Text Points;
	public Text inviteFriendsText;

	public GameObject[] stars;
	public GameObject[] starsGray;

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

	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public Transform goalPopUpSlotsParent;

	void Start()
	{
		//TODO checar login a facebook
		fbLogin ();
		//FBLoggin.GetInstance().onLoginComplete += fbLogin;

		setStartingPlaces ();
	}

	protected void fbLogin()
	{
		if(FBLoggin.GetInstance().isLoggedIn)
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

	public void setGoalPopUpInfo(int starsReached, string levelName, string points)
	{
		this.LevelName.text = levelName;
		this.Points.text = points;

		showStars (starsReached);
	}

	protected void showStars(int starsReached)
	{
		for(int i=0; i<stars.Length; i++)
		{
			stars [i].SetActive (false);
			stars [i].transform.localScale = new Vector3 (0, 0, 0);
		}
		showStarsByAnimation (starsReached);
	}

	protected void showStarsByAnimation(int starsReached)
	{
		if(starsReached >=1)
		{
			winStar ();
			stars [0].SetActive (true);
			stars [0].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
				if (starsReached >= 2) {
					winStar ();
					stars [1].SetActive (true);
					stars [1].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
						if (starsReached == 3) {
							winStar ();
							stars [2].SetActive (true);
							stars [2].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
								//print ("Termino de mostrar estrellas");
							});
						}
					});
				}
			});
		}
	}

	public void playGame()
	{
		soundButton ();

		setStartingPlaces ();
		OnComplete ("continue");
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
	}

	public void exit ()
	{
		soundButton ();

		setStartingPlaces ();
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
		OnComplete ("closeObjective");
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
		close.localScale = Vector2.zero;
		facebookFriends.localScale = Vector2.zero;
		facebookInvite.localScale = Vector2.zero;
		//starPanel.anchoredPosition = new Vector3 (0, starPanel.rect.height*2, 0);
		//facebookInvite.anchoredPosition = new Vector3 (0, -facebookInvite.rect.height*2, 0);
		this.transform.localScale = new Vector2(2,2);
	}

	protected void startAnimation()
	{
		float fullTime = 1;
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
														LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(this.LevelName.text,slotParent);
														leaderboard.showSlots(true);

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

	protected void winStar()
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("star");
			AudioManager.GetInstance().Play("star");
		}
	}

	public void fbAction()
	{
		soundButton ();
		if(FBLoggin.GetInstance().isLoggedIn)
		{
			//TODO HARCODING
			inviteFriendsText.text = "invita Amigos";
			FacebookManager.GetInstance ().requestNewFriends ();
		}
		else
		{
			inviteFriendsText.text = "Conectate";
			FBLoggin.GetInstance ().OnLoginClick ();
		}
	}
}
