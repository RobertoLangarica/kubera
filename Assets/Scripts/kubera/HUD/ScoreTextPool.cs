using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreTextPool : MonoBehaviour 
{
	public Text originalObject;

	protected int current;
	protected List<Text> pool = new List<Text>();

	// Use this for initialization
	void Start () 
	{
		originalObject.enabled = false;
		current = -1;

		for(int i = 0;i < 3 ;i++)
		{
			addTextToThePool();
		}
	}

	/*TODO: Hay que tener congruencia en los terminos, se usa deactivateText
	 * y se piden textos libres getFreeText en lugar de textos desactivados
	 * Que se unifique a liberados o desocupados
	*/
	public Text getFreeText()
	{
		current++;

		//TODO: Hay que manejar 2 listas una deactivos y una de inactivos (es lo mas comun en una pool)
		if(current == pool.Count)
		{
			if(pool[0].text != "Unoccupied")
			{
				addTextToThePool();
			}
			else
			{
				current = 0;
			}
		}

		Text result = pool[current];

		result.enabled = true;

		return result;
	}

	public void deativateText(Text text)
	{
		//TODO: integrar el manejo de las pools
		text.text = "Unoccupied";
		text.enabled = false;
	}

	protected void addTextToThePool()
	{
		GameObject go;
		go = GameObject.Instantiate(originalObject.gameObject) as GameObject;
		pool.Add(go.GetComponent<Text>());
		go.transform.SetParent(transform,false);
	}
}
