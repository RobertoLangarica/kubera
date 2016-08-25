using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;

public class MapManager : MonoBehaviour
{
	public const string fullLifes_PopUp 	= "FullLifes";
	public const string missingLifes_PopUp 	= "MissingLifes";
	public const string noLifes_PopUp 		= "NoLifes";

	public ScrollRect scrollRect;
	public GameObject modal;
	public BossLocked bossLockedPopUp;

	public int currentWorld;
	public Transform worldParent;
	public List<GameObject> worlds;
	protected GameObject WorldPrefab;

	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;
	protected ParalaxManager paralaxManager;
	protected DoorsManager doorsManager;

	protected List<MapLevel> mapLevels;

	public FBFriendsRequestPanel fbFriendsRequestPanel;

	//HACK CampusParty
	public GameObject[] lifes;
	public GameObject popUpNoLifes;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();
		paralaxManager = FindObjectOfType<ParalaxManager> ();
		doorsManager = FindObjectOfType<DoorsManager> ();

		popUpManager.OnPopUpCompleted = OnPopupCompleted;
		print (PersistentData.GetInstance().currentWorld);
		if(PersistentData.GetInstance().currentWorld == -1)
		{
			if(LevelsDataManager.GetCastedInstance<LevelsDataManager>().currentUser.levels.Count != 0)
			{
				currentWorld = PersistentData.GetInstance().currentWorld = (PersistentData.GetInstance().levelsData.levels[LevelsDataManager.GetCastedInstance<LevelsDataManager>().currentUser.levels.Count].world);
				print (currentWorld);
			}
		}
		else
		{
			currentWorld = PersistentData.GetInstance ().currentWorld;
		}
		//selectLevel (currentWorld);

		//initializeLevels ();
		//print (currentWorld);
		changeWorld ();

		//HACK CampusParty
		for(int i=0; i<	PersistentData.GetInstance().lifes; i++)
		{
			lifes [i].SetActive (true);
		}
		if(	PersistentData.GetInstance().lifes == 0)
		{
			popUpNoLifes.SetActive (true);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A))
		{
			changeWorld ();
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

		setWorldOnScene (currentWorld-1);

		mapLevels = new List<MapLevel>(WorldPrefab.GetComponentsInChildren<MapLevel> ());
		paralaxManager.setRectTransform (WorldPrefab.GetComponent<RectTransform> ());
	}

	protected void setWorldOnScene(int world)
	{
		worlds [world].SetActive (true);
		WorldPrefab = (GameObject)Instantiate (worlds [world]);
		WorldPrefab.transform.SetParent (worldParent,false);

		doorsManager = FindObjectOfType<DoorsManager> ();
	}

	protected void initializeLevels()
	{
		Debug.Log ((LevelsDataManager.GetInstance() as LevelsDataManager));
		List<Level> worldsLevels = new List<Level> ((LevelsDataManager.GetInstance() as LevelsDataManager).getLevelsOfWorld(currentWorld));


		MapLevel currentLevel = null;
		
		for (int i = 0; i < mapLevels.Count; i++)
		{
			settingMapLevelInfo (mapLevels[i],worldsLevels[i]);

			settingMapLevelStatus (mapLevels[i]);

			setOnClickDelegates (mapLevels[i]);

			mapLevels [i].updateStatus();
			mapLevels[i].updateStars();
			mapLevels [i].updateText ();

			if(mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_PASSED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED
				||  mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED)
			{
				currentLevel = mapLevels [i];

				if(mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED && i+1 == mapLevels.Count)
				{
					doorsManager.DoorsCanOpen ();
					paralaxManager.setPosToDoor ();
				}
			}
		}
		if(currentLevel == null)
		{
			currentLevel = mapLevels [0];
		}
		print (currentLevel);
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
		case(MapLevel.EMapLevelsStatus.BOSS_REACHED):
		case(MapLevel.EMapLevelsStatus.NORMAL_REACHED):
			level.OnClickNotification += OnLevelUnlockedPressed;
			break;
		/*case(MapLevel.EMapLevelsStatus.BOSS_REACHED):
			level.OnClickNotification += OnBossReachedPressed;
			break;*/
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

	protected void changeWorld()
	{
		paralaxManager.enabled = false;
		if(WorldPrefab != null)
		{
			paralaxManager.Unsubscribe ();
			Destroy (WorldPrefab);
		}

		selectLevel (currentWorld);

		initializeLevels ();
		paralaxManager.enabled = true;
	}

	public void changeCurrentWorld(int world)
	{
		currentWorld = world;

		changeWorld ();
	}

	public void goToScene(string scene)
	{
		ScreenManager.instance.GoToScene (scene);
	}
}
