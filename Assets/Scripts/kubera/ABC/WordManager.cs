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

	/**
	 * Instancia un UIChar que recibe como referencia a character y manda validarlo
	 * */
	public void addCharacter(ABCChar character)
	{
		GameObject letter =  Instantiate(letterPrefab);
		letter.GetComponent<UIChar>().character = character; 
		letter.transform.SetParent(container.transform);
		letter.transform.localScale = new Vector3 (1, 1, 1);

		validateCharacter(character);

	}

	/**
	 * Instancia UICHar y con la cadena que recibe crea un ABCChar que se
	 * agrega como componente a la nueva pieza
	 * */
	public void addCharacter(string value,GameObject piece)
	{
		GameObject letter =  Instantiate(letterPrefab);
		ABCChar character = letter.AddComponent<ABCChar>();
		value = value.ToUpper ();
		if (value == ".")
		{
			character.wildcard = true;
		}
		character.value = ABCDataStructure.getCharValue(value);
		character.character = value.ToUpperInvariant();
		letter.GetComponent<UIChar>().character = character; 
		letter.transform.SetParent(container.transform);
		letter.transform.localScale = new Vector3 (1, 1, 1);
		letter.GetComponent<Tile> ().piece = piece;

		validateCharacter(character);
	}

	/**
	 * Valida el caracter para ver si ya se completo una palabra
	 **/ 
	protected void validateCharacter(ABCChar character)
	{
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
			Tile t = container.transform.GetChild(l).gameObject.GetComponent<Tile>();

			if(t != null && t.piece != null)
			{
				if(words.completeWord)
				{
					if(t)
					{
						t.piece.GetComponent<Letter>().cellIndex.clearCell();
						GameObject.Destroy(t.piece);
					}
				}
				else
				{
					t.piece.GetComponent<Letter>().backToNormal();
				}
			}

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
