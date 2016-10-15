using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookPersistentData : Manager<FacebookPersistentData> 
{
	public string currentPlayerId;
	public List<object> gameFriends = new List<object> ();
	public List<object> invitableFriends = new List<object> ();
	public List<object> allFriends = new List<object>();

	public Dictionary<string, Texture> facebookUsersImage = new Dictionary<string, Texture>();

	public bool infoRequested;

	public void setPlayerId (string id)
	{
		currentPlayerId = id;
	}

	public string getPlayerId()
	{
		return currentPlayerId;
	}

	public bool containTextureByID (string id)
	{
		if(!facebookUsersImage.ContainsKey(id))
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
			//print ("addFriendImage  "+ id);
			facebookUsersImage.Add (id, texture);
		}
		else
		{
			//print ("friend image exist   "+ id);
		}
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

	public Texture getTextureById(string id)
	{
		if(facebookUsersImage.ContainsKey(id))
		{
			return facebookUsersImage [id];
		}
		return null;
	}

	protected Texture2D getFriendPicture(Texture picture)
	{	
		Texture2D tPicture = null;

		if(picture != null)
		{
			tPicture = picture as Texture2D;
		}
		return tPicture;
	}

	protected Sprite getFriendPicture(Texture2D picture)
	{
		Sprite sPicture = null;

		if(picture != null)
		{
			sPicture = Sprite.Create (picture, new Rect (0, 0, picture.width, picture.height), new Vector2 (0, 0));
		}
		return sPicture;
	}

	public Sprite getSpritePictureById(string id)
	{
		return getFriendPicture (getFriendPicture (getTextureById(id)));
	}

	public string getFriendNameById(string id)
	{
		Dictionary<string,object> friendInfo = getFriendInfo (id);
		
		if(friendInfo != null)
		{
			return ((string)friendInfo ["name"]).Split (' ') [0];
		}

		return "";
	}
}
