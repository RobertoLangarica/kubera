using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABC;

public class CellsManager : MonoBehaviour 
{
	public delegate void letterCreated(ABCChar abcChar,UIChar uiChar,bool isBlackLetter);
	[HideInInspector]public letterCreated OnLetterCreated;

	public GameObject cellPrefab;
	public GameObject obstacleLetterPrefab;
	public GameObject singleSquarePiece;
	public GameObject uiLetter;

	[HideInInspector]public int columns = 0;
	[HideInInspector]public int rows = 0;

	//Todas las celdas del grid
	protected List<Cell> cells;

	/*
	 * Se crea la grid y se asigna cada celda como hijo de este gameObject.
	 * Las celdas tienen un espacio de 3% de su tamaño de espacio en tre ellas
	 * 
	 * @return true:Si hubo resize, false: Si no hubo resize
	 */
	public bool resizeGrid(int _columns, int _rows)
	{
		if(_columns != columns || _rows != rows)
		{
			destroyGrid();

			columns = _columns;
			rows = _rows;

			cells = new List<Cell>();

			Vector3 cellInitialPosition = transform.position;
			GameObject cellInstance = null;

			for(int i = 0;i < _rows;i++)
			{
				for(int j = 0;j < _columns;j++)
				{
					cellInstance = GameObject.Instantiate(cellPrefab,cellInitialPosition,Quaternion.identity) as GameObject;
					cellInstance.GetComponent<SpriteRenderer> ().sortingOrder = -1;
					cellInstance.transform.SetParent(transform);
					cells.Add(cellInstance.GetComponent<Cell>());
					
					cellInitialPosition.x += cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x + 0.03f;
				}
				cellInitialPosition.y -= cellPrefab.GetComponent<SpriteRenderer>().bounds.size.y + 0.03f;
				cellInitialPosition.x = transform.position.x;
			}
			return true;
		}

		return false;
	}

	/*
	 * Destruye los objetos del Grid y limpia las celdas
	 */
	protected void destroyGrid()
	{
		if(cells == null)
		{
			return;	
		}

		foreach(Cell val in cells)
		{
			DestroyImmediate(val.gameObject);
		}
		cells.Clear();
	}

	public void resetGrid(int _columns, int _rows)
	{
		if(!resizeGrid(columns,rows))
		{
			setAllCellsToType(EPieceType.NONE);
		}
	}
	
	public void resetGrid()
	{
		setAllCellsToType(EPieceType.NONE);
	}

	public void setAllCellsToType(EPieceType type)
	{
		for(int i = 0;i < rows;i++)
		{
			for(int j = 0;j < columns;j++)
			{
				//[TODO] asignarle tipo
				//getCellAt(j,i).setTypeToCell();
			}
		}
	}

	/*
	 * Regresa la celda debajo del punto (aquella celda que dentro de sus boundaries contenga al punto)
	 * 
	 * @param point Punto sobre el que se evaluan las celdas a ver si alguna esta debajo
	 * 
	 * @return Celda bajo el punto o nulo si no existe
	 */
	public Cell getCellUnderPoint(Vector2 point)
	{
		Vector3 cellPos;
		float cellWidth, cellHeight;
		SpriteRenderer renderer;

		foreach(Cell cell in cells)
		{
			cellPos		= cell.transform.position;
			renderer	= cell.gameObject.GetComponent<SpriteRenderer>();
			cellWidth	= renderer.bounds.size.x;
			cellHeight	= renderer.bounds.size.y;

			if(point.x > cellPos.x && point.x < (cellPos.x + cellWidth) &&
				point.y < cellPos.y && point.y > (cellPos.y - cellHeight))
			{
				return cell;
			}
		}
		return null;
	}

	public Cell getCellUnderPoint(Vector3 point)
	{
		return getCellUnderPoint(new Vector2(point.x, point.y));
	}

	/*
	 * Regresa una Celda dependiendo la posicion (x,y) dentro del grid
	 * 
	 * @param xPos{int}: Posicion 'x' del grid para buscar la celda
	 * @param yPos{int}: Posicion 'y' del grid para buscar la celda
	 * 
	 * @return La celda en la posicion especificada o nulo si no existe
	 */
	protected Cell getCellAt(int xPos,int yPos)
	{
		if(xPos < 0 || yPos < 0 || xPos >= columns || yPos >= rows)
		{
			return null;
		}
		return cells[(columns*yPos)+xPos];
	}

	public List<List<Cell>> getCompletedHorizontalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>(rows);
		Cell cell;
		int noneCells = 0;

		for(int y = 0; y < rows; y++)
		{
			result.Add(new List<Cell>(columns));
			noneCells = 0;
			for(int x = 0; x < columns; x++)
			{
				cell = getCellAt(x,y);
				if(cell.occupied && cell.pieceType != EPieceType.LETTER && cell.pieceType != EPieceType.LETTER_OBSTACLE)
				{
					if(cell.content != null)
					{
						result [result.Count - 1].Add (cell);
					}
					if(cell.pieceType == EPieceType.NONE)
					{
						noneCells++;
					}
				}
				else
				{
					result.RemoveAt(result.Count-1);
					break;
				}
			}
			if(noneCells == columns)
			{
				result.RemoveAt(result.Count-1);
			}
		}

		return result;
	}

	public List<List<Cell>> getCompletedVerticalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>(columns);
		Cell cell;
		int noneCells = 0;

		for(int x = 0; x < columns; x++)
		{
			result.Add(new List<Cell>(rows));
			noneCells = 0;
			for(int y = 0; y < rows; y++)
			{
				cell = getCellAt(x,y);
				if(cell.occupied && cell.pieceType != EPieceType.LETTER && cell.pieceType != EPieceType.LETTER_OBSTACLE)
				{
					if(cell.content != null)
					{
						result [result.Count - 1].Add (cell);
					}
					if(cell.pieceType == EPieceType.NONE)
					{
						noneCells++;
					}
				}
				else
				{
					result.RemoveAt(result.Count-1);
					break;
				}
			}
			if(noneCells == rows)
			{
				result.RemoveAt(result.Count-1);
			}
		}

		return result;
	}
		
	public List<List<Cell>> getCompletedVerticalAndHorizontalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>();

		result.AddRange(getCompletedHorizontalLines());
		result.AddRange(getCompletedVerticalLines());

		return result;
	}


	/*
	 * Evalua si los objetos se pueden posicionar dentro de la grid.
	 * Se evaluan en la posicion donde se encuentran
	 * 
	 * @param objects{GameObject[]}: Objetos a evaluar
	 * 
	 * @return true:si es posible colocarla, false:No se pudo posicionar alguna o todas
	 */
	public bool canPositionateAll(GameObject[] objects)
	{
		List<Vector3> positions = new List<Vector3>(objects.Length);

		for(int i = 0;i < objects.Length;i++)
		{
			positions.Add(objects[i].transform.position);	
		}

		return canPositionateAll(positions.ToArray());
	}
	
	/*
	 * Evalua si todas las posicions son celdas validas y vacias.
	 * 
	 * @params positions{Vecto3[]}: Posiciones a evaluar
	 * 
	 * @return true: Si es posible colocarlas, false:No se puede posicionar alguna o ninguna
	 */
	public bool canPositionateAll(Vector3[] positions)
	{
		Cell cell;

		for(int i = 0;i < positions.Length;i++)
		{
			cell = getCellUnderPoint(positions[i]);

			if(cell == null)
			{
				return false;
			}
			else
			{
				if(cell.occupied || !cell.canPositionateOnThisCell())
				{
					return false;
				}
			}
		}
		return true;
	}
		
	/**
	 * Devuelve las celdas debajo de una pieza
	 **/ 
	public List<Cell> getCellsUnderPiece(Piece piece)
	{
		List<Cell> result = new List<Cell>();

		for(int i = 0;i < piece.pieces.Length;i++)
		{
			result.Add(getCellUnderPoint(piece.pieces[i].transform.position));
		}

		return result;
	}

	/**
	 * Ocupa la celda indicada y le asigna el contenido y tipo indicado
	 **/ 
	public void occupyAndConfigureCell(int cellIndex,GameObject content, EPieceType type,bool positionate = false)
	{
		occupyAndConfigureCell(cells[cellIndex],content,type,positionate);
	}

	/**
	 * Ocupa la celda indicada y le asigna el contenido y tipo indicado
	 **/ 
	public void occupyAndConfigureCell(Cell cell,GameObject content, EPieceType type,bool positionate = false)
	{
		cell.occupied = true;

		setCellContent(cell, content, true,positionate);//destroy
		setCellType(cell, type);
	}

	/**
	 * Cambia el contenido de la celda y destruye el anterior si se necesita y posiciona si se necesita
	 **/ 
	protected void setCellContent(int cellIndex,GameObject content, bool destroyOldContent = true, bool positionate = true)
	{
		setCellContent(cells[cellIndex],content,destroyOldContent,positionate);
	}

	/**
	 * Cambia el contenido de la celda y destruye el anterior si se necesita y posiciona si se necesita
	 **/ 
	protected void setCellContent(int x, int y,GameObject content, bool destroyOldContent = true, bool positionate = true)
	{
		setCellContent(getCellAt(x,y),content,destroyOldContent,positionate);
	}

	/**
	 * Cambia el contenido de la celda y destruye el anterior si se necesita y posiciona si se necesita
	 **/ 
	protected void setCellContent(Cell cell,GameObject content, bool destroyOldContent = true, bool positionate = true)
	{
		if(destroyOldContent)
		{
			DestroyImmediate(cell.content);
		}

		cell.content = content;

		if (positionate) 
		{
			content.transform.position = cell.transform.position + (new Vector3 (cell.GetComponent<SpriteRenderer> ().bounds.extents.x,
				-cell.GetComponent<SpriteRenderer> ().bounds.extents.x, 0));
			
		}
	}

	public void setCellType(int cellIndex,EPieceType type)
	{
		setCellType(cells[cellIndex],type);
	}
		
	public void setCellType(int x, int y, EPieceType type)
	{
		setCellType(getCellAt(x,y),type);
	}

	public void setCellType(Cell cell, EPieceType type)
	{
		cell.pieceType = type;
	}

	public void setCellToType(int cellIndex, int cellType)
	{
		setCellToType (cells [cellIndex], cellType);
	}

	public void setCellToType(Cell cell, int cellType)
	{
		cell.setTypeToCell (cellType);
	}

	public void turnPieceToLetterByWinNotification(Cell cell)
	{
		cell.destroyCell ();
		Transform tempTransform = cell.transform;

		GameObject go = Instantiate (uiLetter)as GameObject;


		go.transform.SetParent (GameObject.Find("CanvasOfLetters").transform,false);

		Vector3 nVec = new Vector3(cell.transform.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
			-cell.transform.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);

		go.GetComponent<RectTransform> ().transform.position = tempTransform.position + nVec;
		cell.content = go;

		ABCChar tempAbcChar = cell.content.GetComponent<ABCChar>();

		UIChar tempUiChar = cell.content.GetComponent<UIChar>();

		cell.content.GetComponent<BoxCollider2D>().enabled = true;

		if(OnLetterCreated != null)
		{
			OnLetterCreated(tempAbcChar,tempUiChar,false);
		}
	}


	/*
	 * Analiza si aun es posible colocar alguna de las piezas disponibles en la grid
	 * 
	 * @params listPieces{List<GameObject>}: Objetos a evaluar
	 * 
	 * @return true: Si una cabe. false: Si ninguna cabe
	 */
	public bool checkIfOneCanFit(List<Piece> piecesList)
	{
		Vector3 offset = Vector3.zero;
		float extentsX = cellPrefab.gameObject.GetComponent<SpriteRenderer>().bounds.extents.x;
		Vector3 moveLittle = new Vector3(extentsX,-extentsX,0);
		Vector3[] vecArr;

		foreach(Cell val in cells)
		{
			if(!val.occupied && val.canPositionateOnThisCell())
			{
				for(int i = 0;i < piecesList.Count;i++)
				{
					
					offset = (val.transform.position + moveLittle) - piecesList[i].pieces[0].transform.position;
					vecArr = new Vector3[piecesList[i].pieces.Length];
					for(int j = 0;j < piecesList[i].pieces.Length;j++)
					{
						vecArr[j] = piecesList[i].pieces[j].transform.position + offset;
					}
					if(canPositionateAll(vecArr))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	/*
	 * La funcion evalua de que manera es en la que se deben de seleccionar por color las celdas,
	 * esto en lo que se prueban los PowerUps, existiran seleccion de coplores en vecinos y de colores en toda la grid
	 * 
	 * @params cell{Cell}: La celda que se usara como base para tomar su color y buscar las demas
	 */
	public Cell[] getCellNeighborsOfSameType(Cell cell)
	{
		List<Cell> finalList = new List<Cell>();
		List<Cell> pendingList = new List<Cell>();

		pendingList.Add(cell);
			
		while(pendingList.Count > 0)
		{
			searchNeigboursOfSameType(pendingList[0],ref finalList,ref pendingList);
		}

		return finalList.ToArray();
	}


	/*
	 * Busca la celdas que sean del mismo color en toda la grid y los agrega a 'selected'
	 * 
	 * @params cell{Cell}: Celda de la que se tomara su color comop parametro para evaluar
	 */
	public Cell[] getCellsOfSameType(Cell cell)
	{
		return getCellsOfSameType(cell.pieceType);
	}

	public Cell[] getCellsOfSameType(EPieceType cellType)
	{
		List<Cell> selection = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].pieceType == cellType)
			{
				selection.Add(cells[i]);
			}
		}
		return selection.ToArray();
	}

	/*
	 * Activa los collider de las piezas que estan colocadas en la grid
	 */
	public void activatePositionedPiecesCollider()
	{
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].content != null)
			{
				cells[i].content.GetComponent<Collider2D>().enabled = true;
			}
		}
	}

	/*
	 * Desactiva los colider de las piezas que estan colocadas en el grid
	 */
	public void deactivatePositionedPiecesCollider()
	{
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].content != null)
			{
				cells[i].content.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public void destroyCell(Cell cell)
	{
		cell.destroyCell ();
	}

	/*
	 * Destruye todas las celdas que estan en la lista de 'selected'
	 */
	public void destroyCells(Cell[] cells)
	{
		for(int i = 0;i < cells.Length;i++)
		{
			cells[i].destroyCell();
		}
	}

	/*
	 * Busca celdas del mismo color en sus vecinos verticales y horizontales
	 * 
	 * @params cell{Cell}: La celda de la que se evaluaran los vecinos
	 * 
	 * @params final{List<Cell>}: En esta lista se agregan las celdas a las que ya se les evaluo el vecino
	 * 
	 * @params pending{List<Cell>}: En esta lista se van agregando las celdas que tienen el mismo color, pero que aun no han sido evaluadas
	 */
	protected void searchNeigboursOfSameType(Cell cell,ref List<Cell> final, ref List<Cell> pending)
	{
		int cX = cells.IndexOf(cell)%columns;
		int cY = cells.IndexOf(cell)/columns;
		Cell tempC = null;

		tempC = getCellAt(cX,cY-1);
		if(tempC != null)
		{
			if(tempC.pieceType == cell.pieceType && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX-1,cY);
		if(tempC != null)
		{
			if(tempC.pieceType == cell.pieceType && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX+1,cY);
		if(tempC != null)
		{
			if(tempC.pieceType == cell.pieceType && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX,cY+1);
		if(tempC != null)
		{
			if(tempC.pieceType == cell.pieceType && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		pending.Remove(cell);
		final.Add(cell);
	}

	public Cell[] getAllEmptyCells()
	{
		List<Cell> result = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(!cells[i].occupied)
			{
				result.Add(cells[i]);
			}
		}
		return result.ToArray();
	}

	public EPieceType colorOfMoreQuantity()
	{
		int[] quantity = new int[8];
		int index = -1;
		int amount = 0;

		for(int i = 0;i < cells.Count;i++)
		{
			switch(cells[i].pieceType)
			{
			case EPieceType.AQUA:
				quantity[0]++;
				break;
			case EPieceType.BLACK:
				quantity[1]++;
				break;
			case EPieceType.BLUE:
				quantity[2]++;
				break;
			case EPieceType.GREEN:
				quantity[3]++;
				break;
			case EPieceType.GREY:
				quantity[4]++;
				break;
			case EPieceType.MAGENTA:
				quantity[5]++;
				break;
			case EPieceType.RED:
				quantity[6]++;
				break;
			case EPieceType.YELLOW:
				quantity[7]++;
				break;
			}
		}

		for(int i = 0;i < quantity.Length;i++)
		{
			if(quantity[i] > amount)
			{
				index = i;
				amount = quantity[i];
			}
		}

		//para regresar un color valido
		if(index == -1)
		{
			index = Random.Range (0, 8);
		}

		EPieceType result = EPieceType.NONE;

		switch(index)
		{
		case 0:
			result = EPieceType.AQUA;
			break;
		case 1:
			result = EPieceType.BLACK;
			break;
		case 2:
			result = EPieceType.BLUE;
			break;
		case 3:
			result = EPieceType.GREEN;
			break;
		case 4:
			result = EPieceType.GREY;
			break;
		case 5:
			result = EPieceType.MAGENTA;
			break;
		case 6:
			result = EPieceType.RED;
			break;
		case 7:
			result = EPieceType.YELLOW;
			break;
		}
		return result;
	}
}