﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Data;

public class MapManager : MonoBehaviour 
{
	public const string fullLifes_PopUp 	= "FullLifes";
	public const string missingLifes_PopUp 	= "MissingLifes";
	public const string noLifes_PopUp 		= "NoLifes";

	public ScrollRect scrollRect;
	public GameObject modal;
	public BossLocked bossLockedPopUp;

	public int currentWorld;
	public List<GameObject> worlds;

	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;
	protected ParalaxManager paralaxManager;

	protected List<MapLevel> mapLevels;

	public FBFriendsRequestPanel fbFriendsRequestPanel;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();
		paralaxManager = FindObjectOfType<ParalaxManager> ();

		popUpManager.OnPopUpCompleted = OnPopupCompleted;

		//selectLevel (currentWorld);

		//initializeLevels ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			selectLevel (currentWorld);

			initializeLevels ();
		}
	}

	protected void stopInput(bool stopInput)
	{
		modal.SetActive (stopInput);
	}

	public void openPopUp(string popUpName)
	{
		popUpManager.activatePopUp (popUpName);
		stopInput (true);
	}

	private void OnPopupCompleted(string action ="")
	{
		stopInput(false);
		switch (action) {
		case "needLifes":
			openPopUp ("fbFriendsRequestPanel");
			fbFriendsRequestPanel.openFriendsRequestPanel (FBFriendsRequestPanel.ERequestType.ASK_LIFES);
			break;
		case "needKeys":
			popUpManager.activatePopUp ("fbFriendsRequestPanel");
			fbFriendsRequestPanel.openFriendsRequestPanel (FBFriendsRequestPanel.ERequestType.ASK_KEYS);
			break;
		default:
			break;
		}
	}

	protected void selectLevel(int world)
	{
		currentWorld = world;
	
		mapLevels = new List<MapLevel>(worlds[currentWorld].GetComponentsInChildren<MapLevel> ());
	}

	protected void initializeLevels()
	{
		Debug.Log ((LevelsDataManager.GetInstance() as LevelsDataManager));
		List<Level> worldsLevels = new List<Level> ((LevelsDataManager.GetInstance() as LevelsDataManager).getLevesOfWorld(currentWorld));

		MapLevel currentLevel = null;

		for (int i = 0; i < mapLevels.Count; i++) 
		{
			settingMapLevelInfo (mapLevels[i],worldsLevels[i]);

			settingMapLevelStatus (mapLevels[i]);

			setOnClickDelegates (mapLevels[i]);

			mapLevels [i].updateStatus();
			mapLevels[i].updateStars();
			mapLevels [i].updateText ();

			if(mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED || mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED ||  mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED )
			{
				currentLevel = mapLevels [i];
			}
		}

		paralaxManager.setPosByCurrentLevel (currentLevel);
	}

	protected void settingMapLevelInfo(MapLevel level,Level data)
	{
		level.lvlName = data.name;
		level.isBoss = data.isBoss;
		level.starsNeeded = data.starsNeeded;
		level.friendsNeeded = data.friendsNeeded;
		level.gemsNeeded = data.gemsNeeded;
	}

	protected void settingMapLevelStatus(MapLevel level)
	{
		if (level.isBoss) 
		{
			if ((LevelsDataManager.GetInstance () as LevelsDataManager).isLevelPassed (level.lvlName)) 
			{
				level.status = MapLevel.EMapLevelsStatus.BOSS_PASSED;
			} 
			else 
			{
				if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelReached (level.lvlName)) 
				{
					level.status = MapLevel.EMapLevelsStatus.BOSS_REACHED;
					Debug.Log ("boss reached");
					Debug.Log (!(LevelsDataManager.GetInstance () as LevelsDataManager).isLevelLocked (level.lvlName));

					if (!(LevelsDataManager.GetInstance () as LevelsDataManager).isLevelLocked (level.lvlName)) 
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_UNLOCKED;
					} 

				}
				else
				{
					level.status = MapLevel.EMapLevelsStatus.BOSS_LOCKED;
				} 
			}
		} 
		else 
		{
			if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelPassed (level.lvlName)) 
			{
				level.status = MapLevel.EMapLevelsStatus.NORMAL_PASSED;
			}
			else
			{
				if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelReached (level.lvlName))
				{
						level.status = MapLevel.EMapLevelsStatus.NORMAL_REACHED;
				}
				else
				{
					level.status = MapLevel.EMapLevelsStatus.NORMAL_LOCKED;
				}
			}
		}

		level.stars = MapLevel.EMapLevelStars.NONE;
		switch ((LevelsDataManager.GetInstance() as LevelsDataManager).getLevelStars (level.lvlName)) 
		{
		case(1):
			level.stars = MapLevel.EMapLevelStars.ONE;
			break;
		case(2):
			level.stars = MapLevel.EMapLevelStars.TWO;
			break;
		case(3):
			level.stars = MapLevel.EMapLevelStars.THREE;
			break;
		}
	}

	protected void setOnClickDelegates(MapLevel level)
	{
		switch (level.status) 
		{
		case(MapLevel.EMapLevelsStatus.BOSS_LOCKED):
		case(MapLevel.EMapLevelsStatus.NORMAL_LOCKED):
			level.OnClickNotification += OnLevelLockedPressed;
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_PASSED):
		case(MapLevel.EMapLevelsStatus.NORMAL_PASSED):
		case(MapLevel.EMapLevelsStatus.BOSS_UNLOCKED):
		case(MapLevel.EMapLevelsStatus.NORMAL_REACHED):
			level.OnClickNotification += OnLevelUnlockedPressed;
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_REACHED):
			level.OnClickNotification += OnBossReachedPressed;
			break;
		}
	}

	public void OnLifesPressed()
	{
		if (UserDataManager.instance.playerLifes == UserDataManager.instance.maximumLifes) 
		{
			openPopUp (fullLifes_PopUp);
		} 
		else if (UserDataManager.instance.playerLifes == 0) 
		{
			openPopUp (noLifes_PopUp);
		} 
		else 
		{
			openPopUp (missingLifes_PopUp);
		}

	}

	public void unlockBoss(string lvlName)
	{
		(LevelsDataManager.GetInstance () as LevelsDataManager).unlockLevel (lvlName);

		for (int i = 0; i < mapLevels.Count; i++) 
		{
			if (mapLevels [i].lvlName == lvlName) 
			{
				mapLevels [i].status = MapLevel.EMapLevelsStatus.BOSS_UNLOCKED;
				mapLevels [i].OnClickNotification -= OnBossReachedPressed;
				mapLevels [i].OnClickNotification += OnLevelUnlockedPressed;
			}
		}
	}

	protected void OnBossReachedPressed(MapLevel pressed)
	{
		if ((LevelsDataManager.GetInstance () as LevelsDataManager).getAllEarnedStars() >= pressed.starsNeeded) 
		{
			unlockBoss (pressed.lvlName);
		} 
		else 
		{
			bossLockedPopUp.lvlName = pressed.lvlName;

			bossLockedPopUp.initializeValues (pressed.friendsNeeded,pressed.gemsNeeded,pressed.starsNeeded);

			openPopUp ("bossLocked");
		}
	}

	protected void OnLevelUnlockedPressed(MapLevel pressed)
	{
		PersistentData.GetInstance ().setLevelNumber (int.Parse (pressed.lvlName));
		SceneManager.LoadScene ("Game");
	}

	protected void OnLevelLockedPressed(MapLevel pressed)
	{
		Debug.LogWarning ("NIVEL BLOQUEADO");
	}
}
