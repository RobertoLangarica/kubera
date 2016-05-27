using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookPersistentData : MonoBehaviour {

	//Solo existe una sola instancia en todo el juego de este objeto
	public static FacebookPersistentData instance = null;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	public object currentPlayerInfo = new List<object> ();
	public List<object> gameFriends = new List<object> ();
	public List<object> invitableFriends = new List<object> ();
	public List<object> allFriends = new List<object>();

	public Dictionary<string, Texture> friendsImage = new Dictionary<string, Texture>();

	public bool infoRequested;
	void Awake() 
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag ("persistentData");
		for(int i=1; i< go.Length; i++)
		{
			DestroyImmediate (go [i]);
		}

		//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
		if (go.Length > 1) 
		{
			destroyed = true;
			return;
		}

		instance = this;
		DontDestroyOnLoad(this);

	}

	void Start()
	{
		if(destroyed)
		{
			return;
		}
	}

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
			print ("rea");
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
