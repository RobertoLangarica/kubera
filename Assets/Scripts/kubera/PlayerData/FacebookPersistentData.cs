using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookPersistentData : Manager<FacebookPersistentData> 
{
	public object currentPlayerInfo = new List<object> ();
	public List<object> gameFriends = new List<object> ();
	public List<object> invitableFriends = new List<object> ();
	public List<object> allFriends = new List<object>();

	public Dictionary<string, Texture> friendsImage = new Dictionary<string, Texture>();

	public bool infoRequested;


	public bool containTextureByID (string id)
	{
		if(!friendsImage.ContainsKey(id))
		{
			return false;
		}
		return true;
	}

	public bool containObjectInList (object obj, List<object> list)
	{
		if(list.Contains(obj))
		{
			return true;
		}
		return false;
	}

	public void addFriendImage(string id, Texture texture)
	{
		if(!containTextureByID(id))
		{
			friendsImage.Add (id, texture);
		}
		else
		{
			print ("friend image exist");
		}
	}

	public Texture getTextureById(string id)
	{
		return friendsImage [id];
	}

	public void addInvitableFriend(List<object> friends)
	{
		invitableFriends.AddRange (friends);
	}

	public void addGameFriend(List<object> friends)
	{
		gameFriends.AddRange (friends);
	}

	public Dictionary<string,object> getFriendInfo(string id)
	{
		for(int i=0; i<gameFriends.Count; i++)
		{
			Dictionary<string,object> friendInfo = ((Dictionary<string,object>)(gameFriends [i]));
			if((string)friendInfo ["id"] == id)
			{
				return friendInfo;
			}
		}
		return null;
	}

	public string getFriendPictureUrl(object friendInfo)
	{
		return GraphUtil.DeserializePictureURL(friendInfo);
	}

	public Texture getTextureFromURL (string url,System.Action<Texture> callBack = null)
	{
		Texture picture = new Texture();
		GraphUtil.LoadImgFromURL(url, delegate(Texture pictureTexture)
			{
				// Setup the User's profile picture
				if (pictureTexture != null)
				{
					if(callBack != null)
					{
						callBack (pictureTexture);
					}
					picture = pictureTexture;
				}
			});
		return picture;
	}

	public void mergeFriends()
	{
		allFriends.AddRange (gameFriends);
		allFriends.AddRange (invitableFriends);
	}
}
