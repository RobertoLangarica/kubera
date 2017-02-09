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
	public Color currentLevelColor;

	public Sprite starsPassed;
	public Sprite starsUnpassed;

	public Image levelIcon;

	public Sprite levelLockedSprite;
	public Sprite levelNormalSprite;
	public Sprite levelCurrentSprite;

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
	public RectTransform facebookBackgroundRect;
	public Image fbImage;
	public FBPicture fbPicture;

	public delegate void DOnClickNotification(MapLevel pressed);
	public DOnClickNotification OnClickNotification;

	public bool nextLevelIsReached;

	protected ParalaxManager paralaxManager;

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
			levelIcon.sprite = levelNormalSprite;
			break;
		case(EMapLevelsStatus.NORMAL_REACHED):
			levelIcon.sprite = levelCurrentSprite;
			//lvlNameText.color = currentLevelColor;
			break;
		}
	}

	public void updateFacebookFriendPicture(FriendInfo friendInfo)
	{
		fbPicture = facebookBackground.GetComponent <FBPicture>();
		fbPicture.fbId = friendInfo.facebookID;

		Invoke ("pictureRequest", 0.1f);
	}

	protected void pictureRequest()
	{
		facebookBackground.SetActive (true);
		Sprite image = fbPicture.getPicture();
		if(image != null)
		{
			fbImage.sprite = image;
		}
		else
		{
			//TODO poner imagen de reload
			fbPicture.OnFound += pictureFound;
		}
	}

	protected void pictureFound(Sprite picture)
	{
		fbImage.sprite = picture;
	}

	public void noFriend()
	{
		facebookBackground.SetActive (false);
	}

	public void myProgress(bool isConectedToFacebook)
	{
		facebookBackground.SetActive (true);
		if(isConectedToFacebook)
		{
			fbPicture = facebookBackground.GetComponent <FBPicture>();
			fbPicture.fbId = Kubera.Data.Sync.KuberaSyncManger.GetCastedInstance<Kubera.Data.Sync.KuberaSyncManger>().facebookUserId;

			Invoke ("pictureRequest", 0.1f);
		}
	}

	public void moveProgress(MapLevel nextLevel)
	{
		//facebookBackground.transform.DOShakePosition (1000);
		facebookBackground.transform.SetParent (nextLevel.gameObject.transform,true);

		facebookBackgroundRect.anchorMax = nextLevel.facebookBackgroundRect.anchorMax;
		facebookBackgroundRect.anchorMin = nextLevel.facebookBackgroundRect.anchorMin;

		facebookBackground.transform.DOLocalMove (nextLevel.facebookBackground.transform.localPosition, 1.5f).OnComplete(()=>
			{
				facebookBackground.transform.SetSiblingIndex (0); 
				Invoke("finish",0.5f);
			});
	}

	protected void finish()
	{
		paralaxManager.finish();
	}

	public void setParalaxManager(ParalaxManager paralaxManager)
	{
		this.paralaxManager = paralaxManager;
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
