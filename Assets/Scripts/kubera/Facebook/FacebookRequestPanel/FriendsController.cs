using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FriendsController : MonoBehaviour {

	public GridLayoutGroup gridLayout;
	public GameObject requestFriend;

	public List<Friend> friends;

	public delegate void DOnActivateFriendToggle(bool activated);
	public DOnActivateFriendToggle OnActivated;
	public Transform friendPanel;

	void Start()
	{
		Vector2 cellSize;
		cellSize.x = Screen.width * 0.5f ;
		cellSize.x = cellSize.x - gridLayout.spacing.x * 0.5f;

		cellSize.y = Screen.height / 9;
		gridLayout.cellSize = cellSize;
		initializeFriends ();
	}

	public void initializeFriends()
	{
		for(int i=0; i<friends.Count; i++)
		{
			friends [i].OnActivated += activated;
		}
	}

	public void selectAllFriends(bool activate)
	{
		for(int i=0; i<friends.Count; i++)
		{
			friends [i].selected.isOn = activate;
		}
	}

	public void activated (bool activated)
	{
		OnActivated (activated);
	}

	public bool isAllFriendsSelected()
	{
		for(int i=0; i<friends.Count; i++)
		{
			if(!friends [i].selected.isOn)
			{
				return false;
			}
		}
		return true;
	}

	public List<string> getFriendsActivatedID()
	{
		List<string> ids = new List<string> ();

		for(int i=0; i<friends.Count; i++)
		{
			if(friends [i].selected.isOn)
			{
				print("id: "+ friends[i].id);
				ids.Add (friends [i].id);
			}
		}

		return ids;
	}

	public void addFriend(string id, string imageURL, string userName,Texture image = null)
	{
		GameObject go = Instantiate (requestFriend)as GameObject;
		go.transform.SetParent (friendPanel,false);
		Friend friend;
		friend = go.GetComponent<Friend>();

		friend.id = id;
		if(image != null)
		{
			friend.setFriendImage (image);
		}
		else
		{
			friend.getTextureFromURL (imageURL);
		}
		friend.userName.text = userName;
		friends.Add (friend);
	}
}
