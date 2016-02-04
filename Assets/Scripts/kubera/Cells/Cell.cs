using UnityEngine;
using System.Collections;
using ABC;

public class Cell : MonoBehaviour 
{
	public ETYPEOFPIECE_ID typeOfPiece;
	public bool available;
	public bool occupied;
	public GameObject piece;

	/*
	 * Valores en el XML para cada tipo de celda
	 * 1 = Celda disponblbe y normal
	 * 2 = Celda que al momento de que se le manda a llamar clearCell se convertira en tipo 1
	 *     EJEMPLO:Para el Tile que empieza con una letra y al destruirla queda disponible, se le coloca esta bandera para que al hacerle clear se convierta en tipo 1
	 * 4 = Celda que es un hueco en la grid (Este si cuenta para la linea)
	 * 8 = Celda que es un obstaculo en la grid (En este obstaculo se agregaran letras a destruir)(hay que añadirle el '2' para que libere la celda)
	 */
	[HideInInspector]
	public int cellType;

	/*
	 * La funcion solo refresca los valores en las celdas
	 */
	public void clearCell()
	{
		typeOfPiece = ETYPEOFPIECE_ID.NONE;
		occupied = false;
		piece = null;

		if((cellType & 0x2) == 0x2)
		{
			cellType = 1;
			available = true;
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

	public bool canPositionateOnThisCell()
	{
		if((cellType & 0x1) == 0x1)
		{
			return true;
		}
		return false;
	}

	public void setTypeToCell(int newCellType = 1)
	{
		cellType = newCellType;
		if((cellType & 0x1) == 0x1)
		{
			typeOfPiece = ETYPEOFPIECE_ID.NONE;
			occupied = false;
			available = true;
			piece = null;
		}
		if((cellType & 0x2) == 0x2)
		{
		}
		if((cellType & 0x4) == 0x4)
		{
			typeOfPiece = ETYPEOFPIECE_ID.NONE;
			occupied = true;
			available = true;
			piece = null;

			//Cambio Temporal
			GetComponent<SpriteRenderer>().enabled = false;
		}
		if((cellType & 0x8) == 0x8)
		{
			typeOfPiece = ETYPEOFPIECE_ID.LETTER_FROM_BEGINING;
			occupied = true;
			available = false;
		}
	}
}