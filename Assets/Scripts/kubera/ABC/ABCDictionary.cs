using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ABC.Tree;

namespace ABC
{
	public class ABCDictionary : MonoBehaviour 
	{
		private List<ABCUnit> alfabet;
		private ABCTree tree = null;

		/**Para el proecsamiento del diccionario en texto**/
		public delegate void DNotify();
		public DNotify onDictionaryFinished;
		private string[] dictionaryWords;
		private int processCount;
		/**********/

		void Start () 
		{
			if(tree == null)
			{
				tree = new ABCTree();
			}
		}

		/**
		 * Registra una nueva palabra en el diccionario
		 * si la palabra existe previamente ya no se agrega 
		 * y si existe parcialmente solo se agregan las partes que no existian previamente
		 **/ 
		public void registerNewWord(string word)
		{
			tree.registerNewIntWord(convertWordToIntsWord(word));

			/*if(!tree.registerNewIntWord(convertWordToIntsWord(word)))
			{
				Debug.Log("Ya existia la palabra: "+word);	
			}*/
		}

		public List<int> convertWordToIntsWord(string word)
		{
			char[] chars = word.ToCharArray();
			List<int> result = new List<int>();

			foreach(char character in chars)
			{
				result.Add(getCharValue(character));	
			}

			return result;
		}

		/**
		 * Recibe una cadena entera e indica si dicha cadena se encuentra dentro del diccionario
		 * */
		public bool isValidWord(string word)
		{
			return tree.isValidWord(convertWordToIntsWord(word));
		}

		/**
		 * Inicializa el chequeo caracter por caracter de una palabra,
		 * para ir validando cada caracter se usa validateChar
		 * */
		public void initCharByCharValidation()
		{
			tree.initCharByCharValidation();
		}

		/**
		 * Limpia y deja en estado sin busqueda
		 * */ 
		public void cleanCharByCharValidation()
		{
			tree.cleanCharByCharValidation();
		}

		/**
		 * Devuelve true si el caracter es parte de una palabra
		 **/ 
		public bool validateChar(ABCChar c)
		{
			if(c.wildcard)
			{
				//El arbol no conoce los caracteres
				//Adignamos el caracter que toma este comodin
				bool result = tree.validateChar(c);
				c.character = getStringByValue(c.value);

				return result;
			}
			else
			{
				return tree.validateChar(c);	
			}
		}

		public bool isCompleteWord()
		{
			return tree.currentSearchIsACompleteWord();
		}

		/**
		 * Elimina un nivel de busqueda, si se elimina un nivel que no es el ultimo
		 * entonces se eliminaran todos los niveles para no fragmentar la lista,
		 * la recuperacion de los niveles se debe de manejar afuera donde se controlan los
		 * caracteres
		 * */
		public void deleteLvlOfSearch(int indexToDelete)
		{
			tree.deleteLvlOfSearch(indexToDelete);
		}

		/**
		 * Recibe un grupo de caracteres y determina si es posible armar una palabra con ellos
		 **/ 
		public bool isAWordPossible(List<ABCChar> chars)
		{
			List<ABCChar> word = tree.getPossibleWord(chars);

			if(word.Count > 0)
			{
				Debug.Log("Posible palabra: "+getStringFromCharList(word));
				return true;
			}

			return false;
		}

		public List<ABCChar> getPosibleWord(List<ABCChar> chars)
		{
			return tree.getPossibleWord(chars);
		}

		private string getStringFromCharList(List<ABCChar> chars)
		{
			string result = "";

			foreach(ABCChar c in chars)
			{
				result += getStringByValue(c.value);
			}

			return result;
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
			ABCUnit tmp;
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

			ABCUnit tmp;
			tmp = alfabet.Find(item => item.ivalue == value);
			
			if(tmp != null)
			{
				return tmp.cvalue.ToString();
			}
			
			return "";
		}

		public void initializeAlfabet(string csvText)
		{
			//Nos aseguramos que sean mayusculas
			string[] chars = csvText.ToUpperInvariant().Split(',');
			alfabet = new List<ABCUnit>();
			int counter = -1;

			foreach(string schar in chars)
			{
				alfabet.Add(new ABCUnit(++counter,schar));
			}
		}
			
		float initTime;
		/**
		 * Procesa el diccionario recibido registrando todas las palabras:
		 * Cada palabra viene en su propia linea
		 **/ 
		public void processDictionary(string text, int maxWordLength = 30)
		{
			Regex regex = new Regex(@"\r\n?|\n");
			StringBuilder builder = new StringBuilder(regex.Replace(text,"\n").ToUpperInvariant());

			//Quitamos toda la acentuacion y obtenemos un arreglo
			builder.Replace('Á','A').Replace('É','E').Replace('Í','I').Replace('Ó','O').Replace('Ú','U').Replace('Ü','U');

			dictionaryWords = builder.ToString().Split('\n');

			//Inicializamos la data de la estrcutura en blanco
			tree = new ABCTree();

			initTime = Time.realtimeSinceStartup;

			//Descomentar para dividir el trabajo en frames
			StartCoroutine("registerDictionaryWords",maxWordLength); 
			return;

			/*
			int rejected = 0;
			int l = dictionaryWords.Length;
			//Registramos todas las palabras
			for(int i = 0; i < l; i++)
			{
				if(dictionaryWords[i].Length < maxWordLength)
				{
					registerNewWord(dictionaryWords[i]);
				}
				else
				{
					rejected++;
				}
			}

			Debug.Log("Registered words: "+(l-rejected));
			Debug.Log("Rejected words by size: "+rejected);
			Debug.Log("Dictionary finished in: "+ (Time.realtimeSinceStartup-initTime).ToString("0000.000")+"s");
			if(onDictionaryFinished != null)
			{
				onDictionaryFinished();
			}*/
		}

		protected IEnumerator registerDictionaryWords(int maxWordLength = 30)
		{
			processCount = 0;

			int rejected = 0;
			//Registramos todas las palabras
			for(int i = 0; i < dictionaryWords.Length; i++)
			{
				if(processCount >= 3000)
				{
					//Debug.Log("remaining: "+(dictionaryWords.Length-i).ToString());
					processCount = 0;
					yield return null;
				}

				processCount++;

				if(dictionaryWords[i].Length < maxWordLength)
				{
					registerNewWord(dictionaryWords[i]);
				}
				else
				{
					rejected++;
				}
			}

			Debug.Log("Registered words: "+(dictionaryWords.Length-rejected));
			Debug.Log("Rejected words by size: "+rejected);
			Debug.Log("Dictionary finished in: "+ (Time.realtimeSinceStartup-initTime).ToString("0000.000")+"s");
			if(onDictionaryFinished != null)
			{
				onDictionaryFinished();
			}

			yield break;
		}

		public List<ABCUnit> getAlfabet()
		{
			return alfabet;
		}

		public void setTreeRoot(ABCSerializer.ABCNode rootNode)
		{
			if(tree == null)
			{
				tree = new ABCTree();
			}

			tree.root = rootNode;
		}

		public ABCSerializer.ABCNode getTreeRoot()
		{
			return tree.root;
		}
	}
}