using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapLevel : MonoBehaviour 
{
	public enum EMapLevelsStatus
	{
		NORMAL_LOCKED,
		NORMAL_UNLOCKED,
		BOSS_LOCKED,
		BOSS_UNLOCKED
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

	public int friendsNeeded = 1;
	public int gemsNeeded = 10;
	public int starsNeeded = 100;

	public delegate void DOnTryOpenBlockedBoss(int friendsNeeded,int gemsNeeded,int starsNeeded);
	public DOnTryOpenBlockedBoss OnOpen;

	void Start()
	{
		lvlName = name.Substring (2).Remove (name.Substring (2).Length-1);
		//lvlNameText.text = lvlName;
	}

	public void onClick()
	{
		print (status);
		switch (status) {
		case EMapLevelsStatus.BOSS_LOCKED:
			if(false)
			{
				OnOpen (friendsNeeded, gemsNeeded, starsNeeded);
			}
			else
			{
				//TODO: animacion de desbloqueo
			}
			break;
		case EMapLevelsStatus.BOSS_UNLOCKED:
			break;
		case EMapLevelsStatus.NORMAL_LOCKED:
			break;
		case EMapLevelsStatus.NORMAL_UNLOCKED:
			//Go to lvl
			print (lvlNameText.text);
			PersistentData.instance.setLevelNumber(int.Parse(lvlNameText.text),true);

			ScreenManager.instance.GoToScene("Game");
			break;
		default:
			break;
		}

	}

	public void initializeFromLevel(Level info)
	{
		info.friendsNeeded = friendsNeeded;
		info.gemsNeeded = gemsNeeded;
		info.starsNeeded = starsNeeded;
	}
}
