using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

	public Color lockedColor;
	public Color normalReachedColor;
	public Color bossPassedColor;
	public Color starsColor;

	public Image levelIcon;
	public Image levelBossIcon;
	public List<Image> levelStars;

	public string lvlName;

	public bool isBoss;

	public int friendsNeeded;
	public int gemsNeeded;
	public int starsNeeded;

	public delegate void DOnClickNotification(MapLevel pressed);
	public DOnClickNotification OnClickNotification;

	public void updateText()
	{
		lvlNameText.text = lvlName.Replace ("0","");
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
				levelStars [i].color = starsColor;
			}
			else
			{
				levelStars [i].gameObject.SetActive (false);
			}
		}
	}

	public void updateStatus()
	{
		switch (status) 
		{
		case(EMapLevelsStatus.BOSS_LOCKED):
			levelBossIcon.gameObject.SetActive (true);
			levelBossIcon.color = levelIcon.color = lockedColor;
			break;
		case(EMapLevelsStatus.NORMAL_LOCKED):
			levelBossIcon.color = levelIcon.color = lockedColor;
			break;
		case(EMapLevelsStatus.BOSS_PASSED):
			levelBossIcon.color = levelIcon.color = bossPassedColor;
			levelBossIcon.gameObject.SetActive (true);
			break;
		case(EMapLevelsStatus.BOSS_REACHED):
		case(EMapLevelsStatus.BOSS_UNLOCKED):
		case(EMapLevelsStatus.NORMAL_PASSED):
		case(EMapLevelsStatus.NORMAL_REACHED):
			levelIcon.color = normalReachedColor;
			break;
		}
	}

	public void onClick()
	{
		print (status);

		if (OnClickNotification != null) 
		{
			OnClickNotification (this);
		}
	}
}
