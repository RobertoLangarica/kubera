using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ABCChar : MonoBehaviour 
{
	public int value;
	public bool wildcard = false;

	[HideInInspector]
	public string character = "A";

	
	void Start () 
	{
		if(!wildcard)
		{
			character = ABCDataStructure.getStringByValue(value);
		}
		else
		{
			character = "";
		}

		Text txt = GetComponentInChildren<Text>();

		if(txt != null)
		{
			txt.text = character;
		}
	}
}
