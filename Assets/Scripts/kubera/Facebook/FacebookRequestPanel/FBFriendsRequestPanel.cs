using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Kubera.Data;
using utils.gems.sync;

public class FBFriendsRequestPanel : PopUpBase {

	public enum ERequestType
	{
		ASK_LIFES,
		ASK_KEYS
	}

	public enum EFriendsType
	{
		GAME,
		ALL
	}

	public Text requestText;
	public Transform allFriendsObject;
	public Transform gameFriendsObject;
	public Text askButton;
	public FriendsController invitableFriends;
	public FriendsController gameFriends;
	public Toggle selectAll;
	public ERequestType currentRequestType;
	public EFriendsType currentFriendType;

	protected bool allFriendsImageExist;
	protected bool allGamesFriendsImageExist;

	public int maxFriendsToShow = 200;

	protected bool allFriendsSelected;
	protected bool friendsInitialized;

	public Text allFriendText;
	public Text kuberaFriendsText;
	public Color notSelected;

	public FacebookManager facebookManager;

	protected Vector2 initialPosRT;

	public ScrollRect allFriendsSR;
	public ScrollRect gameFriendsSR;

	void Start()
	{
		changeBetweenFriends (true);
		invitableFriends.OnActivated = activateAllSelected;
		gameFriends.OnActivated = activateAllSelected;
		askButton.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FB_REQUEST_ASK_TEXT);
	}

	public override void activate()
	{
		popUp.SetActive (true);

	}

	public void closePressed()
	{
		popUp.SetActive (false);

		OnPopUpCompleted (this);
	}

	public void openFriendsRequestPanel(ERequestType requestType,EFriendsType friendsType = EFriendsType.ALL)
	{
		checkImageExist (friendsType);

		allFriendsSR.normalizedPosition = Vector2.zero;
		gameFriendsSR.normalizedPosition = Vector2.zero;

		switch (requestType) {

		case ERequestType.ASK_KEYS:
			requestText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.BOSS_LOCKED_KEY_TEXT);
			currentRequestType = requestType;
			break;
		case ERequestType.ASK_LIFES:
			requestText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FULL_LIFES_POPUP_BUTTON);
			currentRequestType = requestType;
			break;
		}
	}

	public void changeBetweenFriends(bool invitableFriends)
	{
		if(invitableFriends)
		{
			this.invitableFriends.gameObject.SetActive (true);
			gameFriends.gameObject.SetActive (false);
			allFriendsSelected = true;
			activateAllSelected (true);
			currentFriendType = EFriendsType.ALL;

			allFriendText.color = Color.white;
			kuberaFriendsText.color = notSelected;
			gameFriendsObject.SetAsFirstSibling ();
		}
		else
		{
			this.invitableFriends.gameObject.SetActive (false);
			gameFriends.gameObject.SetActive (true);
			allFriendsSelected = false;
			activateAllSelected (true);
			currentFriendType = EFriendsType.GAME;

			kuberaFriendsText.color = Color.white;
			allFriendText.color = notSelected;
			allFriendsObject.SetAsFirstSibling ();
		}

		checkImageExist (currentFriendType);
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
			initializeFriendsController (gameFriends, friendType);
			currentRequestType = requestType;
			break;
		case ERequestType.ASK_LIFES:
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
		case EFriendsType.ALL:
			initializeFriendsController (gameFriends, this.invitableFriends);
			break;
		default:
			break;
		}
	}

	public void initializeFriendsController(List<object> friends,FriendsController friendController)
	{
		for(int i=0; i<friends.Count && i<maxFriendsToShow; i++)
		{
			Dictionary<string,object> friendInfo = ((Dictionary<string,object>)(friends [i]));
			string playerID = (string)friendInfo ["id"];
			//print (friendInfo.Keys.ToCommaSeparateList ());
			string playerName = (string)friendInfo ["name"];
			Texture playerImage = new Texture();

			if(FacebookPersistentData.GetInstance().containTextureByID(playerID))
			{
				playerImage = FacebookPersistentData.GetInstance().getTextureById (playerID);
			}

			friendController.addFriend (playerID, playerName,playerImage);
		}
		friendController.initializeFriends ();
	}

	public void sendRequest()
	{
		List<string> friendsIds = new List<string> ();
		List<List<string>> friIDs = new  List<List<string>>();
		friendsIds = getFriendsActivatedIDByFriendsType (currentFriendType);
		int friendGroups = (int)Mathf.Floor(friendsIds.Count / 30.0f);
	
		/*

		string myId = FacebookPersistentData.GetInstance ().getPlayerId ();

		if(currentFriendType == EFriendsType.GAME)
		{
			for(int i=0; i<friendsIds.Count; i++)
			{
				ShopikaSyncManager.GetCastedInstance<ShopikaSyncManager>().registerInvite(friendsIds[i],myId);
			}
		}*/

		switch (currentRequestType) {
		case ERequestType.ASK_KEYS:

			KuberaAnalytics.GetInstance ().registerFacebookKeyRequest (PersistentData.GetInstance().lastLevelReachedName);

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

			if (!((DataManagerKubera)DataManagerKubera.GetInstance ()).alreadyAskForLifes()) 
			{
				KuberaAnalytics.GetInstance ().registerFacebookFirstLifeRequest (PersistentData.GetInstance().lastLevelReachedName);
				((DataManagerKubera)DataManagerKubera.GetInstance ()).markLifesAsAsked ();
			}

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
		case EFriendsType.ALL:
			return invitableFriends.getFriendsActivatedID ();
		}
		return null;
	}

	protected void checkImageExist(EFriendsType friendsType)
	{
		switch (friendsType) {
		case EFriendsType.ALL:
			if(!allFriendsImageExist)
			{
				allFriendsImageExist = true;
				for(int i=0; i<invitableFriends.friends.Count; i++)
				{
					if(!invitableFriends.friends[i].imageSetted)
					{
						allFriendsImageExist = false;
						Sprite image = FacebookPersistentData.GetInstance ().getSpritePictureById (invitableFriends.friends [i].id);
						if(image == null)
						{
							invitableFriends.friends [i].getTextureFromURL (GraphUtil.DeserializePictureURL(FacebookPersistentData.GetInstance ().getFriendInfo (invitableFriends.friends [i].id)));
						}
						else
						{
							invitableFriends.friends [i].setFriendImage (image);
							invitableFriends.friends [i].imageSetted = true;
						}
					}
				}
			}
			break;
		case EFriendsType.GAME:
			if(!allGamesFriendsImageExist)
			{
				allGamesFriendsImageExist = true;
				for(int i=0; i<gameFriends.friends.Count; i++)
				{
					if(!gameFriends.friends[i].imageSetted)
					{
						allGamesFriendsImageExist = false;
						Sprite image = FacebookPersistentData.GetInstance ().getSpritePictureById (gameFriends.friends [i].id);
						if(image == null)
						{
							gameFriends.friends [i].getTextureFromURL (GraphUtil.DeserializePictureURL(FacebookPersistentData.GetInstance ().getFriendInfo (gameFriends.friends [i].id)));
						}
						else
						{
							gameFriends.friends [i].setFriendImage (image);
							gameFriends.friends [i].imageSetted = true;
						}
					}
				}
			}
			break;
		default:
			break;
		}
	}
}
