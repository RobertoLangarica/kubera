using UnityEngine;
using System.Collections;
using ABC;

public class Cell : MonoBehaviour 
{
	public enum EType
	{
		NORMAL,COLORED,EMPTY,OBSTACLE_LETTER,EMPTY_VISIBLE_CELL,TUTORIAL_LETTER
	}

	public Piece.EType contentType;
	public Piece.EColor contentColor;
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
	 * 16 = Celda vaci pero visible
	 * 32 = Letra tutorial
	 * 
	 * El siguiente nibble se utiliza para los colores que se marcan en la bandera 2.
	 * Son colores del 0 al 7 de momento
	 * 0xNNNN0000
	 */
	[HideInInspector]
	public int type;

	public EType cellType;
	public int colorIndex;

	/*
	 * La funcion solo refresca los valores en las celdas
	 */
	public void clearCell()
	{
		contentType = Piece.EType.NONE;
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
			cellType = EType.NORMAL;
			contentType = Piece.EType.NONE;
			occupied = false;
			available = true;
			content = null;
		}

		if((type & 0x2) == 0x2)
		{
			cellType = EType.COLORED;
			colorIndex = type >> 6;
			occupied = true;
			available = true;
			content = null;

			updateContentTypeByColorIndex(colorIndex);
		}
		if((type & 0x4) == 0x4)
		{
			cellType = EType.EMPTY;
			contentType = Piece.EType.NONE;
			occupied = true;
			available = false;
			content = null;

			GetComponent<SpriteRenderer>().enabled = false;
		}
		if((type & 0x8) == 0x8)
		{
			cellType = EType.OBSTACLE_LETTER;
			contentType = Piece.EType.LETTER_OBSTACLE;
			occupied = true;
			available = false;
		}

		if((type & 0x20) == 0x20)
		{
			cellType = EType.TUTORIAL_LETTER;
			contentType = Piece.EType.LETTER;
			occupied = true;
			available = false;
		}
	}

	public void updateContentTypeByColorIndex(int color)
	{
		switch(color)
		{
		case(1):
			contentColor = Piece.EColor.YELLOW;
			break;
		case(2):
			contentColor = Piece.EColor.MAGENTA;
			break;
		case(3):
			contentColor = Piece.EColor.CYAN;
			break;
		case(4):
			contentColor = Piece.EColor.GREEN;
			break;
		case(5):
			contentColor = Piece.EColor.RED;
			break;
		case(6):
			contentColor = Piece.EColor.PURPLE;
			break;
		case(7):
			contentColor = Piece.EColor.BLUE;
			break;
		case(8):
			contentColor = Piece.EColor.ORANGE;
			break;
		case(9):
			contentColor = Piece.EColor.VIOLET;
			break;
		case(10):
			contentColor = Piece.EColor.TURQUOISE;
			break;
		}
	}
}