using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WordManager : MonoBehaviour {

	public GameObject letterPrefab;
	public GameObject container;
	public ABCDataStructure words;


	protected List<ABCChar> chars;

	void Start()
	{
		chars = new List<ABCChar>();
		//words.OnWordComplete += onWordComplete;
	}

	public void addCharacter(ABCChar character)
	{
		GameObject letter =  Instantiate(letterPrefab);
		letter.GetComponent<UIChar>().character = character; 
		letter.transform.SetParent(container.transform);

		if(chars.Count == 0)
		{
			words.initCharByCharValidation();
		}

		chars.Add(character);
		words.validateChar(character);

		if(words.completeWord)
		{
			onWordComplete();
		}
	}


	/**
	 * Elimina los caracteres de la busqueda actual
	 **/ 
	public void resetValidation()
	{
		chars.Clear();

		int l = container.transform.childCount;
		while(--l >= 0)
		{
			GameObject.Destroy(container.transform.GetChild(l).gameObject);
		}
	}

	/**
	 * Cuando se completa una palabra
	 **/ 
	protected void onWordComplete()
	{
		Debug.Log("Se completo: "+getFullWord());
	}

	/**
	 * Devuelve la palabra que se esta validando en ese momento
	 * */
	public string getFullWord()
	{
		string result = "";

		foreach(ABCChar c in chars)
		{
			result = result+c.character;
		}

		return result;
	}
}
