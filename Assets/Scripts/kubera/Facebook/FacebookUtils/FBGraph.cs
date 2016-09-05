using UnityEngine;
using System;
using System.Collections.Generic;
using Facebook.Unity;

public class FBGraph : MonoBehaviour 
{
	#region PlayerInfo
	// Once a player successfully logs in, we can welcome them by showing their name
	// and profile picture on the home screen of the game. This information is returned
	// via the /me/ endpoint for the current player. We'll call this endpoint via the
	// SDK and use the results to personalize the home screen.
	//
	// Make a Graph API GET call to /me/ to retrieve a player's information
	// See: https://developers.facebook.com/docs/graph-api/reference/user/

	public delegate void DOnGetPlayerInfo(string id, string name);
	public delegate void DOnGetFriends(List<object> friends);
	public delegate void DOnGetAppRequest(List<object> friends);
	public delegate void DOnAddTextureFriend(string id, Texture picture);
	public delegate void DOnFinishGetingInfo();

	public DOnGetPlayerInfo OnPlayerInfo;
	public DOnGetFriends OnGetGameFriends;
	public DOnGetFriends OnGetInvitableFriends;
	public DOnGetAppRequest OnGetAppRequest;
	public DOnAddTextureFriend OnGetFriendTextures;
	public DOnFinishGetingInfo onFinishGettingInfo;
	public DOnFinishGetingInfo onFinishGettingFriends;

	protected bool gameFriendsReady;
	protected bool invitableFriendsReady;
	protected bool appRequestReady;
	protected bool playerInfoReady;
	protected bool texturesFriendReady;// = true;
	protected int texturesCount;
	protected int texturesAdded;
	public void GetPlayerInfo()
	{
		string queryString = "/me?fields=id,first_name,picture.width(60).height(60)";
		FB.API(queryString, HttpMethod.GET, GetPlayerInfoCallback);
	}

	private void GetPlayerInfoCallback(IGraphResult result)
	{
		bool textureReady = false;
		bool infoReady = false;

		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			return;
		}
		//Debug.Log(result.RawResult);

		// Save player id
		string id = "";
		if (result.ResultDictionary.TryGetValue("id", out id))
		{
			
		}

		string name = "";
		result.ResultDictionary.TryGetValue ("first_name", out name);
		

		//Fetch player profile picture from the URL returned
		string playerImgUrl = GraphUtil.DeserializePictureURL(result.ResultDictionary);
		GraphUtil.LoadImgFromURL(playerImgUrl, delegate(Texture pictureTexture)
			{
				// Setup the User's profile picture
				if (pictureTexture != null)
				{
					//Imagen del usuario
					//GameStateManager.UserTexture = pictureTexture;

				}
				OnGetFriendTextures (id,pictureTexture);
				textureReady = true;
				setPlayerInfoReady(infoReady,textureReady);
				//print("finishPLAYERINFO");
			});
		
		OnPlayerInfo (id,name);
		infoReady = true;
		setPlayerInfoReady(infoReady,textureReady);
	}

	protected void setPlayerInfoReady(bool nameId, bool texture)
	{
		if(!playerInfoReady)
		{
			if(nameId && texture)
			{
				playerInfoReady = true;
				AllinfoGathered ();
			}
		}
	}

	// In the above request it takes two network calls to fetch the player's profile picture.
	// If we ONLY needed the player's profile picture, we can accomplish this in one call with the /me/picture endpoint.
	//
	// Make a Graph API GET call to /me/picture to retrieve a players profile picture in one call
	// See: https://developers.facebook.com/docs/graph-api/reference/user/picture/
	public void GetPlayerPicture()
	{
		FB.API(GraphUtil.GetPictureQuery("me", 128, 128), HttpMethod.GET, delegate(IGraphResult result)
			{
				Debug.Log("PlayerPictureCallback");
				if (result.Error != null)
				{
					Debug.LogError(result.Error);
					return;
				}
				if (result.Texture ==  null)
				{
					Debug.Log("PlayerPictureCallback: No Texture returned");
					return;
				}

				//Imagen del usuario
				//GameStateManager.UserTexture = pictureTexture;
			});
	}
	#endregion

	#region Friends
	// We can fetch information about a player's friends via the Graph API user edge /me/friends
	// This endpoint returns an array of friends who are also playing the same game.
	// See: https://developers.facebook.com/docs/graph-api/reference/user/friends
	//
	// We can use this data to provide a set of real people to play against, showing names
	// and pictures of the player's friends to make the experience feel even more personal.
	//
	// The /me/friends edge requires an additional permission, user_friends. Without
	// this permission, the response from the endpoint will be empty. If we know the user has
	// granted the user_friends permission but we see an empty list of friends returned, then
	// we know that the user has no friends currently playing the game.
	//
	// Note:
	// In this instance we are making two calls, one to fetch the player's friends who are already playing the game
	// and another to fetch invitable friends who are not yet playing the game. It can be more performant to batch 
	// Graph API calls together as Facebook will parallelize independent operations and return one combined result.
	// See more: https://developers.facebook.com/docs/graph-api/making-multiple-requests
	//
	public void GetFriends ()
	{
		string queryString = "/me/friends?fields=id,name,picture.width(60).height(60)";
		FB.API(queryString, HttpMethod.GET, GetFriendsCallback);
	}

	private void GetFriendsCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			return;
		}
		//Debug.Log(result.RawResult);

		// Store /me/friends result
		object dataList;
		if (result.ResultDictionary.TryGetValue("data", out dataList))
		{
			var friendsList = (List<object>)dataList;
			CacheFriends(friendsList);
			OnGetGameFriends (friendsList);
			gameFriendsReady = true;
			mergeFriends ();
			AllinfoGathered ();
		}
	}

	// We can fetch information about a player's friends who are not yet playing our game
	// via the Graph API user edge /me/invitable_friends
	// See more about Invitable Friends here: https://developers.facebook.com/docs/games/invitable-friends
	//
	// The /me/invitable_friends edge requires an additional permission, user_friends.
	// Without this permission, the response from the endpoint will be empty.
	//
	// Edge: https://developers.facebook.com/docs/graph-api/reference/user/invitable_friends
	// Nodes returned are of the type: https://developers.facebook.com/docs/graph-api/reference/user-invitable-friend/
	// These nodes have the following fields: profile picture, name, and ID. The ID's returned in the Invitable Friends
	// response are not Facebook IDs, but rather an invite tokens that can be used in a custom Game Request dialog.
	//
	// Note! This is different from the following Graph API:
	// https://developers.facebook.com/docs/graph-api/reference/user/friends
	// Which returns the following nodes:
	// https://developers.facebook.com/docs/graph-api/reference/user/
	//
	public void GetInvitableFriends ()
	{
		string queryString = "/me/invitable_friends?fields=id,name,picture.width(60).height(60)&limit=200";
		FB.API(queryString, HttpMethod.GET, GetInvitableFriendsCallback);
	}

	private void GetInvitableFriendsCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			return;
		}
		//Debug.Log("invitable friends "+result.RawResult);

		// Store /me/invitable_friends result
		object dataList;
		if (result.ResultDictionary.TryGetValue("data", out dataList))
		{
			var invitableFriendsList = (List<object>)dataList;
			CacheFriends(invitableFriendsList);
			OnGetInvitableFriends (invitableFriendsList);
			invitableFriendsReady = true;
			mergeFriends ();
			AllinfoGathered ();
		}
	}

	private void CacheFriends (List<object> newFriends)
	{
		Dictionary<string, object> friend;
		texturesCount += newFriends.Count;
		foreach(Dictionary<string,object> f in newFriends)
		{
			friend =  f as Dictionary<string, object>;

			string playerImgUrl = GraphUtil.DeserializePictureURL(friend);

			getTextureFromURL (playerImgUrl,(string)friend ["id"]);
		}
	}

	private void mergeFriends()
	{
		if(gameFriendsReady && invitableFriendsReady)
		{
			onFinishGettingFriends ();
		}
	}

	private Texture getTextureFromURL (string url,string friendID)
	{
		Texture texture = new Texture();
		GraphUtil.LoadImgFromURL(url, delegate(Texture pictureTexture)
			{
				// Setup the User's profile picture
				if (pictureTexture != null)
				{
					//Imagen del usuario
					//GameStateManager.UserTexture = pictureTexture;
					texture = pictureTexture;
					OnGetFriendTextures (friendID, pictureTexture);
					textureAdded();
				}
			});
		return texture;
	}

	private void textureAdded()
	{
		texturesAdded++;
		if(texturesAdded == texturesCount)
		{
			if(gameFriendsReady && invitableFriendsReady)
			{
				texturesFriendReady = true;
				AllinfoGathered ();
			}
		}
	}

	#endregion

	#region Request
	public void getFriendsAppRequests()
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

		if (!result.ResultDictionary.ContainsKey ("apprequests"))
		{
			//deleteAppRequest ((string)dict["id"]);
			appRequestReady = true;
			AllinfoGathered ();
			return;
		}

		//print (dict.Keys.ToCommaSeparateList ());
		object dataObject;
		List<object> apprequestsData = new List<object>();

		//print ("************ " +(string) dict["id"]);
		if (result.ResultDictionary.TryGetValue ("apprequests", out dataObject)) 
		{
			apprequestsData = (List<object>)(((Dictionary<string, object>)dataObject) ["data"]);
		}
		OnGetAppRequest (apprequestsData);
		appRequestReady = true;
		AllinfoGathered ();
	}
	#endregion

	protected void AllinfoGathered()
	{
		
		if(gameFriendsReady&& invitableFriendsReady && appRequestReady && playerInfoReady && texturesFriendReady)
		{
			onFinishGettingInfo ();
		}
	}   
}        