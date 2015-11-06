using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WordManager : MonoBehaviour {

	public GameObject letterPrefab;
	public GameObject container;
	public ABCDataStructure words;


	[HideInInspector]
	public List<ABCChar> chars;
	protected bool invalidCharlist;//Indica que la lista de caracteres tuvo o tiene uno invalido

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
		addLetterToCorrectSpace(letter);
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
		addLetterToCorrectSpace(letter);
		letter.transform.localScale = new Vector3 (1, 1, 1);
		letter.GetComponent<Tile> ().piece = piece;

		validateCharacter(character);
	}

	/**
	 * Agrega la siguiente letra tomando en cuenta los espacios vacios
	 **/ 
	protected void addLetterToCorrectSpace(GameObject letter)
	{
		//Agregamos la letra al primer lugar vacio
		for(int i = 0; i < chars.Count; i++)
		{
			if(chars[i].empty)
			{
				DestroyImmediate(container.transform.GetChild(i).gameObject);
				letter.transform.SetParent(container.transform);
				letter.transform.SetSiblingIndex(i);
				return;
			}
		}

		//Agregamos la letra al ultimo
		letter.transform.SetParent(container.transform);
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

		if(invalidCharlist)
		{
			int index = getFirstEmptyIndex();

			//Sustituimos el character
			chars[index].empty = false;
			chars[index] = character;
			character.index = index;

			if(!hasEmptyChars())
			{
				//Se completaron los vacios y los validamos
				words.initCharByCharValidation();

				foreach(ABCChar c in chars)
				{
					words.validateChar(c);
				}

				invalidCharlist = false;

				if(words.completeWord)
				{
					onWordComplete();
				}
			}
		}
		else
		{
			character.index = chars.Count;
			chars.Add(character);
			words.validateChar(character);
			
			if(words.completeWord)
			{
				onWordComplete();
			}
		}
	}


	/**
	 * Elimina los caracteres de la busqueda actual
	 **/ 
	public void resetValidation()
	{
		invalidCharlist = false;

		foreach(ABCChar c in chars)
		{
			c.empty = false;
		}

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

	/**
	 * elimina el caracter indicado de la busqueda
	 * */
	public void deleteCharFromSearch(int lvlIndex)
	{
		if(lvlIndex == chars.Count-1)
		{
			//El usuairo elimino el ultimo caracter
			words.deleteLvlOfSearch(lvlIndex);
			chars.RemoveAt(lvlIndex);
		}
		else
		{
			words.cleanCharByCharValidation();
			chars[lvlIndex].empty = true;
			invalidCharlist = true;
		}

		//Eliminamos la letra anterior
		DestroyImmediate(container.transform.GetChild(lvlIndex).gameObject);

		
		if(everythingIsEmpty())
		{
			resetValidation();
		}
		else if(lvlIndex != chars.Count)
		{
			//agregar hijo vacio para indicar el espacio
			GameObject letter =  Instantiate(letterPrefab);
			letter.transform.SetParent(container.transform);
			letter.transform.SetSiblingIndex(lvlIndex);
		}
	}

	/**
	 * Indica si todos los caracteres en la busqueda los elimino 
	 * el usuario y se marcaron como empty = true
	 * */
	protected bool everythingIsEmpty()
	{
		foreach(ABCChar c in chars)
		{
			if(!c.empty)
			{
				return false;
			}
		}

		return true;
	}

	/**
	 * indica si la lista tiene caracteres vacios
	 **/ 
	protected bool hasEmptyChars()
	{	
		return !everythingIsEmpty();
	}
	/**
	 * Obtiene el indice del primer caracter vacio dentro de la lista
	 **/ 
	protected int getFirstEmptyIndex()
	{
		for(int i = 0; i < chars.Count; i++)
		{
			if(chars[i].empty)
			{
				return i;
			}
		}

		return -1;
	}
}
