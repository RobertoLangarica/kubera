using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ABC.Tree;

namespace ABC
{
	public class ABCDataStructure : MonoBehaviour 
	{
		//Alfabeto que contiene todos los caracteres y su representacion en entero
		private List<AlfabetUnit> alfabet;

		[HideInInspector]public ABCTree tree = null;

		/**Para el proecsamiento del diccionario**/
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

			/*for(int i = 0; i < word.Length; i++)
			{
				result.Add(int.Parse(word[i].ToString()));
			}*/

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
			return tree.validateChar(c);
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
			
		float initTime;
		/**
		 * Procesa el diccionario recibido registrando todas las palabras:
		 * Cada palabra viene en su propia linea
		 **/ 
		public void processDictionary(string text)
		{
			Regex regex = new Regex(@"\r\n?|\n");
			StringBuilder builder = new StringBuilder(regex.Replace(text,"\n").ToUpperInvariant());
			//text = regex.Replace(text,"\n");

			//Quitamos toda la acentuacion y obtenemos un arreglo
			builder.Replace('Á','A').Replace('É','E').Replace('Í','I').Replace('Ó','O').Replace('Ú','U').Replace('Ü','U');

			//text = text.Replace('á','a').Replace('é','e').Replace('í','i').Replace('ó','o').Replace('ú','u').Replace('ü','u').ToUpperInvariant();

			/*foreach(AlfabetUnit abc in alfabet)
			{
				builder.Replace(abc.cvalue.ToString(),abc.ivalue.ToString());
			}*/

			dictionaryWords = builder.ToString().Split('\n');

			//Inicializamos la data de la estrcutura en blanco
			tree = new ABCTree();

			initTime = Time.realtimeSinceStartup;

			//Descomentar para dividir el trabajo en frames
			StartCoroutine("registerDictionaryWords"); 
			return;


			int l = dictionaryWords.Length;
			//Registramos todas las palabras
			for(int i = 0; i < l; i++)
			{
				registerNewWord(dictionaryWords[i]);
			}


			Debug.Log("Dictionary finished in: "+ (Time.realtimeSinceStartup-initTime).ToString("0000.000")+"s");
			if(onDictionaryFinished != null)
			{
				onDictionaryFinished();
			}
		}

		protected IEnumerator registerDictionaryWords()
		{
			processCount = 0;

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
				registerNewWord(dictionaryWords[i]);
			}

			Debug.Log("Dictionary finished in: "+ (Time.realtimeSinceStartup-initTime).ToString("0000.000")+"s");
			if(onDictionaryFinished != null)
			{
				onDictionaryFinished();
			}

			yield break;
		}

		public List<AlfabetUnit> getAlfabet()
		{
			return alfabet;
		}
	}
}