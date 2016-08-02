using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
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
	public Transform worldParent;
	public List<GameObject> worlds;
	protected GameObject WorldPrefab;

	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;
	protected ParalaxManager paralaxManager;
	protected DoorsManager doorsManager;
	protected InvitationToReview invitationToReview;
	private GoalManager		goalManager;

	protected List<MapLevel> mapLevels;
	protected bool fromGame;
	protected bool toDoor;
	protected bool toNextLevel;

	protected MapLevel currentLevel = null;
	protected MapLevel lastLevelPlayed = null;
	protected MapLevel nextLevel = null;

	public FBFriendsRequestPanel fbFriendsRequestPanel;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();
		paralaxManager = FindObjectOfType<ParalaxManager> ();
		doorsManager = FindObjectOfType<DoorsManager> ();
		goalManager = FindObjectOfType<GoalManager> ();
		invitationToReview = FindObjectOfType<InvitationToReview> ();

		popUpManager.OnPopUpCompleted = OnPopupCompleted;
		if(PersistentData.GetInstance().currentWorld == -1)
		{
			print (PersistentData.GetInstance ().currentLevel.name);
			if(LevelsDataManager.GetCastedInstance<LevelsDataManager>().currentUserLevels.levels.Count != 0)
			{
				currentWorld = PersistentData.GetInstance().currentWorld = (PersistentData.GetInstance().levelsData.levels[LevelsDataManager.GetCastedInstance<LevelsDataManager>().currentUserLevels.levels.Count].world);
				//print (currentWorld);
			}
		}
		else
		{
			currentWorld = PersistentData.GetInstance ().currentWorld;
		}

		if(PersistentData.GetInstance ().fromGameToLevels)
		{
			fromGame = true;
			PersistentData.GetInstance ().fromGameToLevels = false;
		}

		//selectLevel (currentWorld);

		//initializeLevels ();
		//print (currentWorld);
		changeWorld ();

		paralaxManager.OnFinish += showNextLevelGoalPopUp;
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
		case "closeObjective":
			if(toNextLevel)
			{
				paralaxManager.setPosToNextLevel (nextLevel);
			}
			break;
		case "playGame":
			SceneManager.LoadScene ("Game");
			break;
		case "continue":
			if(toNextLevel)
			{
				paralaxManager.setPosToNextLevel (nextLevel);
			}
			else
			{
				showNextLevelGoalPopUp ();
			}
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

			if(mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.NORMAL_PASSED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_UNLOCKED
				||  mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_REACHED
				|| mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED)
			{				
				currentLevel = mapLevels [i];
				
				if(fromGame && PersistentData.GetInstance().currentLevel.name == mapLevels[i].fullLvlName) 
				{
					lastLevelPlayed = mapLevels [i];
					if(i+1 <mapLevels.Count)
					{
						nextLevel = mapLevels [i+1];
						toNextLevel = true;
					}
				}
				else if(nextLevel != null && nextLevel.fullLvlName == mapLevels[i].fullLvlName)
				{
					print (fromGame);
					print (nextLevel.name);
					print (PersistentData.GetInstance().lastLevelReachedName);
					if(fromGame && nextLevel.fullLvlName == PersistentData.GetInstance().lastLevelReachedName)
					{
						toNextLevel = false;
					}
				}

				if(mapLevels[i].status == MapLevel.EMapLevelsStatus.BOSS_PASSED && i+1 == mapLevels.Count)
				{
					toDoor = true;
					doorsManager.DoorsCanOpen ();
				}
			}
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
		//level.status = MapLevel.EMapLevelsStatus.NORMAL_REACHED;
		//return;

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
		PersistentData.GetInstance ().lastLevelPlayedName = pressed.lvlName;

		goalManager.initializeFromString(PersistentData.GetInstance().currentLevel.goal);
		int starsReached = (LevelsDataManager.GetInstance () as LevelsDataManager).getLevelStars (PersistentData.GetInstance ().currentLevel.name);
		
		setGoalPopUp(goalManager.currentCondition,goalManager.getGoalConditionParameters(),PersistentData.GetInstance().currentLevel.name,starsReached);

		//SceneManager.LoadScene ("Game");
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
		setLastLevelReached ();
		setParalaxManager ();
		paralaxManager.enabled = true;

		PersistentData.GetInstance ().currentWorld = currentWorld;
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
				print (mapLevels [i].status);
				PersistentData.GetInstance ().lastLevelReachedName = mapLevels [i].fullLvlName;
				break;
			}
		}
	}

	protected void setParalaxManager()
	{
		if(currentLevel == null)
		{
			currentLevel = mapLevels [0];
		}

		if(toDoor)
		{
			paralaxManager.setPosToDoor ();
		}
		else if(fromGame)
		{			
			if(PersistentData.GetInstance().fromLoose)
			{
				popUpManager.activatePopUp ("retryPopUp");
				stopInput (true);
			}
			else
			{
				paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel(lastLevelPlayed));
				goalManager.initializeFromString(PersistentData.GetInstance().currentLevel.goal);
				int starsReached = (LevelsDataManager.GetInstance () as LevelsDataManager).getLevelStars (PersistentData.GetInstance ().currentLevel.name);
				int pointsMade = (LevelsDataManager.GetInstance () as LevelsDataManager).getLevelPoints (PersistentData.GetInstance ().currentLevel.name);

				popUpManager.getPopupByName ("goalAfterGame").GetComponent<GoalAfterGame>().setGoalPopUpInfo (starsReached, PersistentData.GetInstance ().currentLevel.name, pointsMade.ToString());
				popUpManager.activatePopUp ("goalAfterGame");
				stopInput (true);
			}
		}
		else
		{
			paralaxManager.setPosByCurrentLevel (paralaxManager.getPosByLevel(currentLevel));
		}
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
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_OBSTACLES_ID;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			/*textToReplace = "{{goalObstacleLetters}}";
			replacement = (Convert.ToInt32(parameters)).ToString();*/
			aABLetterObjectives = 1;
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
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_SYNONYMOUS_ID;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			word= (List<string>)parameters;
			replacement = word[0];
			textB = replacement;
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
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_ANTONYM_ID;
			textA = MultiLanguageTextManager.instance.getTextByID (textId);
			word= (List<string>)parameters;
			replacement = word[0];
			textB = replacement;
			aABLetterObjectives = 0;
			break;
		}
			
		popUpManager.getPopupByName ("goalPopUp").GetComponent<GoalPopUp>().setGoalPopUpInfo (textA,textB,starsReached, letter, levelName,aABLetterObjectives);
		popUpManager.activatePopUp ("goalPopUp");

		stopInput (true);

		//goalText.text = MultiLanguageTextManager.instance.getTextByID(textId).Replace(textToReplace,replacement);
		//activatePopUp("goalPopUp");
		//goalPopUp.SetActive(true);
	}
		
	protected void showNextLevelGoalPopUp ()
	{
		print ("asadas"+PersistentData.GetInstance().lastLevelReachedName);
		OnLevelUnlockedPressed (nextLevel);
	}
}
