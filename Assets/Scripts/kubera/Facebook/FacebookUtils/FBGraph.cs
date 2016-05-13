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

	public delegate void DOnGetPlayerInfo(string name, Texture picture);
	public delegate void DOnGetFriends(List<object> friends);
	public delegate void DOnAddTextureFriend(string id, Texture picture);
	public delegate void DOnFinishGetingFriends();

	public DOnGetPlayerInfo OnPlayerInfo;
	public DOnGetFriends OnGetGameFriends;
	public DOnGetFriends OnGetInvitableFriends;
	public DOnAddTextureFriend OnGetFriendTextures;
	public DOnFinishGetingFriends onFinishGettingFriends;

	public void GetPlayerInfo()
	{
		string queryString = "/me?fields=id,first_name,picture.width(120).height(120)";
		FB.API(queryString, HttpMethod.GET, GetPlayerInfoCallback);
	}

	private void GetPlayerInfoCallback(IGraphResult result)
	{
		Debug.Log("GetPlayerInfoCallback");
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			return;
		}
		//Debug.Log(result.RawResult);

		// Save player name
		string name = "";
		if (result.ResultDictionary.TryGetValue("first_name", out name))
		{
			//name = nombre del usuario
			//GameStateManager.Username = name;
		}

		//Fetch player profile picture from the URL returned
		string playerImgUrl = GraphUtil.DeserializePictureURL(result.ResultDictionary);
		GraphUtil.LoadImgFromURL(playerImgUrl, delegate(Texture pictureTexture)
			{
				// Setup the User's profile picture
				if (pictureTexture != null)
				{
					//Imagen del usuario
					//GameStateManager.UserTexture = pictureTexture;
					OnPlayerInfo (name, pictureTexture);
				}
				//print("finishPLAYERINFO");
			});


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
		string queryString = "/me/friends?fields=id,first_name,picture.width(128).height(128)&limit=200";
		FB.API(queryString, HttpMethod.GET, GetFriendsCallback);
	}

	private void GetFriendsCallback(IGraphResult result)
	{
		Debug.Log("GetFriendsCallback");
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

			onFinishGettingFriends ();
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
		string queryString = "/me/invitable_friends?fields=id,first_name,picture.width(128).height(128)&limit=200";
		FB.API(queryString, HttpMethod.GET, GetInvitableFriendsCallback);
	}

	private void GetInvitableFriendsCallback(IGraphResult result)
	{
		Debug.Log("GetInvitableFriendsCallback");
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
		}
	}

	private void CacheFriends (List<object> newFriends)
	{
		Dictionary<string, object> friend;

		foreach(Dictionary<string,object> f in newFriends)
		{
			friend =  f as Dictionary<string, object>;

			string playerImgUrl = GraphUtil.DeserializePictureURL(friend);

			getTextureFromURL (playerImgUrl,(string)friend ["id"]);
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

				}
			});
		return texture;
	}
	#endregion

}
