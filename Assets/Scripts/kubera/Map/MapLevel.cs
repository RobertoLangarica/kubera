using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	public Image levelIcon;
	public Image levelStars;

	public string lvlName;

	public bool isBoss;

	public int friendsNeeded;
	public int gemsNeeded;
	public int starsNeeded;

	public delegate void DOnClickNotification(MapLevel pressed);
	public DOnClickNotification OnClickNotification;

	public void updateText()
	{
		lvlName = lvlName.Replace ("0","");
		lvlNameText.text = lvlName;
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
