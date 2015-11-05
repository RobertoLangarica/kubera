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

	//public delegate void DOnWordComplete();
	//public DOnWordComplete OnWordComplete;
	
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

		//OnWordComplete += foo;
	}

	protected void foo(){}
	
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
	
	public bool isValid = true;
	public bool completeWord = false;
	protected IntList currentValidationList;
	protected List<IntList> levelsOfSearch;
	protected int levelsCount;
	public void initCharByCharValidation()
	{
		//Valido por default
		isValid = true;
		completeWord = false;

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
					//Armamos el comodin
					IntList wildcard = new IntList();
					wildcard.wildcard = true;
					wildcard.wildcardIndex = 0;
					//El mismo contenido que el nodo anterior 
					//(ya que al ser comodin referencia todas las listas)
					wildcard.content = currentValidationList.content;
					currentValidationList = wildcard;

					//Correcto sin validacion ya que es comodin
					isValid = true;
				}
				else
				{
					//Se estan agregando caracteres de mas
					//checamos si moviendo comodines
					isValid = checkBackwardsForWildcardOptions(c);
				}
			}
			else
			{
				//Buscamos en el nivel actual si existe el caracter que se pide
				if(currentValidationList.wildcard)
				{
					//Si es comodin busca en la lista que el comodin apunta en este momento
					tmp = currentValidationList.content[currentValidationList.wildcardIndex].content.Find(item => item.value == c.value);
				}
				else
				{
					//Busqueda directa en el nodo
					tmp = currentValidationList.content.Find(item => item.value == c.value);
				}


				if(tmp != null)
				{
					//Si existe!!
					//Guardamos el nuevo nivel de busqueda
					currentValidationList = tmp;
					levelsOfSearch.Add(tmp);
					isValid = true;
				}
				else
				{
					//No existe!!
					//checamos si moviendo comodines
					isValid = checkBackwardsForWildcardOptions(c);
				}
			}

			//Checamos si ya se completo una palabra
			if(isValid)
			{
				completeWord = currentValidationList.end;
			}
			else
			{
				completeWord = false;
			}

			return isValid;
		}

		//Invalido por default
		isValid = false;

		return isValid;
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
					levelsOfSearch[levelsCount-1].wildcardIndex = 0;
				}

				currentValidationList = levelsOfSearch[levelsCount-1];
			}
			else
			{
				//Queda como en estado inicial
				initCharByCharValidation();
			}
		}

		//Checamos si ya se completo una palabra
		if(isValid)
		{
			completeWord = currentValidationList.end;
		}
		else
		{
			completeWord = false;
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
		int i = levelsOfSearch.Count;
		IntList tmp;
		List<IntList> copy = new List<IntList>(levelsOfSearch);

		while(--i >= 0)
		{
			//Buscamos un comodin
			if(levelsOfSearch[i].wildcard)
			{
				int l2 = levelsOfSearch[i].content.Count;
				//Guardamos el estado actual
				levelsOfSearch[i].lastCorrectValue = levelsOfSearch[i].wildcardIndex;
				List<IntList> partialChain = levelsOfSearch.GetRange(i+1,levelsOfSearch.Count-i+1);
				List<IntList> partialResult;

				for(int j = levelsOfSearch[i].wildcardIndex+1; j < l2; j++)
				{
					//Este nuevo comodin es valido?
					levelsOfSearch[i].wildcardIndex = j;
					partialResult = getCorrectChain(levelsOfSearch[i].content[j],partialChain);
					
					//Se encontro una cadena valida
					if(partialResult != null)
					{
						//Contiene el caracter a validar?
						tmp = partialResult[partialResult.Count-1].content.Find(item => item.value == charToValidate.value);
						
						if(tmp != null)
						{
							//LISTO tenemos el valor como debe ser
							levelsOfSearch.Add(tmp);
							currentValidationList = tmp;
							return true;
						}
					}
				}
			}
		}

		//Fue invalido hay que devolver el estado original los comodines
		i = levelsOfSearch.Count;
		while(--i >= 0)
		{
			//Buscamos un comodin
			if(copy[i].wildcard)
			{
				if(i == 0)
				{
					copy[i].wildcardIndex = copy[i].lastCorrectValue;
					copy[i].content = data.content;
				}
				else
				{
					copy[i].wildcardIndex = copy[i].lastCorrectValue;
					copy[i].content = copy[i-1].content;
				}
			}
		}

		return false;
	}


	/**
	 * Busca dentro de target la cadena de nodos que contenga todos los valores dentro de value
	 * y devuelve dicha cadena, si no existe una cadena valida devuelve nulo
	 * */
	protected List<IntList> getCorrectChain(IntList target, List<IntList> values)
	{
		List<IntList> result = new List<IntList>();

		int limit = values.Count;
		IntList tmp;
		for(int i = 0; i < limit; i++)
		{
			if(values[i].wildcard)
			{
				//Guardamos el estado del comodin
				values[i].lastCorrectValue = values[i].wildcardIndex;
				
				if(i == 0)
				{
					//Se inicia como comodin apuntando a target
					values[i].content = target.content;
				}
				else
				{
					//Comodin apuntando al valor anterior de result
					values[i].content = result[i-1].content;
				}

				//Ya se termino la validacion o continua debajo de este comodin?
				if(i == limit-1)
				{
					//No hay nadie mas que explorar este comodin es el ultimo
					result.Add(values[i]);
				}
				else
				{
					//Buscamos la cadena de valores correcta debajo de este comodin
					int l2 = values[i].content.Count;
					List<IntList> partialChain = values.GetRange(i+1,limit-i+1);
					List<IntList> partialResult;
					for(int j = 0; j < l2; j++)
					{
						//Si es el correcto que se quede con el valor correcto
						values[i].wildcardIndex = j;
						partialResult = getCorrectChain(values[i].content[j],partialChain);

						//Se encontro una cadena valida
						if(partialResult != null)
						{
							//Completamos la cadena y la devolvemos
							result.AddRange(partialResult);
							return result;
						}
					}

					//Ya se iteraron todas las opciones del comodin y no se encontro
					result.Clear();
					return null;
				}
			}
			else
			{
				tmp = target.content.Find(item => item.value == values[i].value);
				
				if(tmp != null)
				{
					result.Add(tmp);
				}
				else
				{
					//No hay una cadena valida
					return null;
				}
			}
		}

		return null;
	}


	protected bool hasValueInChildren(IntList lst, int searchValue)
	{
		IntList tmp;
		tmp = lst.content.Find(item => item.value == searchValue);

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

	public static string getStringByValue(int value)
	{
		switch(value)
		{
		case 0:
			return "A";
		case 1:
			return "B";
		case 2:
			return "C";
		case 3:
			return "D";
		case 4:
			return "E";
		case 5:
			return "F";
		case 6:
			return "G";
		case 7:
			return "H";
		case 8:
			return "I";
		case 9:
			return "J";
		case 10:
			return "K";
		case 11:
			return "L";
		case 12:
			return "M";
		case 13:
			return "N";
		case 14:
			return "Ñ";
		case 15:
			return "O";
		case 16:
			return "P";
		case 17:
			return "Q";
		case 18:
			return "R";
		case 19:
			return "S";
		case 20:
			return "T";
		case 21:
			return "U";
		case 22:
			return "V";
		case 23:
			return "W";
		case 24:
			return "X";
		case 25:
			return "Y";
		case 26:
			return "Z";
			
		}
		return "";
	}
}
