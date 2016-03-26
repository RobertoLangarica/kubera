using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	//Clase creada para poder crear listas de objetos que inicializaran los ABCChar
	public class ABCCharinfo
	{
		public string pointsOrMultiple;
		public int type;
		public string character;
	}

	public class ABCChar
	{
		public bool wildcard = false;//Indica si este caracter es un comodin
		public string character = "A";//La cadena que representa al caracter
		public int value;//Valor del caracter que se utiliza dentro de ABCDataStructure
		public bool used;//Se usa por ABCDataStructure cuando averigua si se pueden armar palabras
		public string pointsOrMultiple;
		public int typeInfo;

		public bool isVocal()
		{
			switch (character) {
			case "A":
			case "E":
			case "I":
			case "O":
			case "U":
				return true;
			default:
				return false;			
			}
		}
	}
}
