using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ABC
{
	public class ABCDataStructure : MonoBehaviour {


		//Clase para los nodos de la estructura
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

		//Alfabeto que contiene todos los caracteres y su representacion en entero
		private List<AlfabetUnit> alfabet;

		//Inicio de la estructura
		[HideInInspector]public IntList data;

		//public delegate void DOnWordComplete();
		//public DOnWordComplete OnWordComplete;
		
		[HideInInspector]public int wordCount = 0;//Conteo de palabras presentes en el diccionario


		/*****PARA LA BUSQUEDA CARACTER POR CARACTER*****************/
		[HideInInspector]public bool isValid = true;
		[HideInInspector]public bool completeWord = false;//Esta en true solo cuando la validacion por caracter
		protected IntList currentValidationList;
		protected List<IntList> levelsOfSearch;
		protected int levelsCount;
		/************************************************************/
		
		void Start () 
		{
			//Puede no ser nulo si se modifica desde un EditorScript
			if(data == null)
			{
				data = new IntList();
				data.content = new List<IntList>();
			}
		}

		/**
		 * Registra una nueva palabra en el diccionario
		 * si la palabra existe previamente ya no se agrega 
		 * y si existe parcialmente solo se agregan las partes que no existian previamente
		 **/ 
		public void registerNewWord(string word)
		{
			//Dividimos la palabra en chars y la forzamos a mayusculas
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
				tmp = currentList.content.Find(item => item.value == getCharValue(chars[currChar]));

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
					tmp.value = getCharValue(chars[currChar]);
					tmp.end = currChar == limit-1;
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


		/**
		 * Recibe una cadena entera e indica si dicha cadena se encuentra dentro del diccionario
		 * */
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
				tmp = currentList.Find(item => item.value == getCharValue(chars[currChar]));

				//Subpalabra?
				if(tmp != null && currChar == limit-1 && !tmp.end)
				{
					return false;
				}
			}

			return !(currChar < limit);
		}

		/**
		 * Inicializa el chequeo caracter por caracter de una palabra,
		 * para ir validando cada caracter se usa validateChar
		 * */
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
		 * Limpia y deja en estado sin busqueda
		 * */ 
		public void cleanCharByCharValidation()
		{
			isValid = true;
			completeWord = false;
			levelsOfSearch.Clear();
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
					int cant = currentValidationList.wildcard ?
						currentValidationList.content[currentValidationList.wildcardIndex].content.Count 
							: currentValidationList.content.Count;


					if(cant > 0)
					{
						//Armamos el comodin
						IntList wildcard = new IntList();
						wildcard.wildcard = true;
						wildcard.wildcardIndex = 0;
						//El mismo contenido que el nodo anterior 
						//(ya que al ser comodin referencia todas las listas)
						if(currentValidationList.wildcard)
						{
							wildcard.content = currentValidationList.content[currentValidationList.wildcardIndex].content;
						}
						else
						{
							wildcard.content = currentValidationList.content;
						}

						//Correcto sin validacion ya que es comodin
						currentValidationList = wildcard;
						levelsOfSearch.Add(wildcard);
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
				checkAndUpdateForCompleteWord();

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
			//Se elimina un indice mayor a los validados
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
			checkAndUpdateForCompleteWord();
		}

		public void checkAndUpdateForCompleteWord()
		{
			if(isValid)
			{
				if(currentValidationList.wildcard)
				{
					completeWord = currentValidationList.content[currentValidationList.wildcardIndex].end;
				}
				else
				{
					completeWord = currentValidationList.end;
				}
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
			IntList tmp = null;
			List<IntList> copy = new List<IntList>(levelsOfSearch);

			while(--i >= 0)
			{
				//Buscamos un comodin
				if(levelsOfSearch[i].wildcard)
				{
					int l2 = levelsOfSearch[i].content.Count;
					//Guardamos el estado actual
					levelsOfSearch[i].lastCorrectValue = levelsOfSearch[i].wildcardIndex;
					List<IntList> partialChain = levelsOfSearch.GetRange(i+1,levelsOfSearch.Count-(i+1));
					List<IntList> partialResult;

					for(int j = levelsOfSearch[i].wildcardIndex+1; j < l2; j++)
					{
						//Este nuevo comodin es valido?
						levelsOfSearch[i].wildcardIndex = j;

						if(partialChain.Count > 0)
						{
							partialResult = getCorrectChain(levelsOfSearch[i].content[j],partialChain);
							
							//Se encontro una cadena valida
							if(partialResult != null)
							{
								//Contiene el caracter a validar?
								if(partialResult[partialResult.Count-1].wildcard)
								{
									tmp = partialResult[partialResult.Count-1].content[partialResult[partialResult.Count-1].wildcardIndex].content.Find(item => item.value == charToValidate.value);
								}
								else
								{
									tmp = partialResult[partialResult.Count-1].content.Find(item => item.value == charToValidate.value);
								}
							}
						}
						else
						{
							//No hay nadie debajo y lo validamos directamente a este nodo
							tmp = levelsOfSearch[i].content[levelsOfSearch[i].wildcardIndex].content.Find(item => item.value == charToValidate.value);

						}

						//Existe charToValidate al final de la cadena
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
					values[i].wildcardIndex = 0;

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
						List<IntList> partialChain = values.GetRange(i+1,limit-(i+1));
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

		/**
		 * Indica si el objeto IntList tiene el valor searchValue en alguno de sus hijos
		 * */
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


		protected List<ABCChar> sortedChars;//Caracteres ordenados con los comodines al final
		protected List<ABCChar> used = new List<ABCChar>();//Vamos guardando los que ya se usaron para la palabra
		/**
		 * Recibe un grupo de caracteres y determina si es posible armar una palabra con ellos
		 **/ 
		public bool isAWordPossible(List<ABCChar> chars)
		{
			Dictionary<int,bool> validated = new Dictionary<int, bool>();//La letra con la que ya se inicio una busqueda
			IntList tmp;
			int i,l;

			if(sortedChars != null)
			{
				sortedChars.Clear();
			}
			else
			{
				sortedChars = new List<ABCChar>();
			}

			//Sort con los comodines hasta el ultimo (para que primero se agoten las letras)
			for(int x = 0; x < chars.Count; x++)
			{
				if(chars[x].wildcard)
				{
					sortedChars.Add(chars[x]);
				}
				else
				{
					sortedChars.Insert(0,chars[x]);
				}
			}

			//Limpiamos la lista e usados
			used.Clear();


			l = sortedChars.Count;
			i = 0;

			for(; i < l; i++)
			{
				//Solo si este caracter no se a checado previamente
				if(!validated.ContainsKey(sortedChars[i].value))
				{
					validated.Add(sortedChars[i].value,true);

					//Alguna palabra inicia con este caracter
					tmp = data.content.Find(item => item.value == sortedChars[i].value);
					
					if(tmp != null)
					{
						sortedChars[i].used = true;
						used.Add(sortedChars[i]);

						//Aun existen caracteres
						if(used.Count < sortedChars.Count)
						{
							if(searchForNextPossibleWord(tmp))
							{
								//se encontro una palabra!!
								string s = "";
								//Marcamos todos como en desuso
								foreach(ABCChar c in used)
								{
									c.used = false;
									s = s+c.character;
								}

								Debug.Log ("Posible: "+s);

								return true;
							}
							else
							{
								used[0].used = false;
								used.Clear();
							}
						}
					}
				}
			}

			return false;
		}

		/**
		 * Busca dentro dentro de la lista si existen los caracteres en desuso
		 * y forman una palabra
		 **/ 
		protected bool searchForNextPossibleWord(IntList current)
		{
			ABCChar tmp;
			bool result = false;
			int prevused = used.Count;

			foreach(IntList val in current.content)
			{
				tmp = getUnusedCharFromList(val.value, sortedChars);

				if(tmp != null)
				{
					//Lo agregamos como usado
					used.Add(tmp);
					tmp.used = true;

					//Ya es una palabra?
					if(val.end)
					{
						result =  true;
						break;
					}
					else
					{
						//Buscamos otro character
						if(used.Count < sortedChars.Count)
						{
							//Aun existen caracteres asi que buscamos una palabra
							result = searchForNextPossibleWord(val);

							//Ya que se devuelva true
							if(result)
							{
								break;
							}
						}
						else
						{
							//Ya no hay caracteres
							break;
						}
					}
				}
			}

			if(!result)
			{
				//Devolvemos a en desuso los caracteres que se marcaron como tal en esta llamada
				int dif = used.Count - prevused;
				for(int i = 0; i < dif; i++)
				{
					used[used.Count-1].used = false;
					used.RemoveAt(used.Count-1);
				}
			}

			return result;
		}

		/**
		 * Busca un character que no este marcado como usado y que coincida con value
		 * */
		protected ABCChar getUnusedCharFromList(int value,List<ABCChar> chars)
		{
			foreach(ABCChar c in chars)
			{
				if(!c.used && (c.wildcard || c.value == value))
				{
					//Solo para fines de debug le pongo el caharacter y value al comodin
					if(c.wildcard)
					{
						c.value = value;
						c.character = getStringByValue(value);
					}

					return c;
				}
			}

			return null;
		}

		/**
		 * Lee una cadena y lo convierte a un valor entero (basado en su posicion en el abecedario)
		 **/
		public int getCharValue(string c)
		{
			return getCharValue(c.ToCharArray()[0]);
		}

		/**
		 * Lee un caracter y lo convierte a un valor entero (basado en su posicion en el abecedario)
		 **/
		public int getCharValue(char c)
		{
			AlfabetUnit tmp;
			tmp = alfabet.Find(item => item.cvalue == c);

			if(tmp != null)
			{
				return tmp.ivalue;
			}

			return -1;
		}

		/**
		 * Recibe el valor de un caracter basado en el abecedario y devuelve la cadena que lo representa
		 * */
		public string getStringByValue(int value)
		{

			AlfabetUnit tmp;
			tmp = alfabet.Find(item => item.ivalue == value);
			
			if(tmp != null)
			{
				return tmp.cvalue.ToString();
			}
			
			return "";
		}

		/**
		 * Inicializa el alfabeto recibiendo un csv con los caracteres
		 * 
		 * */
		public void initializeAlfabet(string csvText)
		{
			//Nos aseguramos que sean mayusculas
			string[] chars = csvText.ToUpperInvariant().Split(',');
			alfabet = new List<AlfabetUnit>();
			int counter = -1;

			foreach(string schar in chars)
			{
				alfabet.Add(new AlfabetUnit(++counter,schar));
			}
		}

		/**
		 * Procesa el diccionario recibido registrando todas las palabras:
		 * Cada palabra viene en su propia linea
		 **/ 
		public void processDictionary(string text)
		{
			//Breaklines estandar
			Regex regex = new Regex(@"\r\n?|\n");
			text = regex.Replace(text,"\n");

			//Quitamos toda la acentuacion y obtenemos un arreglo
			string[]words = text.Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u').Split('\n');
			int count = words.Length;

			//Inicializamos la data de la estrcutura en blanco
			data = new IntList();
			data.content = new List<IntList>();

			//Registramos todas las palabras
			for(int i =0; i < count; i++)
			{
				//Palabras de menos de 3 caracteres se ignoran
				if(words[i].Length >= 3)
				{
					registerNewWord(words[i]);
				}
			}
		}

		public List<AlfabetUnit> getAlfabet()
		{
			return alfabet;
		}
	}
}