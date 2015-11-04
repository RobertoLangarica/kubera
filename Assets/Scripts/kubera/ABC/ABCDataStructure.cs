using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABCDataStructure : MonoBehaviour {
	
	public class IntList
	{
		public int value;
		public bool end;
		public IntList wildcar;
		public int wildcarIndex; //Representa el valor que se le da en la busqueda al comodin
		public List<IntList> content;
	};

	//Inicio de la estructura
	[HideInInspector]
	public IntList data;

	public int wordCount = 0;

	// Use this for initialization
	void Start () 
	{
		if(data == null)
		{
			data = new IntList();
			data.content = new List<IntList>();
		}

		registerNewWord("ola");
		registerNewWord("hola");
		registerNewWord("hola");
		registerNewWord("holas");
		registerNewWord("salas");
		registerNewWord("sala");
		registerNewWord("ala");
		registerNewWord("sal");
		registerNewWord("sola");
		registerNewWord("sol");
	}
	
	public void registerNewWord(string word)
	{
		//Debug.Log ("Adding: "+word);

		//Dividimos la palabra en chars
		char[] chars = word.ToUpperInvariant().ToCharArray();

		int currChar = -1;
		int limit = chars.Length;
		List<IntList> currentList = null;

		//Buscamos si ya existe el character
		IntList tmp = data;
		while(tmp != null && currChar < limit)
		{
			currChar++;
			if(currChar >= limit)
			{
				break;
			}
			currentList = tmp.content;
			tmp = currentList.Find(item => item.value == ABCDataStructure.getCharValue(chars[currChar]));

			//Una nueva subpalabra
			if(tmp != null && currChar == limit-1 && !tmp.end)
			{
				//Debug.Log("Subpalabra:"+word);
				tmp.end = true;
				return;
			}
		}

		//¿Hay que agregar mas caracteres?
		if(currChar < limit)
		{
			//Agregamos el character y los subsecuentes
			for(;currChar < limit; currChar++)
			{
				tmp = new IntList();
				tmp.value = ABCDataStructure.getCharValue(chars[currChar]);
				tmp.end = currChar == limit-1;
				//Debug.Log(chars[currChar]+"_"+tmp.value);
				tmp.content = new List<IntList>();
				currentList.Add(tmp);
				currentList = tmp.content;
			}

			wordCount++;
			return;
		}
		else
		{
			//Debug.Log("Ya existia previamente la palabra: "+word);
		}
	}

	public bool isValidWord(string word)
	{
		//Dividimos la palabra en chars
		char[] chars = word.ToUpperInvariant().ToCharArray();
		
		int currChar = -1;
		int limit = chars.Length;
		List<IntList> currentList = null;
		
		//Buscamos si existe cada caracter
		IntList tmp = data;
		while(tmp != null && currChar < limit)
		{
			currChar++;
			if(currChar >= limit)
			{
				break;
			}
			currentList = tmp.content;
			tmp = currentList.Find(item => item.value == ABCDataStructure.getCharValue(chars[currChar]));

			//Subpalabra?
			if(tmp != null && currChar == limit-1 && !tmp.end)
			{
				return false;
			}
		}

		return !(currChar < limit);
	}
	
	protected bool isValid;
	protected List<IntList> currentValidationList;
	protected List<IntList> levelsOfSearch;
	public void initCharByCharValidation()
	{
		//Valido por default
		isValid = true;
		//La raiz de las palabras
		currentValidationList = data.content;

		if(levelsOfSearch == null)
		{
			levelsOfSearch = new List<IntList>();
		}
		else
		{
			levelsOfSearch.Clear();
		}
	}

	/**
	 * Devuelve true si el caracter es parte de una palabra
	 **/ 
	public bool validateChar(ABCChar c)
	{
		IntList tmp = null;

		if(isValid)
		{
			if(c.wildcard)
			{
				//Armamos
				IntList wildList = new IntList();

			}
			else
			{
				tmp = currentValidationList.Find(item => item.value == c.value);
			}

			//Guardamos el nuevo nivel de busqueda
			if(tmp != null)
			{
				levelsOfSearch.Add(tmp);
				return true;
			}
		}

		//Invalido por default
		return false;
	}

	protected bool hasValueInChildren(IntList List, int searchValue)
	{
		IntList tmp;
		tmp = currentValidationList.Find(item => item.value == searchValue);

		if(tmp != null)
		{
			return true;
		}

		return false;
	}

	public static int getCharValue(char c)
	{
		switch(c)
		{
		case 'A':
			return 0;
		case 'B':
			return 1;
		case 'C':
			return 2;
		case 'D':
			return 3;
		case 'E':
			return 4;
		case 'F':
			return 5;
		case 'G':
			return 6;
		case 'H':
			return 7;
		case 'I':
			return 8;
		case 'J':
			return 9;
		case 'K':
			return 10;
		case 'L':
			return 11;
		case 'M':
			return 12;
		case 'N':
			return 13;
		case 'Ñ':
			return 14;
		case 'O':
			return 15;
		case 'P':
			return 16;
		case 'Q':
			return 17;
		case 'R':
			return 18;
		case 'S':
			return 19;
		case 'T':
			return 20;
		case 'U':
			return 21;
		case 'V':
			return 22;
		case 'W':
			return 23;
		case 'X':
			return 24;
		case 'Y':
			return 25;
		case 'Z':
			return 26;

		}
		return -1;
	}
}
