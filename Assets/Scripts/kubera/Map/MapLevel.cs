﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapLevel : MonoBehaviour 
{
	public enum EMapLevelsStatus
	{
		NORMAL_LOCKED,
		NORMAL_UNLOCKED,
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

	public Image levelIcon;
	public Image levelStars;

	protected string lvlName;

	public bool isBoss;

	public int friendsNeeded;
	public int gemsNeeded;
	public int starsNeeded;

	public delegate void DOnClickNotification(MapLevel pressed);
	public DOnClickNotification OnClickNotification;

	void Start()
	{
		lvlName = name.Substring (2).Remove (name.Substring (2).Length-1);
		//lvlNameText.text = lvlName;
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
