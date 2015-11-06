using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ABCChar : MonoBehaviour 
{
	public int value;//Valor del caracter que se utiliza dentro de ABCDataStructure
	public bool wildcard = false;//Indica si este caracter es un comodin

	[HideInInspector]
	public string character = "A";//La cadena que representa al caracter
	[HideInInspector]
	public bool empty = false;//Lo usa WordManager al eliminar caracteres
	[HideInInspector]
	public bool used;//Se usa por ABCDataStructure cuando averigua si se pueden armar palabras
	[HideInInspector]
	public int index;//Indice del caracter en WordManager

	void Start () 
	{
		//si es comodin lo dejamos en blanco y sino le dejamos el texto adecuado
		if(!wildcard)
		{
			character = ABCDataStructure.getStringByValue(value);
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
}
