using UnityEngine;
using System.Collections;

public class PersistentData : MonoBehaviour 
{
	[HideInInspector]
	public int currentLevel;
	[HideInInspector]
	public Levels data;

	void Awake() 
	{
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels");
		data = Levels.LoadFromText(tempTxt.text);
	
	}
}