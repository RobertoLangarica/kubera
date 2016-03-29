using UnityEngine;
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

	public Transform panelMessages;
	public GameObject friendRequest;
	public GameObject FacebookConectMessage;

	protected bool facebookConectMessageCreated;

	protected List<string> askedLifes = new List<string>();
	protected List<string> giftLifes = new List<string>();
	protected List<string> askedKeys = new List<string>();
	protected List<string> giftKeys = new List<string>();

	public List<object> friends = new List<object> ();
	public Dictionary<string, Texture> friendImages = new Dictionary<string, Texture>();

	public int maxUsersPerMessage = 5;
	protected int messageCount;

	protected GameObject conectFacebook;

	void Awake()
	{
		fbGraph = FindObjectOfType<FBGraph> ();
		fbLog = FindObjectOfType<FBLog> ();
		facebookNews = FindObjectOfType<FacebookNews> ();

		fbLog.onLoginComplete += OnLoginComplete;

		playerInfo = FindObjectOfType<PlayerInfo> ();

		fbGraph.OnPlayerInfo += showPlayerInfo;
		fbGraph.OnGetFriends += addFriends;
		fbGraph.OnGetFriendTextures += addFriendsTexture;
		fbGraph.onFinishGettingFriends += startFillMessageData;

	}

	protected void OnLoginComplete(bool complete)
	{
		Debug.Log("OnLoginComplete " + complete);

		if (complete)
		{
			// Begin querying the Graph API for Facebook data
			fbGraph.GetPlayerInfo();
			fbGraph.GetFriends();
			fbGraph.GetInvitableFriends();
			//FBGraph.GetScores();

			getFriendsAppRequests ();
			if(conectFacebook != null)
			{
				DestroyImmediate (conectFacebook);
				print (conectFacebook);
			}
		}
		else
		{
			if(!facebookConectMessageCreated)
			{
				//crear FacebookConectMessage
				conectFacebook = Instantiate (FacebookConectMessage);
				conectFacebook.transform.SetParent (panelMessages);
				facebookConectMessageCreated = true;
			}
		}
	}

	protected void showPlayerInfo(string name, Texture picture)
	{
		
	}

	protected void addFriends(List<object> moreFriends)
	{
		friends.AddRange (moreFriends);
	}

	protected void addFriendsTexture(string id, Texture image)
	{
		friendImages.Add (id, image);
	}

	protected Texture getfriendTextureByID (string id)
	{
		return friendImages [id];
	}

	public void acceptGift(bool life, int giftCount)
	{
		if(life)
		{
			print("recibi " + giftCount + ": vidas");	
		}
		else
		{
			print("recibi " + giftCount + ": llaves");	
		}
	}

	public void sendGift(bool life, List<string> friendsIDs)
	{
		if(life)
		{
			sendLife (friendsIDs);
		}
		else
		{
			sendKey (friendsIDs);
		}
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
			"life", // Here you can put in any data you want
			"Send a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void sendKey(List<string> friendsIDs)
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
			"key", // Here you can put in any data you want
			"Send a key to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askLifeToInvitableFriends()
	{
		if (!canPublish())
		{
			return;
		}

		FB.AppRequest ("Give me a life!", // A message for the user
			OGActionType.ASKFOR, // Can be .Send or .AskFor depending on what you want to do with the object.
			"101162080284933", // Here we put the object id we got as a result before.		             
			new List<object> (){ "app_users" },// The id of the sender.
			null,20,
			"askLife", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askKeyToInvitableFriends()
	{
		if (!canPublish())
		{
			return;
		}

		//TODO: objeto de llave no correcto
		FB.AppRequest ("Give me a key!", // A message for the user
			OGActionType.ASKFOR, // Can be .Send or .AskFor depending on what you want to do with the object.
			"795229890609809", // Here we put the object id we got as a result before.		             
			new List<object> (){ "app_users" },// The id of the sender.
			null,20,
			"askKey", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askLifeToFriends()
	{
		if (!canPublish())
		{
			return;
		}

		FB.AppRequest ("Give me a life!", // A message for the user
			OGActionType.ASKFOR, // Can be .Send or .AskFor depending on what you want to do with the object.
			"101162080284933", // Here we put the object id we got as a result before.		             
			null,// The id of the sender.
			null,20,
			"askLife", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
			}
		);
	}

	public void askKeyToFriends()
	{
		if (!canPublish())
		{
			return;
		}

		//TODO: objeto de llave no correcto
		FB.AppRequest ("Give me a key!", // A message for the user
			OGActionType.ASKFOR, // Can be .Send or .AskFor depending on what you want to do with the object.
			"795229890609809", // Here we put the object id we got as a result before.		             
			null,// The id of the sender.
			null,20,
			"askKey", // Here you can put in any data you want
			"Ask a life to your friend", // A title
			delegate (IAppRequestResult result) {
				Debug.Log(result.RawResult);
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

	protected void getFriendsAppRequests()
	{
		FB.API("/me?fields=apprequests{from,data}",HttpMethod.GET,getFriendsAppRequestsCallback);
	}

	protected void getFriendsAppRequestsCallback(IGraphResult result) 
	{
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			return;
		}

		Dictionary<string,object> dict = Json.Deserialize(result.RawResult) as Dictionary<string,object>;

		if (!dict.ContainsKey ("apprequests"))
		{
			//deleteAppRequest ((string)dict["id"]);
			print ("none friendsRequest ");
			return;
		}

		//print (dict.Keys.ToCommaSeparateList ());
		print ("getfriends true");
		object dataObject;
		List<object> apprequestsData = new List<object>();

		//print ("************ " +(string) dict["id"]);
		if (dict.TryGetValue ("apprequests", out dataObject)) 
		{
			apprequestsData = (List<object>)(((Dictionary<string, object>)dataObject) ["data"]);
		}
		chanelData (apprequestsData);
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
			actualizeMessageNumber ();
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
		case "askKey":			
			if(idExistOnList (askedKeys,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				addToList (askedKeys, firstName,playerID, requestID);
			}
			break;
		case "life":			
			if(idExistOnList (giftLifes,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				addToList (giftLifes, firstName, playerID, requestID);
			}
			break;
		case "key":			
			if(idExistOnList (giftKeys,playerID))
			{
				deleteAppRequest (requestID);
			}
			else
			{
				addToList (giftKeys, firstName, playerID, requestID);
			}
			break;
		default:
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

	protected void addToList (List<string> List,string firstName, string playerID, string requestID)
	{
		List.Add (firstName+"-"+playerID+"-"+requestID);
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

	protected void actualizeMessageNumber()
	{
		facebookNews.actualizeMessageNumber (messageCount.ToString());
	}

	protected void startFillMessageData()
	{
		StartCoroutine (waitForFillMessageData());
	}

	IEnumerator waitForFillMessageData()
	{
		yield return new WaitForSeconds(1);
		fillMessageData ();
	}

	protected void fillMessageData ()
	{
		fillData (askedKeys, PanelRequest.ERequestState.KEY, PanelRequest.EAction.SEND);
		fillData (askedLifes, PanelRequest.ERequestState.LIFE, PanelRequest.EAction.SEND);
		fillData (giftKeys, PanelRequest.ERequestState.KEY, PanelRequest.EAction.ACCEPT);
		fillData (giftLifes, PanelRequest.ERequestState.LIFE, PanelRequest.EAction.ACCEPT);
	}

	protected void fillData(List<string> requested, PanelRequest.ERequestState requestState, PanelRequest.EAction action)
	{
		GameObject go;
		PanelRequest pR = null;

		for(int i=0, j=0; i<requested.Count; i++, j++)
		{
			if (j == 0) 
			{
				go = GameObject.Instantiate(friendRequest);
				pR = go.GetComponent<PanelRequest> ();
				pR.setParent (panelMessages);
				pR.facebookManager = this;
				pR.selectRequestState (PanelRequest.ERequestState.KEY);
				pR.selectAction (PanelRequest.EAction.SEND);
				pR.selectTextButton ();
				pR.selectImage ();
			}

			pR.addIds (requested[i]);

			pR.addFriendPicture (getfriendTextureByID (requested [i].Split ('-')[1]));


			if(i == requested.Count-1 || j == maxUsersPerMessage)
			{
				pR.selectText ();
			}

			if(j==maxUsersPerMessage)
			{
				j = 0;
			}
		}
	}

}

