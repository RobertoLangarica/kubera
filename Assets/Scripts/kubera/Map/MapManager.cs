using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;
using Kubera.Data.Sync;
using utils.gems;

public class MapManager : MonoBehaviour
{
	public const string fullLifes_PopUp 	= "FullLifes";
	public const string missingLifes_PopUp 	= "MissingLifes";
	public const string noLifes_PopUp 		= "NoLifes";

	public bool AllLevelsUnlocked = false;

	public ScrollRect scrollRect;
	public GameObject modal;
	public BossLocked bossLockedPopUp;

	[HideInInspector]public int currentWorld =-1;
	public Transform worldParent;
	public List<GameObject> worlds;
	protected GameObject WorldPrefab;

	protected List<MapLevel> mapLevels;
	protected bool fromGame;
	protected bool fromLoose;
	protected bool toStairs;
	protected bool toNextLevel;
	protected bool first;
	protected bool last;

	public bool isInLastLevelWorld;

	protected MapLevel currentLevel = null;
	protected MapLevel lastLevelPlayed = null;
	protected MapLevel nextLevel = null;

	protected string nameOfLastLevelPlayed;

	public List<string> test;

	public FBFriendsRequestPanel fbFriendsRequestPanel;
	public PopUpManager popUpManager;
	public ParalaxManager paralaxManager;
	public InvitationToReview invitationToReview;
	public FriendsOnWorldManager friendsOnWorldManager;
	public GoalManager		goalManager;
	public SettingsButton settingButtons;
	public GoalAfterGame goalAfterGame;
	public GoalPopUp goalPopUp;
	public WorldsPopUp worldsPopUp;

	protected bool cantPlay;

	void Start()
	{
		if(AllLevelsUnlocked)
		{
			Debug.Log("<color=red>Modo test: NIVELES DESBLOQUEADOS</color>");
		}

		//determinamos el mundo a cargar
		if(PersistentData.GetInstance().currentWorld == -1 || !PersistentData.GetInstance().fromGameToLevels)
		{
			if(DataManagerKubera.GetCastedInstance<DataManagerKubera>().currentUser.levels.Count != 0)
			{
				currentWorld = DataManagerKubera.GetCastedInstance<DataManagerKubera>().currentUser.maxWorldReached();

				int passedLevelsCount = DataManagerKubera.GetCastedInstance<DataManagerKubera> ().currentUser.countPassedLevelsByWorld(currentWorld);
				int levelsInWorld = PersistentData.GetInstance().levelsData.getLevelsByWorld(currentWorld).Length;

				PersistentData.GetInstance().maxWorldReached = currentWorld;

				if(passedLevelsCount == levelsInWorld)
				{
					int worldCount = PersistentData.GetInstance().levelsData.getWorldCount();

					if(currentWorld+1 <= worldCount)
					{
						currentWorld++;
					}
					PersistentData.GetInstance().maxWorldReached = currentWorld;
				}
			}
		}
		else
		{
			currentWorld = PersistentData.GetInstance ().currentWorld;
		}
			
		//Flujo entre game y levels
		if(PersistentData.GetInstance ().fromGameToLevels)
		{
			fromGame = true;
			fromLoose= PersistentData.GetInstance ().fromLoose;
			PersistentData.GetInstance ().fromGameToLevels = false;
			nameOfLastLevelPlayed = PersistentData.GetInstance ().lastLevelPlayedName;

			if(PersistentData.GetInstance().fromLoose)
			{
				toNextLevel = false;
			}
			else
			{
				toNextLevel = !PersistentData.GetInstance ().nextLevelIsReached;
			}
		}
		PersistentData.GetInstance ().fromLevelsToGame = true;

		popUpManager.OnPopUpCompleted = OnPopupCompleted;
		paralaxManager.OnFinish += showNextLevelGoalPopUp;
		invitationToReview.OnFinish += afterInvitation;
		settingButtons.OnActivateMusic += activateMusic;

		//Menu de acceso rapido
		initializeWorldsQuickMenuInfo ();

		//Cambiando al mundo adecuado
		changeWorld();

		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved += restartScene;
	}
		
	#if UNITY_EDITOR
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A))
		{
			Debug.Break();
		}
	}
	#endif

	protected void initializeWorldsQuickMenuInfo()
	{
		KuberaUser user = DataManagerKubera.GetCastedInstance<DataManagerKubera> ().currentUser;
		int worldCount = PersistentData.GetInstance().levelsData.getWorldCount();
		int maxWorldReached = PersistentData.GetInstance ().maxWorldReached;
		int starsObtained =0;
		List<LevelData> worldLevels;

		if(maxWorldReached == 0)
		{
			maxWorldReached++;
		}

		for(int i=0; i < worldCount; i++)
		{
			if(maxWorldReached > i || AllLevelsUnlocked)
			{
				worldLevels = user.getLevelsByWorld(i+1);

				for(int j=0; j<worldLevels.Count; j++)
				{
					starsObtained += worldLevels[j].stars;
				}
				worldsPopUp.initializeMiniWorld (i, true, starsObtained, PersistentData.GetInstance().levelsData.getLevelsCountByWorld(i+1) * 3);
				starsObtained = 0;
			}
			else
			{
				worldsPopUp.initializeMiniWorld (i, false, 0,0);
			}

		}
	}

	protected void changeWorld()
	{
		paralaxManager.enabled = false;
		if(WorldPrefab != null)
		{
			paralaxManager.Unsubscribe ();
			Destroy (WorldPrefab);
		}

		configureWorldOnScene (currentWorld);
		//getFriendsOnMap (currentWorld);
		initializeLevels ();
		setLevelsData ();
		setLastLevelReached ();
		Invoke ("setParalaxManager",0.06f);
		Invoke ("showWorld", 0.05f);

		paralaxManager.enabled = true;
		PersistentData.GetInstance ().currentWorld = currentWorld;

		if(fromGame)
		{
			Invoke ("onFinishLoad",0.1f);
		}
		else
		{
			Invoke ("onFinishLoad",0.7f);
		}
	}

	protected void configureWorldOnScene(int world)
	{
		currentWorld = world;

		InstantitateWorldByIndex (currentWorld-1);

		mapLevels = new List<MapLevel>(WorldPrefab.GetComponentsInChildren<MapLevel> ());
		paralaxManager.setRectTransform (WorldPrefab.GetComponent<RectTransform> ());
	}

	protected void InstantitateWorldByIndex(int worldIndex)
	{
		worlds [worldIndex].SetActive (false);
		WorldPrefab = (GameObject)Instantiate (worlds [worldIndex]);
		WorldPrefab.transform.SetParent (worldParent,false);
	}

	protected void getFriendsOnMap(int world)
	{
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			FriendsOnWorld friendsOnWorld = friendsOnWorldManager.existFriendsOnWorld (world.ToString ());

			if(friendsOnWorld == null)
			{
				//TODO info del server
				string[] facebokId = new string[test.Count];
				for(int i=0; i<test.Count; i++)
				{
					facebokId [i] = "10154899709081808";//UnityEngine.Random.Range (123, 1230123).ToString ();
				}
				friendsOnWorld = friendsOnWorldManager.getNewFriendsOnWorld (world.ToString(), test.ToArray (), facebokId);
			}
		}
	}

	protected void initializeLevels()
	{
		List<Level> worldsLevels = new List<Level> (PersistentData.GetInstance().levelsData.getLevelsByWorld(currentWorld));

		if(PersistentData.GetInstance().lastLevelReachedName == "")
		{
			setLastLevelReached ();
		}

		for (int i = 0; i < mapLevels.Count; i++)
		{
			settingMapLevelInfo (mapLevels[i],worldsLevels[i]);
			settingMapLevelStatus (mapLevels[i]);
			setOnClickDelegates (mapLevels[i]);

			mapLevels [i].updateStatus();
			mapLevels[i].updateStars();
			mapLevels [i].updateText ();
			mapLevels [i].setParalaxManager (paralaxManager);

			if(i != 0)
			{
				if (mapLevels [i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED
					|| mapLevels [i].status == MapLevel.EMapLevelsStatus.NORMAL_PASSED
					|| mapLevels [i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED
					|| mapLevels [i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED
					|| mapLevels [i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED) 
				{
					mapLevels [i - 1].nextLevelIsReached = true;
				}
				else
				{
					mapLevels [i - 1].nextLevelIsReached = false;
				}
			}
		}
	}

	protected void setLevelsData()
	{
		bool isConectedToFacebook = KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn;
		bool friendInfoInWorld = false;

		if (isConectedToFacebook) 
		{
			friendInfoInWorld = friendsOnWorldManager.existAnyFriendInWorld (currentWorld.ToString());
		}

		for(int i=0; i< mapLevels.Count; i++)
		{
			if(isConectedToFacebook && friendInfoInWorld)
			{
				FriendInfo friendInfo = isThereAnyFriendOnLevel (currentWorld, mapLevels [i].lvlName);

				if(friendInfo != null)
				{
					mapLevels [i].updateFacebookFriendPicture (friendInfo);
				}
				else
				{
					mapLevels [i].noFriend ();
				}
			}
			else
			{
				mapLevels [i].noFriend ();
			}

			if(mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_PASSED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED)
			{				
				currentLevel = mapLevels [i];
				PersistentData.GetInstance ().lastLevelReachedName = currentLevel.fullLvlName;
				if(fromGame && PersistentData.GetInstance().currentLevel.name == mapLevels[i].fullLvlName) 
				{
					lastLevelPlayed = mapLevels [i];

					if(i+1 != mapLevels.Count)
					{
						nextLevel = mapLevels [i+1];
					}
				}

				if(mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED && i+1 == mapLevels.Count)
				{
					if(currentWorld +1 <= worlds.Count)
					{
						toStairs = true;
					}
					toNextLevel = false;
				}
			}
		}

		if(PersistentData.GetInstance().maxWorldReached <= currentWorld)
		{
			if(toNextLevel)
			{
				lastLevelPlayed.myProgress (isConectedToFacebook);
			}
			else
			{
				if(toStairs && !PersistentData.GetInstance().stairsUnblocked)
				{
					isInLastLevelWorld = true;
					PersistentData.GetInstance ().stairsUnblocked = true;
					currentLevel.myProgress (isConectedToFacebook);
				}
				else if(!toStairs)
				{
					PersistentData.GetInstance ().stairsUnblocked = false;
					currentLevel.myProgress (isConectedToFacebook);
				}
			}
		}
	}

	protected void setLastLevelReached()
	{
		for(int i=mapLevels.Count-1 ; i>=0; i--)
		{
			if(mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_PASSED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED
				||  mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED)
			{	
				PersistentData.GetInstance ().lastLevelReachedName = mapLevels [i].lvlName;
				break;
			}
		}
	}

	protected void setParalaxManager()
	{
		if(currentLevel == null)
		{
			currentLevel = mapLevels [0];
			PersistentData.GetInstance ().lastLevelReachedName = currentLevel.fullLvlName;
		}

		if(first)
		{
			paralaxManager.setPosLastOrFirst(true);
			first = false;
		}
		else if(last)
		{
			paralaxManager.setPosLastOrFirst(false);
			last = false;
		}
		else if(fromGame)
		{			
			if(PersistentData.GetInstance().fromLoose)
			{
				popUpManager.activatePopUp ("retryPopUp");
				stopInput (true);

				(DataManagerKubera.GetInstance () as DataManagerKubera).incrementLevelAttemp (PersistentData.GetInstance ().currentLevel.name);

				if (LifesManager.GetInstance ().currentUser.playerLifes == 0) 
				{
					KuberaAnalytics.GetInstance ().registerLevelWhereReached0Lifes (PersistentData.GetInstance ().currentLevel.name);
				}
			}
			else
			{
				int starsReached = PersistentData.GetInstance ().lastPlayedLevelStars;//;(DataManagerKubera.GetInstance () as DataManagerKubera).getLevelStars (PersistentData.GetInstance ().currentLevel.name);
				int pointsMade = PersistentData.GetInstance ().lastPlayedLevelPoints;//(DataManagerKubera.GetInstance () as DataManagerKubera).getLevelPoints (PersistentData.GetInstance ().currentLevel.name);

				goalManager.initializeFromString(PersistentData.GetInstance().currentLevel.goal);

				string levelName = PersistentData.GetInstance ().currentLevel.name;
				for (int i = 0; i < levelName.Length; i++) 
				{
					if (levelName [i] == '0') 
					{
						levelName = levelName.Remove(i,1);
						i--;
					} 
					else 
					{
						break;
					}
				}


				goalAfterGame.setGoalPopUpInfo (starsReached,levelName , pointsMade.ToString(),PersistentData.GetInstance ().currentWorld);
				popUpManager.activatePopUp ("goalAfterGame");
				stopInput (true);
			}

			if(toStairs)
			{
				paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel(mapLevels[mapLevels.Count-1]));
			}
			else
			{
				paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel(lastLevelPlayed));
			}
		}
		else
		{
			//print ("currentLevel " + currentLevel);
			//paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel( mapLevels [0]));
			paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel(currentLevel));
		}
	}

	protected void settingMapLevelInfo(MapLevel level,Level data)
	{
		level.lvlName = data.name;
		level.fullLvlName = data.name;
		level.isBoss = data.isBoss;
		level.starsNeeded = data.starsNeeded;
		level.friendsNeeded = data.friendsNeeded;
		level.gemsNeeded = data.gemsNeeded;
	}

	protected void settingMapLevelStatus(MapLevel level)
	{
		DataManagerKubera DataManager = (DataManagerKubera.GetInstance() as DataManagerKubera);

		if (level.isBoss)
		{
			/*//facebook
			level.status = MapLevel.EMapLevelsStatus.BOSS_REACHED;
			return;*/

			if (DataManager.isLevelPassed (level.lvlName))
			{
				level.status = MapLevel.EMapLevelsStatus.BOSS_PASSED;
			}
			else
			{
				if (DataManager.isLevelReached (level.lvlName))
				{
					if (!DataManager.isLevelLocked (level.lvlName))
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_UNLOCKED;
					}
					else
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_REACHED;
					}
				}
				else
				{
					if(AllLevelsUnlocked)
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_REACHED;
					}
					else
					{
						level.status = MapLevel.EMapLevelsStatus.BOSS_LOCKED;	
					}
				}
			}
		}
		else
		{
			if (DataManager.isLevelPassed (level.lvlName))
			{
				level.status = MapLevel.EMapLevelsStatus.NORMAL_PASSED;
			}
			else
			{
				if (DataManager.isLevelReached (level.lvlName))
				{
						level.status = MapLevel.EMapLevelsStatus.NORMAL_REACHED;
				}
				else
				{
					if(AllLevelsUnlocked)
					{
						level.status = MapLevel.EMapLevelsStatus.NORMAL_REACHED;
					}
					else
					{
						level.status = MapLevel.EMapLevelsStatus.NORMAL_LOCKED;
					}
				}
			}
		}
			
		switch (DataManager.getLevelStars (level.lvlName))
		{
		case 1:
			level.stars = MapLevel.EMapLevelStars.ONE;
			break;
		case 2:
			level.stars = MapLevel.EMapLevelStars.TWO;
			break;
		case 3:
			level.stars = MapLevel.EMapLevelStars.THREE;
			break;
		default:
			level.stars = MapLevel.EMapLevelStars.NONE;
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
		KuberaUser currentUser = (DataManagerKubera.GetInstance () as DataManagerKubera).currentUser;
		
		if (currentUser.playerLifes == LifesManager.GetInstance().maximumLifes)
		{
			openPopUp (fullLifes_PopUp);
		}
		else if (currentUser.playerLifes == 0)
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
		(DataManagerKubera.GetInstance () as DataManagerKubera).unlockLevel (lvlName);

		//TODO Hacer animacion
		for (int i = 0; i < mapLevels.Count; i++)
		{
			if (mapLevels [i].fullLvlName == lvlName)
			{
				mapLevels [i].status = MapLevel.EMapLevelsStatus.BOSS_UNLOCKED;
				mapLevels [i].OnClickNotification -= OnBossReachedPressed;
				mapLevels [i].OnClickNotification += OnLevelUnlockedPressed;
			}
		}
	}

	protected void OnBossReachedPressed(MapLevel pressed)
	{
		KuberaAnalytics.GetInstance ().registerForBossReached (pressed.fullLvlName,(DataManagerKubera.GetInstance () as DataManagerKubera).getAllEarnedStars());
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).getAllEarnedStars() >= pressed.starsNeeded)
		{
			unlockBoss (pressed.fullLvlName);
			if(nextLevel != null)
			{
				OnLevelUnlockedPressed (nextLevel);
			}
			else
			{
				OnLevelUnlockedPressed (pressed);
			}
		}
		else
		{
			bossLockedPopUp.lvlName = pressed.lvlName;
			bossLockedPopUp.fullLvlName = pressed.fullLvlName;

			bossLockedPopUp.initializeValues (pressed.friendsNeeded,pressed.gemsNeeded,pressed.starsNeeded,pressed.lvlName);

			openPopUp ("bossLocked");
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

	protected void OnLevelUnlockedPressed(MapLevel pressed)
	{
		if(cantPlay)
		{
			return;
		}
		
		PersistentData.GetInstance ().setLevelNumber (int.Parse (pressed.lvlName));
		PersistentData.GetInstance ().lastLevelPlayedName = pressed.lvlName;
		PersistentData.GetInstance ().nextLevelIsReached = pressed.nextLevelIsReached;

		goalManager.initializeFromString(PersistentData.GetInstance().currentLevel.goal);
		int starsReached = (DataManagerKubera.GetInstance () as DataManagerKubera).getLevelStars (PersistentData.GetInstance ().currentLevel.name);
		
		setGoalPopUp(goalManager.currentCondition,goalManager.getGoalConditionParameters(),PersistentData.GetInstance().currentLevel.name,starsReached);

		//HACK temporal para probar el leaderboard
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().getLevelLeaderboard(PersistentData.GetInstance().currentLevel.name);

		//SceneManager.LoadScene ("Game");
	}

	protected void OnLevelLockedPressed(MapLevel pressed)
	{
		Debug.LogWarning ("NIVEL BLOQUEADO");
	}

	protected void activateStairs()
	{
		Stairs stairs = WorldPrefab.GetComponentInChildren<Stairs>();

		if(stairs && !isInLastLevelWorld)
		{
			stairs.mapManager = this;
			stairs.animateStairs ();
		}
	}

	protected void stairsToWait()
	{
		Stairs stairs = WorldPrefab.GetComponentInChildren<Stairs>();

		if(stairs && isInLastLevelWorld)
		{
			cantPlay = true;
			stairs.mapManager = this;
			stairs.animateToWait ();
		}
	}

	public void changeCurrentWorld(int world,bool isFirst, bool isLast)
	{
		toStairs = false;
		cantPlay = false;
		if(isFirst)
		{
			first = true;
		}
		else if(isLast)
		{
			last = true;
		}

		if(world != currentWorld)
		{
			currentWorld = world;
			isInLastLevelWorld = false;

			if(PersistentData.GetInstance().maxWorldReached < world)
			{
				PersistentData.GetInstance ().maxWorldReached = world;
			}

			changeWorld ();
		}
	}

	public void goToScene(string scene)
	{
		ScreenManager.GetInstance().GoToScene (scene);
	}

	protected void onFinishLoad()
	{
		if(fromGame)
		{
			ScreenManager.GetInstance().sceneFinishLoading(0);
		}
		else
		{
			ScreenManager.GetInstance().sceneFinishLoading();
		}
	}

	public void setGoalPopUp(string goalCondition, System.Object parameters,string levelName,int starsReached)
	{
		//Text goalText = goalPopUp.transform.FindChild("Objective").GetComponent<Text>();
		string textId = string.Empty;
		string textToReplace = string.Empty;
		string replacement = string.Empty;
		List<string> word = new List<string> ();
		List<string> letter = new List<string> ();

		string textA = string.Empty;
		string textB = string.Empty;
		int aABLetterObjectives = 0;

		for (int i = 0; i < levelName.Length; i++) 
		{
			if (levelName [i] == '0') 
			{
				levelName = levelName.Remove(i,1);
				i--;
			} 
			else 
			{
				break;
			}
		}

		switch(goalCondition)
		{
		case GoalManager.LETTERS:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_LETTERS_ID;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			textToReplace = "{{goalLetters}}";

			replacement = "";
			IEnumerable letters = parameters as IEnumerable;

			foreach (object oLetter in letters) 
			{
				letter.Add (oLetter.ToString ());
			}

			aABLetterObjectives = 2;
			break;
		case GoalManager.OBSTACLES:
			textToReplace = "{{blackLetters}}";
			replacement = (Convert.ToInt32(parameters)).ToString();

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_OBSTACLES_ID_A;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_OBSTACLES_ID_B;
			textB = MultiLanguageTextManager.instance.getTextByID (textId).Replace (textToReplace, replacement);

			aABLetterObjectives = 0;
			break;
		case GoalManager.POINTS:
			textToReplace = "{{goalPoints}}";
			replacement = (Convert.ToInt32 (parameters)).ToString ();

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_POINTS_ID_A;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_POINTS_ID_B;
			textB = MultiLanguageTextManager.instance.getTextByID (textId).Replace (textToReplace, replacement);
			aABLetterObjectives = 0;
			break;
		case GoalManager.WORDS_COUNT:
			textToReplace = "{{goalWords}}";
			replacement = (Convert.ToInt32(parameters)).ToString();

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_WORDS_ID_A;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_WORDS_ID_B;
			textB = MultiLanguageTextManager.instance.getTextByID (textId).Replace(textToReplace,replacement);
			aABLetterObjectives = 0;
			break;
		case GoalManager.SYNONYMOUS:
			textToReplace = "{{word}}";
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_SYNONYMOUS_ID_A;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			word= (List<string>)parameters;
			replacement = word[0];
			textB = replacement;

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_SYNONYMOUS_ID_B;
			textB = MultiLanguageTextManager.instance.getTextByID (textId).Replace(textToReplace,replacement);

			aABLetterObjectives = 0;
			break;
		case GoalManager.WORD:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_1_WORD_ID;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			word= (List<string>)parameters;
			replacement = word[0];
			textB = replacement;
			aABLetterObjectives = 0;
			break;		
		case GoalManager.ANTONYMS:
			textToReplace = "{{word}}";
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_ANTONYM_ID_A;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			word= (List<string>)parameters;
			replacement = word[0];
			textB = replacement;

			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_SYNONYMOUS_ID_B;
			textB = MultiLanguageTextManager.instance.getTextByID (textId).Replace(textToReplace,replacement);

			aABLetterObjectives = 0;
			break;
		}
			
		goalPopUp.setGoalPopUpInfo (textA,textB,starsReached, letter, levelName,aABLetterObjectives,currentWorld);
		popUpManager.activatePopUp ("goalPopUp");

		stopInput (true);

		//goalText.text = MultiLanguageTextManager.instance.getTextByID(textId).Replace(textToReplace,replacement);
		//activatePopUp("goalPopUp");
		//goalPopUp.SetActive(true);
	}
		
	protected void showNextLevelGoalPopUp ()
	{
		int level = int.Parse (nameOfLastLevelPlayed);

		if (toNextLevel)
		{
			if(!invitationToReview.isHappeningAReview (level))
			{
				if(nextLevel.isBoss)
				{
					OnBossReachedPressed (nextLevel);
				}
				else
				{
					OnLevelUnlockedPressed (nextLevel);
				}
			}
			else
			{
				invitationToReview.showInvitationProcessByLevelNumber(level);
			}

		}
		else
		{
			if(invitationToReview.isHappeningAReview (level))
			{
				invitationToReview.showInvitationProcessByLevelNumber(level);
			}
		}
	}

	protected void afterInvitation()
	{
		if (toNextLevel) 
		{
			OnLevelUnlockedPressed (nextLevel);
		}
	}

	protected void showWorld ()
	{
		worlds [currentWorld-1].SetActive (true);
		WorldPrefab.SetActive (true);

		if(toStairs)
		{
			Invoke ("activateStairs", 0.5f);
		}
	}

	protected void deActivateWorld ()
	{
		worlds [currentWorld-1].SetActive (false);
		WorldPrefab.SetActive (false);
	}

	protected FriendInfo isThereAnyFriendOnLevel(int world, string level)
	{
		return friendsOnWorldManager.getFriendOnLevel (world, level);
	}

	public MapLevel getCurrentLevel()
	{
		return currentLevel;
	}

	private void OnPopupCompleted(string action ="")
	{
		stopInput(false);
		switch (action) 
		{
		case "closeObjective":
			if(toNextLevel)
			{
				paralaxManager.setPosToNextLevel (nextLevel);
				lastLevelPlayed.moveProgress (nextLevel);

				toNextLevel = false;
			}
			else
			{
				stairsToWait ();
			}
			//showWorld();
		break;
		case "retry":
		case "playGame":
			stopInput(true);
			//TODO probablemente no haga falta mostrar el mundo
			//showWorld();
			ScreenManager.GetInstance().GoToScene ("Game");
		break;
		case "continue":
			if(toStairs)
			{
				//showWorld();
				paralaxManager.setPosLastOrFirst (false);
				stairsToWait ();
			}
			else if(toNextLevel)
			{
				//showWorld();
				paralaxManager.setPosToNextLevel (nextLevel);
				lastLevelPlayed.moveProgress (nextLevel);
			}
			else
			{
				toNextLevel = true;
				showNextLevelGoalPopUp ();
			}
		break;
		case "closeRetry":
			//showWorld();
		break;
		case "toWorldTraveler":
			openPopUp ("worldsPopUp");
		break;
		case "toFacebookMessages":
			openPopUp ("facebookNews");
		break;
		case "NoLifes":
			stopInput(true);
			openPopUp ("NoLifes");
		break;
		case "noLifesClose":
			if(popUpManager.isPopUpOpen("goalPopUp") || popUpManager.isPopUpOpen("retryPopUp"))
			{
				stopInput(false);
			}
			else
			{
				stopInput(true);
			}
		break;
		case "askKeys":
			stopInput(true);
			if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
			{
				openPopUp ("fbFriendsRequestPanel");
				fbFriendsRequestPanel.openFriendsRequestPanel (FBFriendsRequestPanel.ERequestType.ASK_KEYS);
			}
			else
			{
				popUpManager.activatePopUp ("fbConnectPopUp");
			}
		break;
		case "needLifes":
			foreach (PopUpBase val in popUpManager.popups) 
			{
				if (val.gameObject.activeSelf) 
				{
					val.deactivate ();
				}
			}

			stopInput(true);
			if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
			{
				openPopUp ("fbFriendsRequestPanel");
				fbFriendsRequestPanel.openFriendsRequestPanel (FBFriendsRequestPanel.ERequestType.ASK_LIFES);
			}
			else
			{
				popUpManager.activatePopUp ("fbConnectPopUp");
			}
		break;
		case "notClose":
			stopInput(true);
			break;
		default:
		break;
		}
	}

	protected void activateMusic(bool activate)
	{
		if(activate)
		{
			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance ().Stop ("gamePlay",false);

				if(!AudioManager.GetInstance().IsPlaying("menuMusic"))
				{
					AudioManager.GetInstance ().Play ("menuMusic");
				}
			}
		}
	}

	public void activateFacebook()
	{
		openPopUp ("facebookLoadingConnect");
	}

	public void activateShopika()
	{
		if (GemsManager.GetCastedInstance<GemsManager> ().currentUserId == GemsManager.GetCastedInstance<GemsManager> ().ANONYMOUS_USER) 
		{
			openPopUp ("shopikaConnect");
		}
	}

	protected void restartScene()
	{
		//SceneManager.LoadScene ("Levels");
		ScreenManager.GetInstance().GoToScene("Levels",true);
	}
}
