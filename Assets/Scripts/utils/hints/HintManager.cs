using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintManager : MonoBehaviour {

	public string[] hints;
	public Text txt;

	// Use this for initialization
	void Start () 
	{
		changeText();
	}
	
	public void changeText()
	{
		txt.text = hints[Random.Range(0,hints.Length-1)];
	}
}
