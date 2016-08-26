using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MapLevel : MonoBehaviour 
{
	public enum EMapLevelsStatus
	{
		NORMAL_LOCKED,
		NORMAL_REACHED,
		NORMAL_PASSED,
		BOSS_LOCKED,
		BOSS_UNLOCKED,
		BOSS_PASSED,
		BOSS_REACHED
	}

	public enum EMapLevelStars
	{
		NONE,	
		ONE,
		TWO,
		THREE
	}

	public EMapLevelStars stars;
	public EMapLevelsStatus status;
	public Text lvlNameText;

	public Sprite starsPassed;
	public Sprite starsUnpassed;

	public Image levelIcon;

	public Sprite levelLockedSprite;
	public Sprite levelNormalSprite;

	public Sprite levelLockedBossSprite;
	public Sprite levelNormalBossSprite;
	public Sprite levelUnlockedBossSprite;

	public List<Image> levelStars;

	public string lvlName;
	public string fullLvlName;

	public bool isBoss;

	public int friendsNeeded;
	public int gemsNeeded;
	public int starsNeeded;

	public GameObject facebookBackground;
	public Image facebookFriend;
	public FriendPicture friend;

	public delegate void DOnClickNotification(MapLevel pressed);
	public DOnClickNotification OnClickNotification;

	public void updateText()
	{
		for (int i = 0; i < lvlName.Length; i++) 
		{
			if (lvlName [i] == '0') 
			{
				lvlName = lvlName.Remove(i,1);
				i--;
			} 
			else 
			{
				break;
			}
		}

		lvlNameText.text = lvlName;
	}

	public void updateStars()
	{
		int until = -1;

		switch (stars) 
		{
		case(EMapLevelStars.NONE):
			until = 0;
			break;
		case(EMapLevelStars.ONE):
			until = 1;
			break;
		case(EMapLevelStars.TWO):
			until = 2;
			break;
		case(EMapLevelStars.THREE):
			until = 3;
			break;
		}

		for (int i = 0; i < levelStars.Count; i++) 
		{
			if (i < until) 
			{
				levelStars [i].gameObject.SetActive (true);
				levelStars [i].sprite = starsPassed;
			}
			else
			{
				levelStars [i].gameObject.SetActive (false);
			}
		}
	}

	public void updateStatus()
	{
		//print (status + "  "+lvlName);
		switch (status) 
		{
		case(EMapLevelsStatus.BOSS_LOCKED):
			levelIcon.sprite = levelLockedBossSprite;
			break;
		case(EMapLevelsStatus.NORMAL_LOCKED):
			levelIcon.sprite = levelLockedSprite;
			break;
		case(EMapLevelsStatus.BOSS_PASSED):
			levelIcon.sprite = levelNormalBossSprite;
			break;
		case(EMapLevelsStatus.BOSS_REACHED):
		case(EMapLevelsStatus.BOSS_UNLOCKED):
			levelIcon.sprite = levelUnlockedBossSprite;
			break;
		case(EMapLevelsStatus.NORMAL_PASSED):
		case(EMapLevelsStatus.NORMAL_REACHED):
			levelIcon.sprite = levelNormalSprite;
			break;
		}
	}

	public void updateFacebookFriendPicture(FriendInfo friendInfo)
	{
		friend = facebookBackground.GetComponent < FriendPicture>();
		friend.fbId = friendInfo.facebookID;

		Invoke ("pictureRequest", 0.5f);
	}

	protected void pictureRequest()
	{
		facebookBackground.SetActive (true);
		Sprite image = friend.getPicture();
		if(image != null)
		{
			facebookFriend.sprite = image;
		}
		else
		{
			//TODO poner imagen de reload
			friend.OnFound += pictureFound;
		}
	}

	protected void pictureFound(Sprite picture)
	{
		facebookFriend.sprite = picture;
	}

	public void noFriend()
	{
		facebookBackground.SetActive (false);
	}

	public void myProgress()
	{
		facebookBackground.SetActive (true);
	}

	public void moveProgress(MapLevel nextLevel)
	{
		//facebookBackground.transform.DOShakePosition (1000);
		facebookBackground.transform.SetParent (nextLevel.gameObject.transform,true);

		facebookBackground.transform.DOLocalMove (nextLevel.facebookBackground.transform.localPosition, 1.5f).OnComplete(()=>
			{
				facebookBackground.transform.SetSiblingIndex (0); 

				FindObjectOfType<ParalaxManager>().finish();
			});
		facebookBackground.transform.DOScale (nextLevel.facebookBackground.transform.localScale,1.5f);
	}

	public void onClick()
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}
		if (OnClickNotification != null) 
		{
			OnClickNotification (this);
		}
	}
}
