using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIChar : MonoBehaviour 
{
	[HideInInspector]
	public ABCChar character;

	protected Text textfield;


	// Use this for initialization
	void Start () 
	{
		textfield = GetComponentInChildren<Text>();
		textfield.text = character.character;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
}
