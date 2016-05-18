using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour 
{
	public const string fullLifes_PopUp 	= "FullLifes";
	public const string missingLifes_PopUp 	= "MissingLifes";
	public const string noLifes_PopUp 		= "NoLifes";

	public ScrollRect scrollRect;
	public GameObject modal;

	public Sprite normalLocked;
	public Sprite normalUnlocked;
	public Sprite bossLocked;
	public Sprite bossUnlocked;

	public Sprite noStar;
	public Sprite oneStar;
	public Sprite twoStars;
	public Sprite threeStars;

	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;

	protected List<MapLevel> mapLevels;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();

		popUpManager.OnPopUpCompleted = OnPopupCompleted;

		mapLevels = new List<MapLevel>(FindObjectsOfType<MapLevel> ());

		for (int i = 0; i < mapLevels.Count; i++) 
		{
			updateLevelIcon (mapLevels [i]);
			updateLevelStars (mapLevels[i]);
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
	}

	protected void updateLevelIcon(MapLevel level)
	{
		switch (level.status) 
		{
		case(MapLevel.EMapLevelsStatus.BOSS_LOCKED):
			changeSprite (level.levelIcon,bossLocked);
			break;
		case(MapLevel.EMapLevelsStatus.BOSS_UNLOCKED):
			changeSprite (level.levelIcon,bossUnlocked);
			break;
		case(MapLevel.EMapLevelsStatus.NORMAL_LOCKED):
			changeSprite (level.levelIcon,normalLocked);
			break;
		case(MapLevel.EMapLevelsStatus.NORMAL_UNLOCKED):
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

	protected void changeSprite(Image img,Sprite sprite)
	{
		img.sprite = Sprite.Create (sprite.texture, sprite.textureRect, new Vector2 (0.5f, 0.5f));
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
}
