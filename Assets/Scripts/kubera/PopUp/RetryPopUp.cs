using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RetryPopUp : PopUpBase
{
	public Text LevelNumber;

	protected LifesManager lifesManager;

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

	void Start()
	{
		lifesManager = FindObjectOfType<LifesManager> ();

		setStartingPlaces ();
		content.text = "Seguro lo lograras";
	}

	public override void activate()
	{
		popUp.SetActive (true);

		PersistentData.GetInstance().startLevel--;
		LevelNumber.text = PersistentData.GetInstance().lastLevelPlayedName;

		startAnimation ();
	}

	public void close()
	{
		leaderboardManager.moveCurrentLeaderboardSlots (goalPopUpSlotsParent);
		setStartingPlaces ();
		OnComplete ("closeRetry");
	}

	public void retryLevel()
	{
		if (UserDataManager.instance.playerLifes > 0) 
		{
			setStartingPlaces ();
			OnComplete ("retry");
		} 
		else 
		{
			setStartingPlaces ();
			OnComplete ("NoLifesPopUp");
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

}