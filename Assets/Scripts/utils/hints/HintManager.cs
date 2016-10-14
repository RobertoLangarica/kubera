using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintManager : MonoBehaviour {

	public string[] hints;
	public Text txt;

	void Start () 
	{
		changeText();
	}
	
	public void changeText()
	{
		txt.text = hints[Random.Range(0,hints.Length)];
	}
}
