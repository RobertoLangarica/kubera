using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ABCSerializer;

namespace ABC.Tree
{
	public class ABCTree 
	{
		public ABCNode root;

		/*****PARA LA BUSQUEDA CARACTER POR CARACTER*****************/
		private bool isValid;
		private ABCNode currentValidationNode;
		private List<ABCNode> levelsOfSearch;
		private int levelsCount;
		/************************************************************/

		/***PARA BUSCAR PALABRAS POSIBLES***************/
		private List<ABCChar> sortedChars;//Caracteres ordenados con los comodines al final
		private List<ABCChar> used;//Vamos guardando los que ya se usaron para la palabra
		/************************************************************/

		public ABCTree()
		{
			root = new ABCNode();
			isValid = true;
			levelsOfSearch = new List<ABCNode>();
			used = new List<ABCChar>();
			currentValidationNode = root;
		}

		/**
		 * @return true:added, false:Not added
		 **/ 
		public bool registerNewIntWord(List<int> word)
		{
			int index = -1;
			int size = word.Count;
			ABCNode next = root;
			ABCNode current = null;

			while(next != null && index < size)
			{
				index++;
				if(index >= size)
				{
					break;
				}
				current = next;
				//Nos traemos el nodo que haga match con el valor
				next = current.children.Find(item => item.value == word[index]);

				//Una nueva subpalabra?
				if(next != null && index == size-1 && !next.end)
				{
					next.end = true;
					return true;
				}
			}

			//Hay que agregar mas nodos?
			if(index < size)
			{
				//Agregamos el character y los subsecuentes
				for(;index < size; index++)
				{
					next = new ABCNode();
					next.value = word[index];
					next.end = (index == size-1);
					current.children.Add(next);
					current = next;
				}
				return true;
			}

			//Palabra existente
			return false;
		}

		public bool isValidWord(List<int> word)
		{
			int index = -1;
			int size = word.Count;
			List<ABCNode> current = null;

			//Buscamos si existe cada caracter
			ABCNode next = root;
			while(next != null && index < size)
			{
				index++;
				if(index >= size)
				{
					break;
				}
				current = next.children;
				next = current.Find(item => item.value == word[index]);

				//No se un match
				if(next == null)
				{
					return false;
				}
				//Se llego al ultimo caracter y no es un nodo terminal
				else if(index == size-1 && !next.end)
				{
					return false;
				}
			}

			return true;
		}

		public void initCharByCharValidation()
		{
			cleanCharByCharValidation();
		}

		public void cleanCharByCharValidation()
		{
			isValid = true;
			currentValidationNode = root;
			levelsCount = 0;
			levelsOfSearch.Clear();
		}

		/**
		 * @return true: Si el caracter es parte de una palabra
		 **/ 
		public bool validateChar(ABCChar c)
		{
			ABCNode tmp = null;

			//Aumentamos el conteo de niveles de busqueda
			levelsCount++;

			if(isValid)
			{
				if(c.wildcard)
				{
					int cant = currentValidationNode.wildcard ? 
						currentValidationNode.children[currentValidationNode.wildcardIndex].children.Count
						: currentValidationNode.children.Count;

					if(cant > 0)
					{
						//El comodin no es parte del arbol es un nodo puntero
						//Es igual a algun nodo y referencia sus hijos asi se maneja externamente
						ABCNode wildcard = new ABCNode();
						wildcard.wildcard = true;
						wildcard.wildcardIndex = 0;

						if(currentValidationNode.wildcard)
						{
							copyListToOnlyMutableList<ABCNode>(wildcard.children,currentValidationNode.children[currentValidationNode.wildcardIndex].children);
						}
						else
						{
							copyListToOnlyMutableList<ABCNode>(wildcard.children,currentValidationNode.children);
						}

						//Valor al que apunta el comodin
						wildcard.value = wildcard.children[wildcard.wildcardIndex].value;
						c.value =   wildcard.value;

						//TODO: Validar si se completa palabra (no por ser comodin es valida la busqueda)
						//Correcto sin validacion ya que es comodin
						currentValidationNode = wildcard;
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
					if(currentValidationNode.wildcard)
					{
						//Si es comodin busca en la lista que el comodin apunta en este momento
						tmp = currentValidationNode.children[currentValidationNode.wildcardIndex].children.Find(item => item.value == c.value);
					}
					else
					{
						//Busqueda directa en el nodo
						tmp = currentValidationNode.children.Find(item => item.value == c.value);
					}


					if(tmp != null)
					{
						//Si existe!!
						//Guardamos el nuevo nivel de busqueda
						currentValidationNode = tmp;
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

				return isValid;
			}

			//Invalido por default
			isValid = false;

			return isValid;
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
			ABCNode tmp = null;
			List<ABCNode> copy = new List<ABCNode>(levelsOfSearch);

			while(--i >= 0)
			{
				//Buscamos un comodin
				if(levelsOfSearch[i].wildcard)
				{
					int childrenCount = levelsOfSearch[i].children.Count;

					//Guardamos el estado actual
					levelsOfSearch[i].lastCorrectValue = levelsOfSearch[i].wildcardIndex;
					List<ABCNode> partialChain = levelsOfSearch.GetRange(i+1,levelsOfSearch.Count-(i+1));
					List<ABCNode> partialResult;

					for(int j = levelsOfSearch[i].wildcardIndex +1; j < childrenCount; j++)
					{
						//Este nuevo comodin es valido?
						levelsOfSearch[i].wildcardIndex = j;
						levelsOfSearch[i].value = levelsOfSearch[i].children[levelsOfSearch[i].wildcardIndex].value;
							
						if(partialChain.Count > 0)
						{
							partialResult = getCorrectChain(levelsOfSearch[i].children[j],partialChain);

							//Se encontro una cadena valida
							if(partialResult != null)
							{
								//Contiene el caracter a validar?
								if(partialResult[partialResult.Count-1].wildcard)
								{
									tmp = partialResult[partialResult.Count-1].children[partialResult[partialResult.Count-1].wildcardIndex].children.Find(item => item.value == charToValidate.value);
								}
								else
								{
									tmp = partialResult[partialResult.Count-1].children.Find(item => item.value == charToValidate.value);
								}
							}
						}
						else
						{
							//No hay nadie debajo y lo validamos directamente a este nodo
							tmp = levelsOfSearch[i].children[levelsOfSearch[i].wildcardIndex].children.Find(item => item.value == charToValidate.value);
						}

						//Existe charToValidate al final de la cadena
						if(tmp != null)
						{
							//LISTO tenemos el valor como debe ser
							levelsOfSearch[i].lastCorrectValue = levelsOfSearch[i].wildcardIndex;
							levelsOfSearch.Add(tmp);
							currentValidationNode = tmp;
							return true;
						}
					}
				}
			}

			//Fue invalido hay que devolver el estado original los comodines
			i = copy.Count;
			while(--i >= 0)
			{
				//Buscamos un comodin
				if(copy[i].wildcard)
				{
					copy[i].wildcardIndex = copy[i].lastCorrectValue;
					if(i == 0)
					{
						copyListToOnlyMutableList<ABCNode>(copy[i].children, root.children);
					}
					else
					{
						copyListToOnlyMutableList<ABCNode>(copy[i].children,copy[i-1].children);
					}
					copy[i].value = copy[i].children[copy[i].wildcardIndex].value;
				}
			}

			return false;
		}

		/**
		 * Busca dentro de target la cadena de nodos que contenga todos los valores dentro de value
		 * y devuelve dicha cadena, si no existe una cadena valida devuelve nulo
		 * */
		protected List<ABCNode> getCorrectChain(ABCNode target, List<ABCNode> values)
		{
			List<ABCNode> result = new List<ABCNode>();

			int limit = values.Count;
			ABCNode tmp;
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
						copyListToOnlyMutableList<ABCNode>(values[i].children, target.children);
					}
					else
					{
						//Comodin apuntando al valor anterior de result
						copyListToOnlyMutableList<ABCNode>(values[i].children, result[i-1].children);
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
						int l2 = values[i].children.Count;
						List<ABCNode> partialChain = values.GetRange(i+1,limit-(i+1));
						List<ABCNode> partialResult;

						for(int j = 0; j < l2; j++)
						{
							//Si es el correcto que se quede con el valor 
							values[i].wildcardIndex = j;
							partialResult = getCorrectChain(values[i].children[j],partialChain);

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
					tmp = target.children.Find(item => item.value == values[i].value);

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

					currentValidationNode = levelsOfSearch[levelsCount-1];
				}
				else
				{
					//Queda como en estado inicial
					initCharByCharValidation();
				}
			}
		}

		public bool currentSearchIsACompleteWord()
		{
			if(isValid)
			{
				if(levelsOfSearch.Count > 0)
				{
					if(levelsOfSearch[levelsOfSearch.Count-1].wildcard)
					{
						return levelsOfSearch[levelsOfSearch.Count-1].children[levelsOfSearch[levelsOfSearch.Count-1].wildcardIndex].end;
					}
					else
					{
						return levelsOfSearch[levelsOfSearch.Count-1].end;
					}
				}
			}

			return false;
		}

		/**
		 * Indica si el objeto IntList tiene el valor searchValue en alguno de sus hijos
		 * */
		protected bool hasValueInChildren(ABCNode lst, int searchValue)
		{
			ABCNode tmp;
			tmp = lst.children.Find(item => item.value == searchValue);

			if(tmp != null)
			{
				return true;
			}

			return false;
		}

		/**
		 * Recibe un grupo de caracteres y determina si es posible armar una palabra con ellos
		 **/ 
		public List<ABCChar> getPossibleWord(List<ABCChar> chars)
		{
			List<ABCChar> result = new List<ABCChar>();
			Dictionary<int,bool> validated = new Dictionary<int, bool>();//La letra con la que ya se inicio una busqueda
			ABCNode tmp;
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

			//Limpiamos la lista de usados
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
					tmp = root.children.Find(item => item.value == sortedChars[i].value);

					if(tmp != null)
					{
						sortedChars[i].used = true;
						used.Add(sortedChars[i]);

						//Aun existen caracteres
						if(used.Count < sortedChars.Count)
						{
							if(nodeHaveAPossibleWord(tmp))
							{
								result = new List<ABCChar>(used);

								//Marcamos todos como en desuso
								foreach(ABCChar c in used)
								{
									c.used = false;
								}

								used.Clear();

								return result;

							}
							else
							{
								//removemos el unico marcado como usado
								used[0].used = false;
								used.Clear();
							}
						}
						else
						{
							//removemos el unico marcado como usado
							used[0].used = false;
							used.Clear();
						}
					}
				}
			}

			return result;
		}

		/**
		 * Busca dentro dentro de la lista si existen los caracteres en desuso
		 * y forman una palabra
		 **/ 
		protected bool nodeHaveAPossibleWord(ABCNode current)
		{
			ABCChar tmp;
			bool result = false;
			//int prevused = used.Count;

			foreach(ABCNode val in current.children)
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
							result = nodeHaveAPossibleWord(val);

							//Ya que se devuelva true
							if(result)
							{
								break;
							}
							else
							{
								//Removemos el ultimo nodo agregado
								used[used.Count-1].used = false;
								used.RemoveAt(used.Count-1);
							}
						}
						else
						{
							//Ya no hay caracteres
							//Removemos el ultimo nodo agregado
							used[used.Count-1].used = false;
							used.RemoveAt(used.Count-1);
							break;
						}
					}
				}
			}

			/*if(!result)
			{
				//Devolvemos a en desuso los caracteres que se marcaron como tal en esta llamada
				int dif = used.Count - prevused;
				for(int i = 0; i < dif; i++)
				{
					used[used.Count-1].used = false;
					used.RemoveAt(used.Count-1);
				}
			}*/


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
					if(c.wildcard)
					{
						c.value = value;
					}

					return c;
				}
			}

			return null;
		}
			

		public void copyListToOnlyMutableList<T>(List<T> mutable, List<T> dataToCopy)
		{
			mutable.Clear();

			foreach(T t in dataToCopy)
			{
				mutable.Add(t);
			}
		}
	}	
}
