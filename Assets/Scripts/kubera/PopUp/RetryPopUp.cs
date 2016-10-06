using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Kubera.Data;
using Kubera.Data.Sync;

public class RetryPopUp : PopUpBase
{
	public Text LevelNumber;
	public Text LevelNumberShadow;
	public Text LevelText;
	public Text LevelTextShadow;

	public Text inviteFriendsText;
	public Text playText;

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


	public LeaderboardManager leaderboardManager;
	public Transform slotParent;
	public ScrollRect scrollRect;

	public Transform goalPopUpSlotsParent;
	public GridLayoutGroup FriendsgridLayoutGroup;

	public Sprite[] worldTopBackground;
	public Sprite[] worldIcon;
	public Image topLevelImage;
	public Image topIcon;
	public Image topIconShadow;
	protected bool pressed;

	void Start()
	{
		//TODO checar login a facebook
		//fbLogin ();
		//FBLoggin.GetInstance().onLoginComplete += fbLogin;

		setStartingPlaces ();
		FriendsgridLayoutGroup.cellSize = new Vector2 (Screen.width * 0.225f, Screen.height * 0.15f);

		inviteFriendsText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_FACEBOOK);
		playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_NEXT);
		content.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.LOOSEGAME_POPUP_TEXT);
		LevelText.text = LevelTextShadow.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.OBJECTIVES_NAME_TEXT_ID);
	}

	protected void fbLogin()
	{
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
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

		PersistentData.GetInstance().startLevel--;
		LevelNumber.text = LevelNumberShadow.text = PersistentData.GetInstance().lastLevelPlayedName;

		topLevelImage.sprite = worldTopBackground [PersistentData.GetInstance().currentWorld-1];
		topIcon.sprite = topIconShadow.sprite = worldIcon [PersistentData.GetInstance().currentWorld-1];

		startAnimation ();
	}

	public void close()
	{
		soundButton ();
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
		setStartingPlaces ();
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
			OnComplete ("retry",false);
		} 
		else 
		{
			pressed = false;
			OnComplete ("NoLifes",false);
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
														LevelLeaderboard leaderboard = leaderboardManager.getLeaderboard(this.LevelNumber.text,slotParent);
														leaderboard.showSlots(true);

														scrollRect.horizontalNormalizedPosition = 0;

														facebookInvite.DOScale(new Vector2(1,1),tenth);
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