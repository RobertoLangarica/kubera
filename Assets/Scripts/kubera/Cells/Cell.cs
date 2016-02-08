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
	 * 0x0000
	 * 1 = Celda disponblbe y normal
	 * 2 = Cubo de color que se agrega desde el inicio
	 * 4 = Celda vacia (si cuenta para la linea)
	 * 8 = Letra como obstaculo
	 * 
	 * El siguiente nibble se utiliza para los colores que se marcan en la bandera 2
	 * Son colores del 0 al 7 de momento
	 * 0xNNNN0000
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
			occupied = false;
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