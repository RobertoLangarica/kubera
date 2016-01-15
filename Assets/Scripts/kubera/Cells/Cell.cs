using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour 
{
	public ECOLORS_ID color;
	public bool available;
	public bool occupied;
	public GameObject piece;

	/*
	 * Valores en el XML para cada tipo de celda
	 * 1 = Celda disponblbe y normal
	 * 2 = Celda que es un hueco en la grid (Este si cuenta para la linea)
	 * 4 = Celda que al momento de que se le manda a llamar clearCell se convertira en tipo 1
	 *     EJEMPLO:Para el Tile que empieza con una letra y al destruirla queda disponible, se le coloca esta bandera para que al hacerle clear se convierta en tipo 1
	 * 8 = Celda que es un obstaculo en la grid (En este obstaculo se agregaran letras a destruir)
	 */
	[HideInInspector]
	public int cellType;

	/*
	 * La funcion solo refresca los valores en las celdas
	 */
	public void clearCell()
	{
		color = ECOLORS_ID.NONE;
		occupied = false;
		piece = null;

		if((cellType & 0x4) == 0x4)
		{
			cellType = 1;
		}
	}

	/*
	 * La funcion destruye las piezas colocadas en la celda
	 */
	public void destroyCell()
	{
		DestroyImmediate(piece);
		clearCell();
	}

	public bool canPositionateInThisCell()
	{
		if((cellType & 0x1) == 0x1)
		{
			return true;
		}
		return false;
	}
}