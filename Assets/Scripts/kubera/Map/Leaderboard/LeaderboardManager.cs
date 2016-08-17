using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour {

	protected List<LevelLeaderboard> leaderboards = new List<LevelLeaderboard>();
	protected FacebookPersistentData facebook;

	public GameObject slot;

	protected LevelLeaderboard currentLeaderboard = null;

	public int maxLeaderbords = 15;

	public Sprite[] dummyIconImages;

	void Start()
	{
		facebook = FindObjectOfType<FacebookPersistentData> ();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			print (FacebookPersistentData.GetInstance ().getFriendNameById ("10154899709081808"));
			print (FacebookPersistentData.GetInstance ().getSpritePictureById ("10154899709081808"));
			
		}
	}

	protected LevelLeaderboard existLeaderboard(string id)
	{
		for(int i=0; i<leaderboards.Count; i++)
		{
			if(leaderboards[i].id == id)
			{
				return leaderboards[i];
			}
		}
		return null;
	}

	public LevelLeaderboard getLeaderboard(string id, Transform parent)
	{
		LevelLeaderboard leaderboard = null;

		if(isCurrentLeaderboard(id))
		{
			return  currentLeaderboard;
		}
		else if(currentLeaderboard != null)
		{
			currentLeaderboard.showSlots(false);
		}

		leaderboard = existLeaderboard (id);

		if(leaderboard != null)
		{
			currentLeaderboard = leaderboard;
			return leaderboard;
		}

		if(leaderboards.Count == maxLeaderbords)
		{
			print ("todelete");
			disposeLedearboard (leaderboards [maxLeaderbords - 1]);
		}


		GameObject go = new GameObject (id);
		leaderboard = go.AddComponent<LevelLeaderboard> ();
		go.transform.SetParent (this.transform);

		leaderboard.id = id;
		leaderboard.slot = slot;
		leaderboard.parent = parent;

		test[] test = null;
		if(!FBLoggin.GetInstance().isLoggedIn)
		{
			test = new test[Random.Range(3,6)];
		}

		for(int i=0; i<test.Length; i++)
		{
			test [i] = new test ();
			test [i].idFacebook = Random.Range(0,100).ToString();
			test [i].score = Random.Range(100,20000);
			test [i].rank = i+1;
		}

		for(int i=0; i<test.Length; i++)
		{
			Sprite sprite = FacebookPersistentData.GetInstance ().getSpritePictureById (test [i].idFacebook);
			string name = FacebookPersistentData.GetInstance ().getFriendNameById (test [i].idFacebook);
			int score = test[i].score;
			int rank = test[i].rank;

			if(sprite == null)
			{
				sprite = dummyIconImages [Random.Range (0, dummyIconImages.Length)];
			}
			leaderboard.setSlotInfo (sprite,name,score,rank);
		}

		leaderboards.Insert (0,leaderboard);
		currentLeaderboard = leaderboard;
		return leaderboard;
	}

	public bool isCurrentLeaderboard(string id)
	{
		if(currentLeaderboard != null && id == currentLeaderboard.id)
		{
			return  true;
		}
		return  false;
	}

	public void disposeAllLeaderboards()
	{
		for(int i=0; i<leaderboards.Count; i++)
		{
			leaderboards [i].deleteSlots ();
			DestroyImmediate (leaderboards [i].gameObject);
		}

		leaderboards.Clear ();
	}

	public void disposeLedearboard(LevelLeaderboard leaderboard)
	{
		leaderboard.deleteSlots ();
		DestroyImmediate (leaderboard.gameObject);
		leaderboards.Remove (leaderboard);
	}

	public void showCurrentLeaderboard(bool show)
	{
		if(currentLeaderboard != null)
		{
			currentLeaderboard.showSlots (show);
		}
	}

	public void moveCurrentLeaderboardSlots(Transform parent)
	{
		currentLeaderboard.moveSlots (parent);
	}
}
