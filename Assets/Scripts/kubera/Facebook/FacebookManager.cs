using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Facebook.Unity;
using UnityEngine.UI;
using Facebook.MiniJSON;
using Kubera.Data;
using Kubera.Data.Sync;
using utils.gems.sync;

public class FacebookManager : Manager<FacebookManager>
{
	public FBGraph fbGraph;
	public FacebookNews facebookNews;
	public MapManager mapManager;
	public FBFriendsRequestPanel fbRequestPanel;

	public Transform panelMessages;
	public GameObject friendRequest;
	public GameObject FacebookConectMessage;

	[HideInInspector]public bool facebookConectMessageCreated;

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

	public delegate void DOnSuccesRequest();
	public DOnSuccesRequest OnSuccesRequest;

	protected override void Awake ()
	{
		base.Awake ();
		fbRequestPanel.facebookManager = this;
	}

	void Start()
	{
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			OnLoginComplete ();
		}
		else if(!facebookConectMessageCreated)
		{
			//crear FacebookConectMessage
			//print ("creando mensaje para conectar");
			conectFacebook = Instantiate (FacebookConectMessage);
			conectFacebook.transform.SetParent (panelMessages,false);
			facebookConectMessageCreated = true;
		}
		//KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.OnLoginSuccessfull += OnLoginComplete;

		fbGraph.OnPlayerInfo += showPlayerInfo;
		fbGraph.OnGetGameFriends += addGameFriends;
		fbGraph.OnGetInvitableFriends += addInivitableFriends;
		fbGraph.OnGetFriendTextures += addUsersImage;
		fbGraph.onFinishGettingFriends += mergeFriends;

		fbGraph.OnGetAppRequest += chanelData;

		fbGraph.onFinishGettingInfo += fillMessageData;
	}

	protected void loginCompleted()
	{
		Invoke ("OnLoginComplete", 1);
	}

	protected void OnLoginComplete(string message = "")
	{
		if (KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			//print ("is loggedIn--------------------------");
			if(canRequestMoreFriends())
			{
				//print ("canRequestMoreFriends--------------------------");
				fbGraph.GetPlayerInfo();
				fbGraph.GetFriends();
				fbGraph.GetInvitableFriends();

				FacebookPersistentData.GetInstance ().infoRequested = true;
			}
			else
			{
				fbGraph.setActive ();

				fillRequestPanel (FacebookPersistentData.GetInstance().gameFriends, FBFriendsRequestPanel.EFriendsType.GAME);
				fillRequestPanel (FacebookPersistentData.GetInstance().allFriends, FBFriendsRequestPanel.EFriendsType.ALL);
			}

			//print ("getFriendsAppRequests--------------------------");
			fbGraph.getFriendsAppRequests ();
			if(conectFacebook != null)
			{
				DestroyImmediate (conectFacebook);
			}
		}
	}

	public bool canRequestMoreFriends()
	{
		if(FacebookPersistentData.GetInstance().infoRequested)
		{
			return false;
		}
		return true;
	}

	protected void showPlayerInfo(string id, string name)
	{
		FacebookPersistentData.GetInstance ().setPlayerId (id);
	}

	protected void addGameFriends(List<object> gameFriends)
	{
		FacebookPersistentData.GetInstance().addGameFriend (gameFriends);

		fillRequestPanel (gameFriends, FBFriendsRequestPanel.EFriendsType.GAME);
	}

	protected void addInivitableFriends(List<object> invitableFriends)
	{
		FacebookPersistentData.GetInstance().addInvitableFriend (invitableFriends);
	}

	protected void mergeFriends()
	{
		FacebookPersistentData.GetInstance().mergeFriends ();
		fillRequestPanel (FacebookPersistentData.GetInstance().allFriends, FBFriendsRequestPanel.EFriendsType.ALL);
	}

	protected void addUsersImage(string id, Texture image,bool myPicture = false)
	{
		if(myPicture)
		{
			FacebookPersistentData.GetInstance().addFriendImage (id, image);
			return;
		}
		
		if(FacebookPersistentData.GetInstance().containTextureByID(id))
		{			
			FacebookPersistentData.GetInstance().addFriendImage (id, image);
		}
	}

	public void acceptGift(bool life, int giftCount,GameObject requestToDelete,List<string> requestId, string bossReached = "0")
	{
		//TODO: 
		if(life)
		{
			//print("recibi " + giftCount + ": vidas");	
			LifesManager.GetInstance ().giveALife (giftCount);
		}
		else
		{
			//print("recibi " + giftCount + ": llaves");
			mapManager.unlockBoss (bossReached);
		}
		for(int i=0; i<requestId.Count; i++)
		{
			deleteAppRequest (requestId [i]);
		}
		DestroyImmediate (requestToDelete);
	}

	public void sendGift (bool life, List<string> friendsIDs,GameObject requestToDelete,List<string> requestId, string bossReached = "0")
	{
		if(life)
		{
			sendLife (friendsIDs,requestId);
		}
		else
		{
			sendKey (friendsIDs,bossReached,requestId);
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

	public void sendLife(List<string> friendsIDs, List<string> requestID)
	{
		print ("sendlife");
		/*if (!canPublish())
		{
			return;
		}*/
		string myFBID = FacebookPersistentData.GetInstance ().getPlayerId();

		FB.AppRequest ("Here, take this life!", // A message for the user
			OGActionType.SEND, // Can be .Send or .AskFor depending on what you want to do with the object.
			"101162080284933", // Here we put the object id we got as a result before.		             
			friendsIDs,// The id of the sender.
			//null,20,
			"SendLife", // Here you can put in any data you want
			"Send a life to your friend", // A title
			delegate (IAppRequestResult result) {
				
				if(result.Error == null)
				{
					for(int i=0; i<friendsIDs.Count; i++)
					{						
						ShopikaSyncManager.GetCastedInstance<ShopikaSyncManager>().registerInvite(myFBID,friendsIDs[i]);
						deleteAppRequest(requestID[i]);
					}
				}
				Debug.Log(result.RawResult);
			}
		);
	}

	public void sendKey(List<string> friendsIDs,string bossReached,List<string> requestID)
	{
		/*if (!canPublish())
		{
			return;
		}*/

		string myFBID = FacebookPersistentData.GetInstance ().getPlayerId();
		print (bossReached);

		FB.AppRequest ("Here, take this key!", // A message for the user
			OGActionType.SEND, // Can be .Send or .AskFor depending on what you want to do with the object.
			"795229890609809", // Here we put the object id we got as a result before.		             
			friendsIDs,// The id of the sender.
			//null,20,
			"SendKey,"+bossReached, // Here you can put in any data you want
			"Send a key to your friend", // A title
			delegate (IAppRequestResult result) {
				if(result.Error == null)
				{
					for(int i=0; i<friendsIDs.Count; i++)
					{						
						ShopikaSyncManager.GetCastedInstance<ShopikaSyncManager>().registerInvite(myFBID,friendsIDs[i]);
						deleteAppRequest(requestID[i]);
					}
				}
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askKey(List<string> idsFriends)
	{
		if(idsFriends.Count>50)
		{
			return;
		}
		FB.AppRequest ("Give me a key!", OGActionType.ASKFOR, "795229890609809", idsFriends, "askKey,"+ mapManager.bossLockedPopUp.fullLvlName, // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {

				if(result.Error == null)
				{
					if(OnSuccesRequest != null)
					{
						OnSuccesRequest();
					}
				}
				Debug.Log (result.RawResult);
			}
		);
	}

	public void askLife(List<string> idsFriends)
	{
		FB.AppRequest ("Give me a life!", OGActionType.ASKFOR, "101162080284933", idsFriends, "askLife", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				if(result.Error == null)
				{
					if(OnSuccesRequest != null)
					{
						OnSuccesRequest();
					}
				}
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
		}
	}

	protected void saveDataOnList(string type,string firstName, string playerID, string requestID)
	{
		//print (requestID);
		//print (type);
		switch (type) {
		case "askLife":
			if(idExistOnList (askedLifes,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				gotMessage ();
				addToList (askedLifes, firstName, playerID, requestID);
			}
			break;
		case "SendLife":			
			if(idExistOnList (giftLifes,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				gotMessage ();
				addToList (giftLifes, firstName, playerID, requestID);
			}
			break;
		default:
			//llaves que me pidieron
			if (type.Contains("askKey"))
			{
				string[] splitType = type.Split (',');
				string bossReached = "";
				if(splitType.Length >1)
				{
					bossReached = splitType [1];
				}

				if(idExistOnList (askedKeys,playerID) )
				{
					deleteAppRequest (requestID);
				}
				else
				{
					gotMessage ();
					addToList (askedKeys, firstName,playerID, requestID, bossReached);
				}
			}
			//llaves que pedí
			else if(type.Contains("SendKey"))
			{
				string[] splitType = type.Split (',');
				string bossReached = "";
				if(splitType.Length >1)
				{
					bossReached = splitType [1];
				}

				if(giftKeys.Count == maxUsersPerMessage || idExistOnList (giftKeys,playerID )|| !(DataManagerKubera.GetInstance () as DataManagerKubera).isLevelLocked(bossReached))
				{

					//print ("delete boosKey");
					deleteAppRequest (requestID);
				}
				else
				{
					gotMessage ();
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

	protected void addToList (List<string> List,string firstName, string playerID, string requestID,string bossReached = "")
	{
		if(bossReached == "")
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
		//print (id);
		FB.API (id, HttpMethod.DELETE, deleteCallBackRequest);
	}

	protected void deleteCallBackRequest(IGraphResult result)
	{
		if (result.Error != null) 
		{
			//Debug.LogError (result.Error);
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
				pR.selectAction (action);
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

				if (requested.Count == 1 ||j == maxUsersPerMessage || requested[i].Split(',')[3] != requested[i+1].Split(',')[3] ) 
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
		if(!FacebookPersistentData.GetInstance().containTextureByID(id))
		{
			Texture friendTexture = FacebookPersistentData.GetInstance().getTextureFromURL (FacebookPersistentData.GetInstance().getFriendPictureUrl (FacebookPersistentData.GetInstance().getFriendInfo (id)));
			return friendTexture;
		}
		return FacebookPersistentData.GetInstance().facebookUsersImage [id];
	}


	IEnumerator requestImage(PanelAppRequest pR, string id)
	{
		int requested = 0;
		FacebookPersistentData.GetInstance().getTextureFromURL (FacebookPersistentData.GetInstance().getFriendPictureUrl (FacebookPersistentData.GetInstance().getFriendInfo (id)),delegate(Texture pictureTexture)
			{
				if(pictureTexture != null)
				{	
						FacebookPersistentData.GetInstance().addFriendImage(id,pictureTexture);
					pR.addFriendPicture (pictureTexture);
				}
				requested = 1;
			});

		yield return new WaitUntil(()=>requested == 1 );
	}

	protected void actualizeMessageNumber()
	{
		facebookNews.actualizeMessageNumber (messageCount);
	}

	void OnDestroy() 
	{
		if (KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ()) 
		{
			KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.OnLoginSuccessfull -= OnLoginComplete;
		}

		fbGraph.OnPlayerInfo -= showPlayerInfo;
		fbGraph.OnGetGameFriends -= addGameFriends;
		fbGraph.OnGetInvitableFriends -= addInivitableFriends;
		fbGraph.OnGetFriendTextures -= addUsersImage;

		fbGraph.OnGetAppRequest -= chanelData;

		fbGraph.onFinishGettingInfo -= fillMessageData;
	}

	public void fillRequestPanel(List<object> friends, FBFriendsRequestPanel.EFriendsType friendType)
	{
		fbRequestPanel.initializeFriendsController (FBFriendsRequestPanel.ERequestType.ASK_LIFES, friends,friendType);
	}

	public void updateMessagesNumber(int messagesProcessed)
	{
		facebookNews.updateCurrentMessages (messagesProcessed);
	}
}

