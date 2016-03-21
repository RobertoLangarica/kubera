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

	public class ABCChar : MonoBehaviour 
	{
		public enum EType
		{
			OBSTACLE, NORMAL	
		}


		public bool wildcard = false;//Indica si este caracter es un comodin
		public string character = "A";//La cadena que representa al caracter
		public int value;//Valor del caracter que se utiliza dentro de ABCDataStructure
		public bool used;//Se usa por ABCDataStructure cuando averigua si se pueden armar palabras

		public string pointsOrMultiple;
		public Text txtLetter;
		public Text txtPoints;
		public EType type;
		[HideInInspector]public bool empty = false;//Lo usa WordManager al eliminar caracteres
		[HideInInspector]public int index;//Indice del caracter en WordManager


		protected bool textActualized; //texto actualizado



		void Start () 
		{
			//si es comodin lo dejamos en blanco y sino le dejamos el texto adecuado
			if(!wildcard)
			{
				ABCDictionary abc = FindObjectOfType<ABCDictionary>();
				value = abc.getCharValue(character.ToUpper().ToCharArray()[0]);
				character = abc.getStringByValue(value);
			}
			else
			{
				character = "";
			}
		}

		/**
		 * Inicializamos el texto
		 **/
		public void initializeText()
		{
			if(txtLetter != null && txtPoints != null)
			{
				txtLetter.text = character;
				txtPoints.text = pointsOrMultiple;
			}
		}

		public void initializeFromInfo(ABCCharinfo info)
		{
			character = info.character;
			pointsOrMultiple = info.pointsOrMultiple;
			type = (EType)info.type;

			initializeText ();

			if(character == ".")
			{
				wildcard = true;
			}
		}

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
