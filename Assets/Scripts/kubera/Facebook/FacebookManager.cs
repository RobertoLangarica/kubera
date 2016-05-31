﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Facebook.Unity;
using UnityEngine.UI;
using Facebook.MiniJSON;

public class FacebookManager : MonoBehaviour
{
	protected FBGraph fbGraph;
	protected FacebookNews facebookNews;
	protected PlayerInfo playerInfo;
	protected FBLog fbLog;
	protected MapManager mapManager;
	public FBFriendsRequestPanel fbRequestPanel;

	public Transform panelMessages;
	public GameObject friendRequest;
	public GameObject FacebookConectMessage;

	protected bool facebookConectMessageCreated;

	///Vidas que pedi
	protected List<string> askedLifes = new List<string>();
	///vidas que me dieron
	protected List<string> giftLifes = new List<string>();
	///laves que pedi
	protected List<string> askedKeys = new List<string>();
	///llaves que me dieron
	protected List<string> giftKeys = new List<string>();


	public Dictionary<string, Texture> friendsImage = new Dictionary<string, Texture>();

	public int maxUsersPerMessage = 5;
	protected int messageCount;

	protected GameObject conectFacebook;

	void Awake()
	{
		fbGraph = FindObjectOfType<FBGraph> ();
		fbLog = FindObjectOfType<FBLog> ();
		facebookNews = FindObjectOfType<FacebookNews> ();
		playerInfo = FindObjectOfType<PlayerInfo> ();
		mapManager = FindObjectOfType<MapManager> ();
	}

	void Start()
	{
		fbLog.onLoginComplete += OnLoginComplete;

		fbGraph.OnPlayerInfo += showPlayerInfo;
		fbGraph.OnGetGameFriends += addGameFriends;
		fbGraph.OnGetInvitableFriends += addInivitableFriends;
		fbGraph.OnGetFriendTextures += addFriendsImage;
		fbGraph.onFinishGettingFriends += mergeFriends;

		fbGraph.OnGetAppRequest += chanelData;

		fbGraph.onFinishGettingInfo += fillMessageData;

		OnLoginComplete (fbLog.isLoggedIn);
	}

	protected void OnLoginComplete(bool complete)
	{
		Debug.Log("OnLoginComplete " + complete);

		if (complete)
		{
			if(canRequestMoreFriends())
			{
				fbGraph.GetPlayerInfo();
				fbGraph.GetFriends();
				fbGraph.GetInvitableFriends();
			}

			fbGraph.getFriendsAppRequests ();
			if(conectFacebook != null)
			{
				DestroyImmediate (conectFacebook);
			}
		}
		else
		{
			if(!facebookConectMessageCreated)
			{
				//crear FacebookConectMessage
				print ("creando mensaje para conectar");
				conectFacebook = Instantiate (FacebookConectMessage);
				conectFacebook.transform.SetParent (panelMessages,false);
				facebookConectMessageCreated = true;
			}
		}
	}

	public bool canRequestMoreFriends()
	{
		print (FacebookPersistentData.instance.infoRequested);
		if(FacebookPersistentData.instance.infoRequested)
		{
			return false;
		}
		return true;
	}

	protected void showPlayerInfo(string id, string name)
	{
		
	}

	protected void addGameFriends(List<object> gameFriends)
	{
		FacebookPersistentData.instance.addGameFriend (gameFriends);

		fillRequestPanel (gameFriends, FBFriendsRequestPanel.EFriendsType.GAME);
	}

	protected void addInivitableFriends(List<object> invitableFriends)
	{
		FacebookPersistentData.instance.addInvitableFriend (invitableFriends);
	}

	protected void mergeFriends()
	{
		FacebookPersistentData.instance.mergeFriends ();
		fillRequestPanel (FacebookPersistentData.instance.allFriends, FBFriendsRequestPanel.EFriendsType.ALL);
	}

	protected void addFriendsImage(string id, Texture image)
	{
		if(FacebookPersistentData.instance.containTextureByID(id))
		{			
			FacebookPersistentData.instance.addFriendImage (id, image);
		}
	}

	public void acceptGift(bool life, int giftCount,GameObject requestToDelete, string bossReached = "0")
	{
		//TODO: 
		if(life)
		{
			print("recibi " + giftCount + ": vidas");	
		}
		else
		{
			print("recibi " + giftCount + ": llaves");
			mapManager.unlockBoss ();
		}
		DestroyImmediate (requestToDelete);
	}

	public void sendGift (bool life, List<string> friendsIDs,GameObject requestToDelete, string bossReached = "0")
	{
		if(life)
		{
			sendLife (friendsIDs);
		}
		else
		{
			sendKey (friendsIDs,bossReached);
		}
		DestroyImmediate (requestToDelete);
	}

	protected bool canPublish()
	{
		if(FBPermissions.HavePublishActions)
		{
			return true;
		}
		else
		{
			FBPermissions.PromptForPublish ();
			print ("can'tPublish");
			return false;
		}
	}

	public void sendLife(List<string> friendsIDs)
	{
		if (!canPublish())
		{
			return;
		}

		FB.AppRequest ("Here, take this life!", // A message for the user
			OGActionType.SEND, // Can be .Send or .AskFor depending on what you want to do with the object.
			"101162080284933", // Here we put the object id we got as a result before.		             
			friendsIDs,// The id of the sender.
			//null,20,
			"SendLife", // Here you can put in any data you want
			"Send a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void sendKey(List<string> friendsIDs,string bossReached)
	{
		if (!canPublish())
		{
			return;
		}

		FB.AppRequest ("Here, take this key!", // A message for the user
			OGActionType.SEND, // Can be .Send or .AskFor depending on what you want to do with the object.
			"795229890609809", // Here we put the object id we got as a result before.		             
			friendsIDs,// The id of the sender.
			//null,20,
			"SendKey,"+bossReached, // Here you can put in any data you want
			"Send a key to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askKey(List<string> idsFriends)
	{
		if(idsFriends.Count<50)
		{
			return;
		}
		FB.AppRequest ("Give me a key!", OGActionType.ASKFOR, "795229890609809", idsFriends, "askKey,"+(Data.LevelsDataManager.GetInstance ().getCurrentData ().bossReached +1).ToString(), // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log (result.RawResult);
			}
		);
	}

	public void askLife(List<string> idsFriends)
	{
		FB.AppRequest ("Give me a life!", OGActionType.ASKFOR, "101162080284933", idsFriends, "askLife", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log (result.RawResult);
			}
		);
	}
		
	public void requestNewFriends()
	{
		//a todos tus amigos
		FB.AppRequest(
			"Invita a tus amigos a jugar",
			null,null, null, null, null, "Invita a tus amigos a jugar",
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}
		

	protected void chanelData(List<object> data)
	{
		for(int i=0; i<data.Count; i++)
		{
			Dictionary<string,object> dataDict = ((Dictionary<string,object>)(data [i]));
			Dictionary<string,object> from = ((Dictionary<string,object>)(dataDict["from"]));

			string playerID = (string)from ["id"];
			string firstName = ((string)from ["name"]).Split (' ') [0];
			string dataType = (string)dataDict ["data"];
			string requestID = (string)dataDict ["id"];

			saveDataOnList (dataType, firstName, playerID, requestID);
			gotMessage ();
		}
	}

	protected void saveDataOnList(string type,string firstName, string playerID, string requestID)
	{
		switch (type) {
		case "askLife":
			if(idExistOnList (askedLifes,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				addToList (askedLifes, firstName, playerID, requestID);
			}
			break;
		case "sendLife":			
			if(idExistOnList (giftLifes,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				addToList (giftLifes, firstName, playerID, requestID);
			}
			break;
		default:
			print (type);
			//llaves que me pidieron
			if (type.Contains("askKey"))
			{
				string[] splitType = type.Split (',');
				int bossReached = 0;
				if(splitType.Length >1)
				{
					bossReached = int.Parse (splitType [1]);
				}

				if(idExistOnList (askedKeys,playerID) )
				{
					deleteAppRequest (requestID);
				}
				else
				{
					addToList (askedKeys, firstName,playerID, requestID, bossReached);
				}
			}
			//llaves que pedí
			else if(type.Contains("sendKey"))
			{
				string[] splitType = type.Split (',');
				int bossReached = 0;
				if(splitType.Length >1)
				{
					bossReached = int.Parse (splitType [1]);
				}

				if(askedKeys.Count == maxUsersPerMessage || idExistOnList (giftKeys,playerID )|| bossReached <= Data.LevelsDataManager.GetInstance ().getCurrentData ().bossReached)
				{
					deleteAppRequest (requestID);
				}
				else
				{
					addToList (giftKeys, firstName, playerID, requestID, bossReached);
				}
			}
				
			//if(type)

			break;
		}
	}

	protected bool idExistOnList(List<string> list,string id)
	{
		string playerID;
		for(int i=0; i<list.Count; i++)
		{
			playerID = list [i].Split('-')[1];
			if(playerID == id)
			{
				return true;
			}
		}
		return false;
	}

	protected void addToList (List<string> List,string firstName, string playerID, string requestID,int bossReached=-1)
	{
		if(bossReached == -1)
		{			
			List.Add (firstName+"-"+playerID+"-"+requestID);
		}
		else
		{
			List.Add (firstName+"-"+playerID+"-"+requestID+"-"+bossReached);
		}
	}

	public void deleteAppRequest(string id)
	{
		FB.API (id, HttpMethod.DELETE, deleteCallBackRequest);
	}

	protected void deleteCallBackRequest(IGraphResult result)
	{
		if (result.Error != null) 
		{
			Debug.LogError (result.Error);
			return;
		}
		else
		{
			//print ("deleteCallBackRequest:  " + result.RawResult);
		}
	}

	protected void gotMessage()
	{
		messageCount++;
	}

	protected void fillMessageData ()
	{
		sortData (askedKeys);
		fillData (askedKeys, PanelAppRequest.ERequestState.KEY, PanelAppRequest.EAction.SEND,true);
		fillData (giftKeys, PanelAppRequest.ERequestState.KEY, PanelAppRequest.EAction.ACCEPT);
		fillData (askedLifes, PanelAppRequest.ERequestState.LIFE, PanelAppRequest.EAction.SEND);
		fillData (giftLifes, PanelAppRequest.ERequestState.LIFE, PanelAppRequest.EAction.ACCEPT);

		actualizeMessageNumber ();
	}

	protected void sortData(List<string> requested)
	{
		string requestedSorted = "";
		for(int i=0; i<requested.Count; i++)
		{
			for(int j=0; j< requested.Count -1; j++)
			{
				if(int.Parse (requested[j].Split(',')[3]) > int.Parse(requested[j+1].Split(',')[3]))
				{
					requestedSorted = requested [j+1];
					requested [j + 1] = requested [j];
					requested [j] = requestedSorted;
				}
			}
		}
	}
		
	protected void fillData(List<string> requested, PanelAppRequest.ERequestState requestState, PanelAppRequest.EAction action,bool askedKeys = false)
	{
		GameObject go;
		PanelAppRequest pR = null;

		for(int i=0, j=0; i<requested.Count; i++, j++)
		{
			string id="";
			id = requested [i].Split ('-') [1];

			if (j == 0) 
			{
				go = GameObject.Instantiate(friendRequest);
				pR = go.GetComponent<PanelAppRequest> ();
				pR.setParent (panelMessages,false);
				pR.facebookManager = this;
				pR.selectRequestState (requestState);
				pR.selectAction (PanelAppRequest.EAction.SEND);
				pR.selectTextButton ();
				pR.selectImage ();
			}

			pR.addIds (requested[i]);
			Texture texture = getfriendTextureByID (id);

			if(texture == null)
			{
				StartCoroutine (requestImage (pR, id));
			}
			else
			{
				pR.addFriendPicture (texture);
			}


			if(askedKeys) 
			{	
				if (i == requested.Count - 1 || j == maxUsersPerMessage || requested[i].Split(',')[3] != requested[i+1].Split(',')[3]) 
				{
					pR.selectText ();
				}

				if (j == maxUsersPerMessage || requested[i].Split(',')[3] != requested[i+1].Split(',')[3] ) 
				{
					j = 0;
				}
			}
			else
			{
				if (i == requested.Count - 1 || j == maxUsersPerMessage  ) 
				{
					pR.selectText ();
				}

				if (j == maxUsersPerMessage) 
				{
					j = 0;
				}
			}
		}
	}

	protected Texture getfriendTextureByID (string id)
	{
		if(!FacebookPersistentData.instance.containTextureByID(id))
		{
			Texture friendTexture = FacebookPersistentData.instance.getTextureFromURL (FacebookPersistentData.instance.getFriendPictureUrl (FacebookPersistentData.instance.getFriendInfo (id)));
			return friendTexture;
		}
		return FacebookPersistentData.instance.friendsImage [id];
	}


	IEnumerator requestImage(PanelAppRequest pR, string id)
	{
		int requested = 0;
		FacebookPersistentData.instance.getTextureFromURL (FacebookPersistentData.instance.getFriendPictureUrl (FacebookPersistentData.instance.getFriendInfo (id)),delegate(Texture pictureTexture)
			{
				if(pictureTexture != null)
				{	
					FacebookPersistentData.instance.addFriendImage(id,pictureTexture);
					pR.addFriendPicture (pictureTexture);
				}
				requested = 1;
			});

		yield return new WaitUntil(()=>requested == 1 );
	}

	protected void actualizeMessageNumber()
	{
		facebookNews.actualizeMessageNumber (messageCount.ToString());
	}

	void OnDestroy() {
		fbLog.onLoginComplete -= OnLoginComplete;

		fbGraph.OnPlayerInfo -= showPlayerInfo;
		fbGraph.OnGetGameFriends -= addGameFriends;
		fbGraph.OnGetInvitableFriends -= addInivitableFriends;
		fbGraph.OnGetFriendTextures -= addFriendsImage;

		fbGraph.OnGetAppRequest -= chanelData;

		fbGraph.onFinishGettingInfo -= fillMessageData;
	}

	public void fillRequestPanel(List<object> friends, FBFriendsRequestPanel.EFriendsType friendType)
	{
		fbRequestPanel.initializeFriendsController (FBFriendsRequestPanel.ERequestType.ASK_LIFES, friends,friendType);
	}
}

