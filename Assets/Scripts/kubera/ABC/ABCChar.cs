using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

	protected string pointsValue;//Cantidad de puntos que entraga la letra al ser usada
	protected string typeOfLetter;//El tpo de letra que es, puede ntregar powerUps al momento de usarse

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


		//Si se encuentra un componente texto le actualizamos el texto.
		Text txt = GetComponentInChildren<Text>();

		if(txt != null)
		{
			txt.text = character;
		}
	}

	void initializeFromScriptableABCChar(ScriptableABCChar scriptAbcVals)
	{
		character = scriptAbcVals.character;
		pointsValue = scriptAbcVals.pointsValue;
		typeOfLetter = scriptAbcVals.typeOfLetter;
	}

	public void givePoint()
	{
		switch(pointsValue)
		{
		case("x2"):
		{}
			break;
		case("x3"):
		{}
			break;
		default:
		{}
			break;
		}
	}
}
