using UnityEngine;
using System.Collections;
using Parse;

public class ParseManager : MonoBehaviour 
{
	void Start()
	{
		ParseObject testObject = new ParseObject("TestObject");
		testObject["foo"] = "REMOTE TEst";
		testObject.SaveAsync();

	}
}
