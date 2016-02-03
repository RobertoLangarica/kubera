using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WordManager : MonoBehaviour {

	public GameObject letterPrefab;
	public GameObject empty;
	public GameObject container;

	public string wildCardPointValue;

	[HideInInspector]public ABCDataStructure words;
	[HideInInspector]public List<ABCChar> chars;
	protected bool invalidCharlist;//Indica que la lista de caracteres tuvo o tiene uno invalido

	protected int padding = 300;
	protected int sortingAfterSwap;
	protected Vector2[] positionOfLetters; //los vectores de las letras 

	void Start()
	{
		chars = new List<ABCChar>();
		container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding;
		words = FindObjectOfType<ABCDataStructure>();
		//words.OnWordComplete += onWordComplete;
	}

	/**
	 * SOLO PARA TEST
	 * */
	public void addAndCloneCharacter(ABCChar character)
	{
		GameObject letter =  Instantiate(letterPrefab);
		ABCChar char2 = letter.AddComponent<ABCChar>();
		char2.value = character.value;
		char2.index = character.index;
		char2.used = character.used;
		char2.wildcard = character.wildcard;
		letter.GetComponent<UIChar>().character = char2; 
		addLetterToCorrectSpace(letter);
		letter.transform.localScale = new Vector3 (1, 1, 1);
		
		validateCharacter(char2);
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
			character.pointsValue = wildCardPointValue;
		}
		character.value = words.getCharValue(value);
		character.character = value.ToUpperInvariant();
		letter.GetComponent<UIChar>().character = character; 
		addLetterToCorrectSpace(letter);
		letter.transform.localScale = new Vector3 (1, 1, 1);
		letter.GetComponent<UIChar> ().piece = piece;
		
		validateCharacter(character);
		
		//para que las letras esten centradas HardCoding
		actualizePadding(true);
	}

	public void addCharacter(ABCChar pieceABCChar,GameObject piece)
	{
		GameObject letter =  Instantiate(letterPrefab);
		ABCChar character = letter.AddComponent<ABCChar>();

		character.wildcard = pieceABCChar.wildcard;
		character.value = words.getCharValue(pieceABCChar.character.ToUpper());
		character.character = pieceABCChar.character.ToUpperInvariant();
		character.pointsValue = pieceABCChar.pointsValue;
		character.typeOfLetter = pieceABCChar.typeOfLetter;
		letter.GetComponent<UIChar>().character = character; 

		addLetterToCorrectSpace(letter);
		letter.transform.localScale = new Vector3 (1, 1, 1);
		letter.GetComponent<UIChar> ().piece = piece;

		validateCharacter(character);

		//para que las letras esten centradas HardCoding
		actualizePadding(true);
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
				else
				{
					FindObjectOfType<ShowNext>().isWordCompleted(false);
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
			else
			{
				FindObjectOfType<ShowNext>().isWordCompleted(false);
			}
		}
	}


	/**
	 * Elimina los caracteres de la busqueda actual
	 **/ 
	public void resetValidation()
	{
		invalidCharlist = false;

		int l = container.transform.childCount;
		while(--l >= 0)
		{
			GameObject.Destroy(container.transform.GetChild(l).gameObject);
		}

		foreach(ABCChar c in chars)
		{
			c.empty = false;
		}

		chars.Clear();

		container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding = 300;
	}

	/**
	 * Cuando se completa una palabra
	 **/ 
	protected void onWordComplete()
	{
		Debug.Log("Se completo: "+getFullWord());
		FindObjectOfType<ShowNext>().isWordCompleted(true);
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
			actualizePadding(false);
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
			container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding = 300;
		}
		else if(lvlIndex != chars.Count)
		{
			//agregar hijo vacio para indicar el espacio
			GameObject letter =  Instantiate(empty);

			letter.transform.SetParent(container.transform);
			letter.transform.SetSiblingIndex(lvlIndex);
			letter.transform.localScale = new Vector3(1,1,1);
		}
	}

	public void deleteCharFromSearch(ABCChar abcChar)
	{
		deleteCharFromSearch(abcChar.index);
	}

	public void deleteCharFromSearch(UIChar uiChar)
	{
		deleteCharFromSearch(uiChar.character);
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
		foreach(ABCChar c in chars)
		{
			if(c.empty)
			{
				return true;
			}
		}
		return false;
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

	/**
	 * Revisa si es posible armar una palabra con los caracteres que se tienen en este momento
	 **/ 
	public void checkIfAWordisPossible()
	{
		checkIfAWordisPossible(chars);
	}

	/**
	 * Revisa si es posible armar una palabra con los caracteres que se tienen
	 **/
	public void checkIfAWordisPossible(List<ABCChar> pool)
	{
		//Debug.Log ("Possible word: "+words.isAWordPossible(pool));
		if(!words.isAWordPossible(pool))
		{
			print("perdio de verdad");
		}
	}

	/**
	 *Cambia el orden de las letras 
	 **/
	public void swappingLetters(GameObject letter)
	{
		for(int i=0; i<container.transform.childCount; i++)
		{			
			if(container.transform.GetChild(i).gameObject != letter)
			{
				if((int)container.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x > ((int)letter.GetComponent<RectTransform>().anchoredPosition.x - ((int)letter.GetComponent<RectTransform>().rect.width * 0.5f)) 
				   && (int)container.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x < ((int)letter.GetComponent<RectTransform>().anchoredPosition.x + ((int)letter.GetComponent<RectTransform>().rect.width * 0.5f)))
				{
					if(letter.GetComponent<RectTransform>().anchoredPosition.x > container.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
					{
						//izquierda a derecha
						sortingAfterSwap = i;

						for(int j=container.transform.childCount-2; j>=i; j--)
						{
							//print(container.transform.GetChild(j).GetComponent<ABCChar>().character + "       "+j);
							//print(position[j+1]);
							container.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = positionOfLetters[j+1];
						}
					}
					else
					{
						//derecha a izquierda 
						sortingAfterSwap = i+1;

						for(int j =0; j<=i; j++)
						{
							//print(container.transform.GetChild(j).GetComponent<ABCChar>().character);
							//print(position[j]);
							container.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = positionOfLetters[j];
						}
					}
					break;
				}
			}
		}
	}


	
	/*
	 * activa o desactiva el poder mover las letras
	 * a la letra que se movera se mueve su index para que este arriba de las otras letras
	 */
	public void canSwappLetters(bool deActivate,GameObject letter)
	{
		if(deActivate)
		{
			container.GetComponent<HorizontalLayoutGroup>().enabled = false;
			sortingAfterSwap = letter.transform.GetSiblingIndex();
			letter.transform.SetSiblingIndex(100);
		}
		else
		{
			container.GetComponent<HorizontalLayoutGroup>().enabled = true;
			letter.transform.SetSiblingIndex(sortingAfterSwap);
			chars.Clear();
			for(int i=0; i<container.transform.childCount; i++)
			{
				//print(container.transform.GetChild(i).GetComponent<ABCChar>().character);
				chars.Add(container.transform.GetChild(i).GetComponent<ABCChar>());
			}
			words.initCharByCharValidation();
			foreach(ABCChar c in chars)
			{
				words.validateChar(c);
			}
			if(words.completeWord)
			{
				onWordComplete();
			}
			else
			{
				FindObjectOfType<ShowNext>().isWordCompleted(false);
			}
		}
	}

	/*
	 * llena el arreglo de vectores de las posiciones de las letras para poder moverlas
	 */
	public void setPositionToLetters()
	{
		positionOfLetters = new Vector2[container.transform.childCount];
		for(int i=0; i<container.transform.childCount; i++)
		{
			positionOfLetters[i] = container.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
		}
	}

	/**
	 * actualiza el padding de la estructura de letras mostradas para que se vean centradas
	 **/
	protected void actualizePadding(bool adding)
	{
		if(adding)
		{
			if(container.GetComponent<HorizontalLayoutGroup>().padding.left > 0)
			{
				container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding = padding-50;
			}
		}
		else
		{
			if(container.GetComponent<HorizontalLayoutGroup>().padding.left < 300)
			{
				container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding = padding+50;
			}
		}
	}
}
