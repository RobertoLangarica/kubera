using UnityEngine;
using System.Collections;
using ABC;

public class Cell : MonoBehaviour 
{
	public EPieceType contentType;
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
	 * 16 = Letra como obstaculo pero visible
	 * 32 = Letra tutorial
	 * 
	 * El siguiente nibble se utiliza para los colores que se marcan en la bandera 2
	 * Son colores del 0 al 7 de momento
	 * 0xNNNN0000
	 */
	[HideInInspector]
	public int type;

	/*
	 * La funcion solo refresca los valores en las celdas
	 */
	public void clearCell()
	{
		contentType = EPieceType.NONE;
		occupied = false;
		content = null;

		if((type & 0x2) == 0x2 || (type & 0x8) == 0x8 || (type & 0x20) == 0x20)
		{
			type = 1;
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
		if((type & 0x1) == 0x1)
		{
			return true;
		}
		return false;
	}

	public void setType(int newCellType = 1)
	{
		type = newCellType;

		if((type & 0x1) == 0x1)
		{
			contentType = EPieceType.NONE;
			occupied = false;
			available = true;
			content = null;
		}
		if((type & 0x2) == 0x2)
		{
			occupied = true;
			available = true;
			content = null;

			updateCellColor(type >> 6);
		}
		if((type & 0x4) == 0x4)
		{
			contentType = EPieceType.NONE;
			occupied = true;
			available = false;
			content = null;

			//Cambio Temporal
			GetComponent<SpriteRenderer>().enabled = false;
		}
		if((type & 0x8) == 0x8)
		{
			contentType = EPieceType.LETTER_OBSTACLE;
			occupied = true;
			available = false;
		}
		if((type & 0x10) == 0x10)
		{
			contentType = EPieceType.NONE;
			occupied = true;
			available = false;
			content = null;

			//Cambio Temporal
			GetComponent<SpriteRenderer>().color = Color.cyan;
		}
		if((type & 0x20) == 0x20)
		{
			contentType = EPieceType.LETTER;
			occupied = true;
			available = false;
		}
	}

	public void updateCellColor(int color)
	{
		switch(color)
		{
		case(1):
			contentType = EPieceType.AQUA;
			break;
		case(2):
			contentType = EPieceType.BLUE;
			break;
		case(3):
			contentType = EPieceType.GREEN;
			break;
		case(4):
			contentType = EPieceType.MAGENTA;
			break;
		case(5):
			contentType = EPieceType.RED;
			break;
		case(6):
			contentType = EPieceType.YELLOW;
			break;
		case(7):
			contentType = EPieceType.GREY;
			break;
		}
	}
}