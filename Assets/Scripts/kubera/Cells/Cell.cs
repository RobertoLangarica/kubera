using UnityEngine;
using System.Collections;
using ABC;

public class Cell : MonoBehaviour 
{
	public EPieceType pieceType;
	public bool available;
	public bool occupied;
	public GameObject content;

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
		pieceType = EPieceType.NONE;
		occupied = false;
		content = null;

		if((cellType & 0x2) == 0x2 || (cellType & 0x8) == 0x8)
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
		DestroyImmediate(content);
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
		int tempType = 0;

		cellType = newCellType;
		if((cellType & 0x1) == 0x1)
		{
			pieceType = EPieceType.NONE;
			occupied = false;
			available = true;
			content = null;
		}
		if((cellType & 0x2) == 0x2)
		{
			occupied = true;
			available = true;
			content = null;
			tempType = cellType >> 4;

			updateCellColor(tempType);
		}
		if((cellType & 0x4) == 0x4)
		{
			pieceType = EPieceType.NONE;
			occupied = true;
			available = true;
			content = null;

			//Cambio Temporal
			GetComponent<SpriteRenderer>().enabled = false;
		}
		if((cellType & 0x8) == 0x8)
		{
			pieceType = EPieceType.LETTER_OBSTACLE;
			occupied = true;
			available = false;
		}
	}

	public void updateCellColor(int color)
	{
		switch(color)
		{
		case(1):
			pieceType = EPieceType.AQUA;
			//Debug.Log(typeOfPiece);
			break;
		case(2):
			pieceType = EPieceType.BLUE;
			//Debug.Log(typeOfPiece);
			break;
		case(3):
			pieceType = EPieceType.GREEN;
			//Debug.Log(typeOfPiece);
			break;
		case(4):
			pieceType = EPieceType.MAGENTA;
			//Debug.Log(typeOfPiece);
			break;
		case(5):
			pieceType = EPieceType.RED;
			//Debug.Log(typeOfPiece);
			break;
		case(6):
			pieceType = EPieceType.YELLOW;
			//Debug.Log(typeOfPiece);
			break;
		case(7):
			pieceType = EPieceType.GREY;
			//Debug.Log(typeOfPiece);
			break;
		}
	}
}