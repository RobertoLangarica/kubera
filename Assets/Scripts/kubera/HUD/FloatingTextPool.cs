using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloatingTextPool : MonoBehaviour 
{
	public FloatingTextForPool originalObject;

	protected List<FloatingTextForPool> freeText = new List<FloatingTextForPool>();
	protected List<FloatingTextForPool> occupiedText = new List<FloatingTextForPool>();

	// Use this for initialization
	void Start () 
	{
		originalObject.myText.enabled = false;

		for(int i = 0;i < 3 ;i++)
		{
			addTextToThePool();
		}
	}

	//DONE: Cambie deactivateText por releaseText
	public FloatingTextForPool getFreeText()
	{
		//Por si no hay
		if(freeText.Count == 0)
		{
			addTextToThePool();
		}

		FloatingTextForPool result = freeText[0];
		freeText.RemoveAt (0);

		occupiedText.Add (result);

		result.myText.enabled = true;

		//Si se acabaron estiramos la pool
		if(freeText.Count == 0)
		{
			addTextToThePool();
		}

		return result;
	}

	public void releaseText()
	{
		FloatingTextForPool result = occupiedText[0];
		occupiedText.RemoveAt (0);

		freeText.Add (result);

		result.myText.enabled = false;
	}

	protected void addTextToThePool()
	{
		FloatingTextForPool go;
		go = GameObject.Instantiate<FloatingTextForPool>(originalObject);
		go.setPool(this);
		freeText.Add(go);
		go.gameObject.transform.SetParent(transform,false);
	}
}
