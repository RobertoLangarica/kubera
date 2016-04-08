using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloatingTextPool : MonoBehaviour 
{
	public Text originalObject;

	protected List<Text> freeText = new List<Text>();
	protected List<Text> occupiedText = new List<Text>();

	// Use this for initialization
	void Start () 
	{
		originalObject.enabled = false;

		for(int i = 0;i < 3 ;i++)
		{
			addTextToThePool();
		}
	}

	//DONE: Cambie deactivateText por releaseText
	public Text getFreeText()
	{
		if(freeText.Count == 0)
		{
			addTextToThePool();
		}

		Text result = freeText[0];
		freeText.RemoveAt (0);

		occupiedText.Add (result);

		result.enabled = true;

		return result;
	}

	public void releaseText(Text text)
	{
		Text result = occupiedText[0];
		occupiedText.RemoveAt (0);

		freeText.Add (result);

		result.enabled = false;
	}

	protected void addTextToThePool()
	{
		GameObject go;
		go = GameObject.Instantiate(originalObject.gameObject) as GameObject;
		freeText.Add(go.GetComponent<Text>());
		go.transform.SetParent(transform,false);
	}
}
