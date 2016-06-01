using UnityEngine;
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

	public Sprite normalLocked;
	public Sprite normalUnlocked;
	public Sprite bossLocked;
	public Sprite bossUnlocked;
	public Sprite bossReached;

	public Sprite noStar;
	public Sprite oneStar;
	public Sprite twoStars;
	public Sprite threeStars;

	public int currentWorld;
	public List<GameObject> worlds;

	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;

	protected List<MapLevel> mapLevels;

	public FBFriendsRequestPanel fbFriendsRequestPanel;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();

		popUpManager.OnPopUpCompleted = OnPopupCompleted;
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
		Debug.Log (worldsLevels.Count + "----------");
		
		for (int i = 0; i < mapLevels.Count; i++) 
		{
			Debug.Log ("setLEvel");
			settingMapLevelInfo (mapLevels[i],worldsLevels[i]);

			Debug.Log ("setStatus");
			settingMapLevelStatus (mapLevels[i]);

			Debug.Log ("SetClicks");
			setOnClickDelegates (mapLevels[i]);

			Debug.Log ("updateIOmg");
			updateLevelIcon (mapLevels [i]);
			updateLevelStars (mapLevels[i]);
		}
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
			if ((LevelsDataManager.GetInstance () as LevelsDataManager).isLevelPassed (level.name)) 
			{
				level.status = MapLevel.EMapLevelsStatus.BOSS_PASSED;
			} 
			else 
			{
				if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelLocked (level.name)) 
				{
					level.status = MapLevel.EMapLevelsStatus.BOSS_LOCKED;
				}
				else 
				{
					if ((LevelsDataManager.GetInstance () as LevelsDataManager).isLevelReached (level.name)) 
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_REACHED;
					} 
					else 
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_UNLOCKED;
					}
				}
			}

		} 
		else 
		{
			if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelPassed (level.name)) 
			{
				level.status = MapLevel.EMapLevelsStatus.NORMAL_PASSED;
			}
			else
			{
				if ((LevelsDataManager.GetInstance() as LevelsDataManager).isLevelReached (level.name)) 
				{
					level.status = MapLevel.EMapLevelsStatus.NORMAL_UNLOCKED;
				}
				else
				{
					level.status = MapLevel.EMapLevelsStatus.NORMAL_LOCKED;
				}
			}
		}

		level.stars = MapLevel.EMapLevelStars.NONE;
		switch ((LevelsDataManager.GetInstance() as LevelsDataManager).getLevelStars (level.name)) 
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

	protected void updateLevelIcon(MapLevel level)
	{
		switch (level.status) 
		{
		case(MapLevel.EMapLevelsStatus.BOSS_LOCKED):
			changeSprite (level.levelIcon,bossLocked);
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_REACHED):
			changeSprite (level.levelIcon,bossUnlocked);
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_PASSED):
		case(MapLevel.EMapLevelsStatus.BOSS_UNLOCKED):
			changeSprite (level.levelIcon,bossUnlocked);
			break;
		case(MapLevel.EMapLevelsStatus.NORMAL_LOCKED):
			changeSprite (level.levelIcon,normalLocked);
			break;
		case(MapLevel.EMapLevelsStatus.NORMAL_UNLOCKED):
		case(MapLevel.EMapLevelsStatus.NORMAL_PASSED):
			changeSprite (level.levelIcon,normalUnlocked);
			break;
		}
	}

	protected void updateLevelStars(MapLevel level)
	{
		switch(level.stars)
		{
		case(MapLevel.EMapLevelStars.NONE):
			changeSprite (level.levelStars,noStar);
			break;
		case(MapLevel.EMapLevelStars.ONE):
			changeSprite (level.levelStars,oneStar);
			break;
		case(MapLevel.EMapLevelStars.TWO):
			changeSprite (level.levelStars,twoStars);
			break;
		case(MapLevel.EMapLevelStars.THREE):
			changeSprite (level.levelStars,threeStars);
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
		case(MapLevel.EMapLevelsStatus.NORMAL_UNLOCKED):
			level.OnClickNotification += OnLevelUnlockedPressed;
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_REACHED):
			level.OnClickNotification += OnBossReachedPressed;
			break;
		}
	}

	protected void changeSprite(Image img,Sprite sprite)
	{
		img.sprite = Sprite.Create (sprite.texture, sprite.textureRect, new Vector2 (0.5f, 0.5f));
	}

	public void OnLifesPressed()
	{
		if (UserDataManager.instance.playerLifes == UserDataManager.instance.maximumLifes) {
			openPopUp (fullLifes_PopUp);
		} else if (UserDataManager.instance.playerLifes == 0) {
			openPopUp (noLifes_PopUp);
		} else {
			openPopUp (missingLifes_PopUp);
		}

	}

	public void unlockBoss(string lvlName)
	{
		(LevelsDataManager.GetInstance () as LevelsDataManager).unlockLevel (lvlName);
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
			bossLockedPopUp.starsNeeded = pressed.starsNeeded;
			bossLockedPopUp.stars = (LevelsDataManager.GetInstance () as LevelsDataManager).getAllEarnedStars ();
			bossLockedPopUp.gemsNeeded = pressed.gemsNeeded;

			popUpManager.activatePopUp ("bossLocked");
		}
	}

	protected void OnLevelUnlockedPressed(MapLevel pressed)
	{
		PersistentData.instance.currentLevel = PersistentData.instance.levelsData.getLevelByName (pressed.lvlName);
		SceneManager.LoadScene ("Game");
	}

	protected void OnLevelLockedPressed(MapLevel pressed)
	{
		Debug.LogWarning ("NIVEL BLOQUEADO");
	}
}
