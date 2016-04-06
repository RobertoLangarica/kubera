using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class lvl : MonoBehaviour {

	public Text lvlNameText;
	protected string lvlName;

	void Start()
	{
		lvlName = name.Substring (2).Remove (name.Substring (2).Length-1);
		lvlNameText.text = lvlName;
	}

	public void onClick()
	{
		//Go to lvl
		print (lvlNameText.text);
		PersistentData.instance.setLevelNumber(int.Parse(lvlNameText.text),true);

		ScreenManager.instance.GoToScene("Game");
	}
}
