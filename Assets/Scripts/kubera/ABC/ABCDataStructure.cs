using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABCDataStructure : MonoBehaviour {
	
	public class IntList
	{
		public int value;//Valor de caracter que representa este nodo
		public bool end;//Indica si este valor es el final de una palabra
		public IntList parent = null;//Nodo padre
		public List<IntList> content;//nodos hijos

		//Para validacion
		public bool wildcard = false;//Indica si en la validacion se usa como comodin
		public int wildcardIndex; //Representa el valor que se le da en la busqueda al comodin
		public int lastCorrectValue;//Ultimo valor correcto en modo comodin

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
		IntList currentList = null;

		//Buscamos si ya existe el character
		IntList tmp = data;
		while(tmp != null && currChar < limit)
		{
			currChar++;
			if(currChar >= limit)
			{
				break;
			}
			currentList = tmp;
			tmp = currentList.content.Find(item => item.value == ABCDataStructure.getCharValue(chars[currChar]));

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
				tmp.parent = currentList;
				currentList.content.Add(tmp);
				currentList = tmp;
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
	protected IntList currentValidationList;
	protected List<IntList> levelsOfSearch;
	protected int levelsCount;
	public void initCharByCharValidation()
	{
		//Valido por default
		isValid = true;
		//La raiz de las palabras
		currentValidationList = data;

		//Sin niveles de busqueda
		levelsCount = 0;
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

		//Aumentamos el conteo de niveles de busqueda
		levelsCount++;

		if(isValid)
		{
			if(c.wildcard)
			{
				if(currentValidationList.content.Count > 0)
				{
					//Armamos el comodin tomando el primer valor como el correcto
					IntList wildcard = currentValidationList.content[0];
					wildcard.wildcardIndex = 0;//Indicamos cual indice tenemos ahorita
					wildcard.wildcard = true;
					currentValidationList = wildcard;

					//Correcto sin validacion ya que es comodin
					return true;
				}
				else
				{
					//Se estan agregando caracteres de mas
					//checamos si moviendo comodines
					return checkBackwardsForWildcardOptions(c);
				}
			}
			else
			{
				//Buscamos en el nivel actual si existe el caracter que se pide
				tmp = currentValidationList.content.Find(item => item.value == c.value);

				if(tmp != null)
				{
					//Si existe!!
					//Guardamos el nuevo nivel de busqueda
					currentValidationList = tmp;
					levelsOfSearch.Add(tmp);
					return true;
				}
				else
				{
					//No existe!!
					//checamos si moviendo comodines
					return checkBackwardsForWildcardOptions(c);
				}
			}


		}

		//Invalido por default
		return false;
	}

	/**
	 * Elimina un nivel de busqueda, si se elimina un nivel que no es el ultimo
	 * entonces se eliminaran todos los niveles para no fragmentar la lista,
	 * la recuperacion de los niveles se debe de manejar afuera donde se controlan los
	 * caracteres
	 * */
	public void deleteLvlOfSearch(int indexToDelete)
	{
		//Se elimina un indice mayos a los validados
		if(indexToDelete >= levelsOfSearch.Count)
		{
			//Disminuimos la cuenta de los niveles
			levelsCount--;

			//La lista es valida de nuevo?
			if(levelsCount <= levelsOfSearch.Count)
			{
				isValid = true;
			}
		}
		else 
		{
			//Eliminamos todos los niveles sobrantes
			for(int i = levelsOfSearch.Count-1; i >= indexToDelete; i--)
			{
				//Disminuimos la cuenta de los niveles
				levelsCount--;
				//Quitamos cualquier comodin
				levelsOfSearch[i].wildcard = false;
				levelsOfSearch.RemoveAt(i);
			}

			//Hacemos reset de currentValidationList para continuar la busqueda de la manera correcta
			if(levelsCount > 0)
			{
				//Si el nivel que quedo es comodin lo reseteamos al estado default
				if(levelsOfSearch[levelsCount-1].wildcard)
				{
					levelsOfSearch[levelsCount-1].wildcard = false;

					//Armamos el comodin tomando el primer valor como el correcto
					IntList wildcard = levelsOfSearch[levelsCount-1].parent.content[0];
					wildcard.wildcardIndex = 0;//Indicamos cual indice tenemos ahorita
					wildcard.wildcard = true;
					levelsOfSearch[levelsCount-1] = wildcard;
				}

				currentValidationList = levelsOfSearch[levelsCount-1];
			}
			else
			{
				//Queda como en estado inicial
				initCharByCharValidation();
			}
		}

	}

	/**
	 * Hace un chequeo de los niveles de busqueda hacia atras exclusivamente
	 * para mover los comodines hacia un estado donde se admita charToValidate.
	 * 
	 * Si no existe ningun estado valido entonces devuelve todos los comodines al ultimo estado
	 * activo.
	 * 
	 * 
	 * @return true si el caracter es valido. false si el caracter es invalido
	 * */
	protected bool checkBackwardsForWildcardOptions(ABCChar charToValidate)
	{
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
