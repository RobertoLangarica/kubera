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

		protected WordManager wordManager;
		protected bool usedFromGrid;
		public bool isSelected;

		void Start () 
		{
			wordManager = FindObjectOfType<WordManager>();

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


			//Si se encuentra un componente texto le actualizamos el texto.
			Text txt = GetComponentInChildren<Text>();

			if(txt != null)
			{
				txt.text = character;
			}
		}

		public void initializeFromScriptableABCChar(ScriptableABCChar scriptAbcVals)
		{
			character = scriptAbcVals.character;
			pointsValue = scriptAbcVals.pointsValue;
			typeOfLetter = scriptAbcVals.typeOfLetter;
			
			if(character == ".")
			{
				wildcard = true;
			}
		}
	
		
		public void ShootLetter()
		{
			if(!usedFromGrid)
			{
				wordManager.addCharacter(this,gameObject);
				usedFromGrid=true;
				gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,.2f);
			}
		}
		
		public void backToNormal()
		{
			usedFromGrid=false;
			gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
		}
	}
}
