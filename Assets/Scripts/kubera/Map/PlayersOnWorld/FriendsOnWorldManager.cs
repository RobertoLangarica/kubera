using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsOnWorldManager : MonoBehaviour {

	public List<FriendsOnWorld> friendsOnWorlds = new List<FriendsOnWorld>();

	protected FriendsOnWorld currentFriends;

	public FriendsOnWorld existFriendsOnWorld(string world)
	{
		for(int i=0; i<friendsOnWorlds.Count; i++)
		{
			if(friendsOnWorlds[i].world == world)
			{
				currentFriends = friendsOnWorlds [i];
				return friendsOnWorlds[i];
			}
		}
		return null;
	}

	public FriendsOnWorld getNewFriendsOnWorld(string world, string[] levels, string[] facebokID)
	{
		FriendsOnWorld friendsOnWorld = new FriendsOnWorld ();
		
		friendsOnWorld.world = world;

		for(int i=0; i<levels.Length; i++)
		{
			FriendInfo player = new FriendInfo();
			player.level = levels[i];
			player.facebookID = facebokID[i];
			friendsOnWorld.friendsInfo.Add (player);
		}
		friendsOnWorlds.Add (friendsOnWorld);
		currentFriends = friendsOnWorld;
		return currentFriends;
	}

	public FriendInfo getFriendOnLevel(int world, string level)
	{
		for(int i=0; i<currentFriends.friendsInfo.Count; i++)
		{
			if(currentFriends.friendsInfo[i].level == level)
			{
				return currentFriends.friendsInfo [i];
			}
		}
		return null;
	}
}
