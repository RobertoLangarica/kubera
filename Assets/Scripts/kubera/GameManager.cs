using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	[HideInInspector]
	public Levels data;

	protected int currLevel;

	// Use this for initialization
	void Awake () 
	{
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels");
		data = Levels.LoadFromText(tempTxt.text);
		currLevel = 0;
		Debug.Log(data.levels[0].pool);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
