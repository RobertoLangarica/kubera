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

	void Start()
	{
		lvlName = name.Substring (2).Remove (name.Substring (2).Length-1);
		//lvlNameText.text = lvlName;
	}

	public void onClick()
	{
		//Go to lvl
		print (lvlNameText.text);
		PersistentData.instance.setLevelNumber(int.Parse(lvlNameText.text),true);

		ScreenManager.instance.GoToScene("Game");
	}

	public void initializeFromLevel(Level info)
	{
		
	}
}
