using UnityEngine;
using System.Collections;

public class ABCChar : MonoBehaviour 
{
	public string character = "A";
	public bool wildcard = false;

	protected int value;
	
	void Start () 
	{
		if(!wildcard)
		{
			character = character.ToUpperInvariant();
			value = ABCDataStructure.getCharValue(character.ToCharArray()[0]);
		}
	}
}
