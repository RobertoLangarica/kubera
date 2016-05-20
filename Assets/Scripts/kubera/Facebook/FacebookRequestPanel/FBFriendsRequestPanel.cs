﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FBFriendsRequestPanel : PopUpBase {

	public enum ERequestType
	{
		ASK_LIFES,
		ASK_KEYS
	}

	public enum EFriendsType
	{
		GAME,
		INVITABLE
	}

	public Text requestText;
	public Button allFriendsButton;
	public Button gameFriendsButton;
	public FriendsController invitableFriends;
	public FriendsController gameFriends;
	public Toggle selectAll;
	public ERequestType currentRequestType;
	public EFriendsType currentFriendType;

	protected bool allFriendsSelected;
	protected bool friendsInitialized;
	protected FacebookManager facebookManager;

	void Start()
	{
		facebookManager = FindObjectOfType<FacebookManager>();

		changeBetweenFriends (true);
		invitableFriends.OnActivated = activateAllSelected;
		gameFriends.OnActivated = activateAllSelected;

	}

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void closePressed()
	{
		popUp.SetActive (false);

		OnPopUpCompleted ();
	}

	public void openFriendsRequestPanel(ERequestType requestType,EFriendsType friendsType = EFriendsType.INVITABLE)
	{
		currentRequestType = requestType;
		currentFriendType = friendsType;
	}

	public void changeBetweenFriends(bool invitableFriends)
	{
		if(invitableFriends)
		{
			this.invitableFriends.gameObject.SetActive (true);
			gameFriends.gameObject.SetActive (false);
			allFriendsSelected = true;
			activateAllSelected (true);
			currentFriendType = EFriendsType.INVITABLE;
		}
		else
		{
			this.invitableFriends.gameObject.SetActive (false);
			gameFriends.gameObject.SetActive (true);
			allFriendsSelected = false;
			activateAllSelected (true);
			currentFriendType = EFriendsType.GAME;
		}
	}

	public void selectAllFriends()
	{
		if(allFriendsSelected)
		{
			invitableFriends.selectAllFriends (selectAll.isOn);
		}
		else
		{
			gameFriends.selectAllFriends (selectAll.isOn);
		}
	}

	protected FriendsController getFriendsActive()
	{
		if(allFriendsSelected)
		{			
			return invitableFriends;
		}

		return gameFriends;
	}

	public void activateAllSelected(bool activated)
	{
		if(activated)
		{
			if(isAllFriendsSelected(getFriendsActive()))
			{
				selectAll.isOn = true;
			}
			else
			{
				selectAll.isOn = false;
			}
		}
		else
		{
			selectAll.isOn = false;
		}
	}

	public bool isAllFriendsSelected(FriendsController friends)
	{
		return friends.isAllFriendsSelected ();
	}

	public void initializeFriendsController(ERequestType requestType,List<object> gameFriends,EFriendsType friendType)
	{
		switch (requestType) {

		case ERequestType.ASK_KEYS:
			requestText.text = "pide llave";
			initializeFriendsController (gameFriends, friendType);
			currentRequestType = requestType;
			break;
		case ERequestType.ASK_LIFES:
			requestText.text = "pide vidas";
			initializeFriendsController (gameFriends,friendType);
			currentRequestType = requestType;
			break;
		}
	}



	public void initializeFriendsController(List<object> gameFriends, EFriendsType friendType)
	{
		switch (friendType) {
		case EFriendsType.GAME:
			initializeFriendsController (gameFriends, this.gameFriends);
			break;
		case EFriendsType.INVITABLE:
			initializeFriendsController (gameFriends, this.invitableFriends);
			break;
		default:
			break;
		}
	}

	public void initializeFriendsController(List<object> friends,FriendsController friendController)
	{
		for(int i=0; i<friends.Count; i++)
		{
			Dictionary<string,object> friendInfo = ((Dictionary<string,object>)(friends [i]));
			string playerID = (string)friendInfo ["id"];
			//print (friendInfo.Keys.ToCommaSeparateList ());
			string playerName = (string)friendInfo ["name"];
			string playerImgUrl = GraphUtil.DeserializePictureURL(friendInfo);

			friendController.addFriend (playerID,playerImgUrl, playerName);
		}
		friendController.initializeFriends ();
	}

	public void sendRequest()
	{
		List<string> friendsIds = new List<string> ();
		List<List<string>> friIDs = new  List<List<string>>();
		friendsIds = getFriendsActivatedIDByFriendsType (currentFriendType);
		int friendGroups = (int)Mathf.Floor(friendsIds.Count / 30.0f);

		switch (currentRequestType) {
		case ERequestType.ASK_KEYS:

			if(friendsIds.Count>30)
			{
				for(int i=0,k = 0; i <= friendGroups; i++)
				{
					friIDs.Add (new List<string>());
					for (int j = 0; j < 30; j++,k++) 
					{
						if (friendsIds [k] != null) 
						{
							friIDs [i].Add (friendsIds [k]);
						}
					}
				}

				for(int i = 0;i < friIDs.Count;i++)
				{
					facebookManager.askKey (friIDs[i]);
				}
			}
			else
			{
				facebookManager.askKey (friendsIds);
				return;
			}

			break;
		case ERequestType.ASK_LIFES:

			if(friendsIds.Count>30)
			{
				for(int i=0,k = 0; i <= friendGroups; i++)
				{
					friIDs.Add (new List<string>());
					for (int j = 0; j < 30; j++,k++) 
					{
						if (k < friendsIds.Count) 
						{
							friIDs [i].Add (friendsIds [k]);
						}
					}
				}

				for(int i = 0;i < friIDs.Count;i++)
				{
					facebookManager.askLife (friIDs[i]);
				}
			}
			else
			{
				facebookManager.askLife (friendsIds);
				return;
			}

			break;
		default:
			break;
		}
	}

	protected List<string> getFriendsActivatedIDByFriendsType(EFriendsType friendType)
	{
		switch (friendType) 
		{
		case EFriendsType.GAME:
			return gameFriends.getFriendsActivatedID ();
		case EFriendsType.INVITABLE:
			return invitableFriends.getFriendsActivatedID ();
		}
		return null;
	}
}
