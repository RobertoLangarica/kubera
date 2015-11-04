using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	[HideInInspector]
	public Levels data;

	// Use this for initialization
	void Start () 
	{
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels");
		data = Levels.LoadFromText(tempTxt.text);

		Debug.Log(data.levels[0].pool);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
