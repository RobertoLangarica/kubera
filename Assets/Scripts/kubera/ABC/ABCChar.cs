using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ABC
{
	//Clase creada para poder crear listas de objetos que inicializaran loas ABCChar
	public class ScriptableABCChar
	{
		public string pointsValue;
		public string typeOfLetter;
		public string character;
	}

	public class ABCChar : MonoBehaviour 
	{
		public int value;//Valor del caracter que se utiliza dentro de ABCDataStructure
		public bool wildcard = false;//Indica si este caracter es un comodin
		
		public string character = "A";//La cadena que representa al caracter
		[HideInInspector]public bool empty = false;//Lo usa WordManager al eliminar caracteres
		[HideInInspector]public bool used;//Se usa por ABCDataStructure cuando averigua si se pueden armar palabras
		[HideInInspector]public int index;//Indice del caracter en WordManager

		public string pointsValue;//Cantidad de puntos que entraga la letra al ser usada
		public string typeOfLetter;//El tpo de letra que es, puede ntregar powerUps al momento de usarse

		public bool isSelected;

		public Text letter;
		public Text pointsValueText;

		void Start () 
		{
			//si es comodin lo dejamos en blanco y sino le dejamos el texto adecuado
			if(!wildcard)
			{
				ABCDataStructure abc = FindObjectOfType<ABCDataStructure>();
				value = abc.getCharValue(character.ToUpper().ToCharArray()[0]);
				character = abc.getStringByValue(value);
			}
			else
			{
				character = "";
			}


			//Actualizamos el texto.
			if(letter != null && pointsValueText != null)
			{
				letter.text = character;
				pointsValueText.text = pointsValue;
			}
		}

		public void initializeFromScriptableABCChar(ScriptableABCChar scriptAbcVals)
		{
			character = scriptAbcVals.character;
			pointsValue = scriptAbcVals.pointsValue;
			typeOfLetter = scriptAbcVals.typeOfLetter;

			print (character + " " + pointsValue + " " + typeOfLetter);
			if(character == ".")
			{
				wildcard = true;
			}
		}
	}
}
