﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABC;

public class CellsManager : MonoBehaviour 
{
	public GameObject cellPrefab;
	//public GameObject obstacleLetterPrefab;
	//public GameObject singleSquarePiece;
	public GameObject uiLetter;

	public Transform topOfScreen;
	public Transform bottomOfScreen;

	[HideInInspector]public int columns = 0;
	[HideInInspector]public int rows = 0;
	[HideInInspector]public float cellSize;

	protected float cellScalePercentage = 0.059f;

	protected float percentOfTheCellForInnerRect = 0.05f;

	public Color firstCellCollor = new Color (0.39f, 0.79f, 0.81f);
	public Color secondCellCollor = new Color (0.42f, 0.83f, 0.87f);
	public Frame frame;

	//Todas las celdas del grid
	protected List<Cell> cells;

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.K)) 
		{
			getAvailableVerticalAndHorizontalLines ();
		}
	}

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
			cellSize = cellPrefab.GetComponent<SpriteRenderer>().sprite.rect.height;
			float screenHeight = ((topOfScreen.position - bottomOfScreen.position).magnitude)*100;
			float cellScale = (screenHeight*cellScalePercentage) / cellSize;
			cellSize = screenHeight * (cellScalePercentage*0.01f);
			float gap = cellSize * 0.1f;
			gap = 0;

			for(int i = 0;i < _rows;i++)
			{
				for(int j = 0;j < _columns;j++)
				{
					cellInstance = GameObject.Instantiate(cellPrefab,cellInitialPosition,Quaternion.identity) as GameObject;
					cellInstance.GetComponent<SpriteRenderer> ().sortingOrder = -1;
					cellInstance.transform.SetParent(transform);
					cellInstance.name = cells.Count.ToString ();
					cells.Add(cellInstance.GetComponent<Cell>());
					cellInstance.transform.localScale = new Vector3 (cellScale,cellScale,cellScale);
					
					cellInitialPosition.x += cellSize + gap;

					if(i % 2 == 0)
					{
						if(j % 2 == 0 )
						{
							cellInstance.GetComponent<SpriteRenderer> ().color = firstCellCollor;
						}
						else
						{
							cellInstance.GetComponent<SpriteRenderer> ().color = secondCellCollor;
						}
					}
					else
					{
						if(j % 2 == 0 )
						{
							cellInstance.GetComponent<SpriteRenderer> ().color = secondCellCollor;
						}
						else
						{
							cellInstance.GetComponent<SpriteRenderer> ().color = firstCellCollor;
						}
					}
				}
				cellInitialPosition.y -= cellSize + gap;
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
			setAllCellsToType(Piece.EType.NONE);
		}
	}
	
	public void resetGrid()
	{
		setAllCellsToType(Piece.EType.NONE);
	}

	public void setAllCellsToType(Piece.EType type)
	{
		for(int i = 0;i < rows;i++)
		{
			for(int j = 0;j < columns;j++)
			{
				//[TODO] asignarle tipo
				getCellAt(j,i).setType();
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
	public Cell getCellUnderPoint(Vector3 point,bool useOffset = false)
	{
		float offset;
		SpriteRenderer spriteRenderer;

		foreach(Cell cell in cells)
		{
			spriteRenderer	= cell.gameObject.GetComponent<SpriteRenderer>();
			point.z = spriteRenderer.bounds.center.z;

			offset = spriteRenderer.bounds.size.x * percentOfTheCellForInnerRect;

			if (useOffset) 
			{
				if (spriteRenderer.bounds.Contains (new Vector3 (point.x - offset, point.y + offset, point.z)) &&
				    spriteRenderer.bounds.Contains (new Vector3 (point.x + offset, point.y - offset, point.z))) 
				{
					return cell;
				}
			}
			else 
			{
				if (spriteRenderer.bounds.Contains (point)) 
				{
					return cell;
				}
			}
		}

		return null;
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
		bool keepAdding = false;
		bool nothingFound = true;	
		bool addRow = false;

		for(int y = 0; y < rows; y++)
		{
			result.Add(new List<Cell>(columns));
			keepAdding = true;
			nothingFound = true;
			addRow = true;

			for(int x = 0; x < columns; x++)
			{
				cell = getCellAt(x,y);
				if(cell.occupied && cell.contentType != Piece.EType.LETTER && cell.contentType != Piece.EType.LETTER_OBSTACLE)
				{
					if(cell.content != null && keepAdding)
					{
						result [result.Count - 1].Add (cell);
						addRow = false;
						nothingFound = false;
					}
					if(cell.contentType == Piece.EType.NONE)
					{
						//cell.name = "null";
						if(!addRow)
						{
							result.Add(new List<Cell>(columns));
						}
						keepAdding = true;
						addRow = true;
						nothingFound = true;
					}
				}
				else
				{
					if(keepAdding)
					{
						if(cell.available || cell.contentType == Piece.EType.LETTER || cell.contentType == Piece.EType.LETTER_OBSTACLE)
						{
							keepAdding = false;
							result.RemoveAt(result.Count-1);

							nothingFound = false;
							addRow = false;
						}
					}
				}
			}
			if(nothingFound)
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
		bool keepAdding = false;
		bool nothingFound = true;	
		bool addRow = false;

		for(int x = 0; x < columns; x++)
		{
			result.Add(new List<Cell>(rows));

			keepAdding = true;
			nothingFound = true;
			addRow = true;

			for(int y = 0; y < rows; y++)
			{
				cell = getCellAt(x,y);
				if(cell.occupied && cell.contentType != Piece.EType.LETTER && cell.contentType != Piece.EType.LETTER_OBSTACLE)
				{
					if(cell.content != null && keepAdding)
					{
						result [result.Count - 1].Add (cell);
						addRow = false;
						nothingFound = false;
					}
					if(cell.contentType == Piece.EType.NONE)
					{
						//cell.name = "null";
						if(!addRow)
						{
							result.Add(new List<Cell>(columns));
						}
						keepAdding = true;
						addRow = true;
						nothingFound = true;
					}
				}
				else
				{
					if(keepAdding)
					{
						if(cell.available || cell.contentType == Piece.EType.LETTER || cell.contentType == Piece.EType.LETTER_OBSTACLE)
						{
							keepAdding = false;
							result.RemoveAt(result.Count-1);

							nothingFound = false;
							addRow = false;
						}
					}
				}
			}
			if(nothingFound)
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

	public List<List<Cell>> getAvailableHorizontalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>(rows);
		Cell cell;
		bool keepAdding = false;
		bool nothingFound = true;	
		bool addRow = false;

		for(int y = 0; y < rows; y++)
		{
			result.Add(new List<Cell>(columns));
			keepAdding = true;
			nothingFound = true;
			addRow = true;

			for(int x = 0; x < columns; x++)
			{
				cell = getCellAt(x,y);
				if(!cell.occupied && cell.available)
				{
					if(cell.content == null && keepAdding)
					{
						result [result.Count - 1].Add (cell);
						addRow = false;
						nothingFound = false;
					}
					if(cell.contentType != Piece.EType.NONE)
					{
						//cell.name = "null";
						if(!addRow)
						{
							result.Add(new List<Cell>(columns));
						}
						keepAdding = true;
						addRow = true;
						nothingFound = true;
					}
				}
				else
				{
					if(keepAdding)
					{
						if(cell.contentType == Piece.EType.LETTER || cell.contentType == Piece.EType.LETTER_OBSTACLE || cell.contentType == Piece.EType.PIECE)
						{
							keepAdding = false;
							result.RemoveAt(result.Count-1);

							nothingFound = false;
							addRow = false;
						}
					}
				}
			}
			if(nothingFound)
			{
				result.RemoveAt(result.Count-1);

			}
		}

		return result;
	}

	public List<List<Cell>> getAvailableVerticalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>(columns);
		Cell cell;
		bool keepAdding = false;
		bool nothingFound = true;	
		bool addRow = false;

		for(int x = 0; x < columns; x++)
		{
			result.Add(new List<Cell>(rows));

			keepAdding = true;
			nothingFound = true;
			addRow = true;

			for(int y = 0; y < rows; y++)
			{
				cell = getCellAt(x,y);
				if(!cell.occupied && cell.available)
				{
					if(cell.content == null && keepAdding)
					{
						result [result.Count - 1].Add (cell);
						addRow = false;
						nothingFound = false;
					}
					if(cell.contentType != Piece.EType.NONE)
					{
						//cell.name = "null";
						if(!addRow)
						{
							result.Add(new List<Cell>(columns));
						}
						keepAdding = true;
						addRow = true;
						nothingFound = true;
					}
				}
				else
				{
					if(keepAdding)
					{
						if(cell.contentType == Piece.EType.LETTER || cell.contentType == Piece.EType.LETTER_OBSTACLE || cell.contentType == Piece.EType.PIECE)
						{
							keepAdding = false;
							result.RemoveAt(result.Count-1);

							nothingFound = false;
							addRow = false;
						}
					}
				}
			}
			if(nothingFound)
			{
				result.RemoveAt(result.Count-1);
			}
		}

		return result;
	}

	public List<List<Cell>> getAvailableVerticalAndHorizontalLines()
	{
		List<List<Cell>> result = new List<List<Cell>>();

		result.AddRange(getAvailableHorizontalLines());
		result.AddRange(getAvailableVerticalLines());

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
		Vector3[] positions = new Vector3[objects.Length];
		for(int i = 0;i < objects.Length;i++)
		{
			positions[i] = objects[i].transform.position;
		}

		return canPositionateAll(positions);
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
	 * Devuelve las celdas debajo de una pieza.
	 * Si se le indica breakIfEmpty = true, detiene la busqueda en cuanto 
	 * no se encuentra una celda debajo
	 **/ 
	public List<Cell> getCellsUnderPiece(Piece piece, bool breakIfEmpty = true)
	{
		List<Cell> result = new List<Cell>();
		Cell cell;

		for(int i = 0;i < piece.squares.Length;i++)
		{
			cell = getCellUnderPoint(piece.squares[i].transform.position,true);
			if(cell != null)
			{
				result.Add(cell);		
			}
			else if(breakIfEmpty)
			{
				break;
			}
		}

		return result;
	}

	/**
	 * Devuelve las celdas bajo la pieza que esten libres para posicionarse
	 **/ 
	public List<Cell> getFreeCellsUnderPiece(Piece piece)
	{
		List<Cell> result = getCellsUnderPiece(piece,true);

		for(int i = result.Count-1;i >= 0 ;i--)
		{
			if(result[i].occupied || !result[i].canPositionateOnThisCell())
			{
				result.RemoveAt(i);	
			}
		}

		return result;
	}

	/**
	 * Ocupa la celda indicada y le asigna el contenido y tipo indicado
	 **/ 
	public void occupyAndConfigureCell(int cellIndex,GameObject content, Piece.EType type, Piece.EColor color,bool positionate = false)
	{
		occupyAndConfigureCell(cells[cellIndex],content,type,color,positionate);
	}

	/**
	 * Ocupa la celda indicada y le asigna el contenido y tipo indicado
	 **/ 
	public void occupyAndConfigureCell(Cell cell,GameObject content, Piece.EType type, Piece.EColor color,bool positionate = false)
	{
		cell.occupied = true;

		setCellContent(cell, content, true,positionate);//destroy
		setCellContentType (cell, type);
		setCellContentColor (cell, color);
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

	public void setCellContentType(int cellIndex,Piece.EType type)
	{
		setCellContentType(cells[cellIndex],type);
	}
		
	public void setCellContentType(int x, int y, Piece.EType type)
	{
		setCellContentType(getCellAt(x,y),type);
	}

	public void setCellContentType(Cell cell, Piece.EType type)
	{
		cell.contentType = type;
	}

	public void setCellContentColor(int cellIndex,Piece.EColor color)
	{
		setCellContentColor(cells[cellIndex],color);
	}

	public void setCellContentColor(int x, int y, Piece.EColor color)
	{
		setCellContentColor(getCellAt(x,y),color);
	}

	public void setCellContentColor(Cell cell, Piece.EColor color)
	{
		cell.contentColor = color;
	}

	public void setCellType(int cellIndex, int cellType)
	{
		setCellType (cells [cellIndex], cellType);
	}

	public void setCellType(Cell cell, int cellType)
	{
		cell.setType (cellType);
	}

	/*
	 * Analiza si aun es posible colocar alguna de las piezas disponibles en la grid
	 * 
	 * @params listPieces{List<GameObject>}: Objetos a evaluar
	 * 
	 * @return true: Si una cabe. false: Si ninguna cabe
	 */
	public bool checkIfOnePieceCanFit(List<Piece> piecesList)
	{
		Vector3 offset = Vector3.zero;
		float extentsX = cellSize * 0.5f;
		float betweenSquaresDist = (piecesList [0].squares [0].transform.position - piecesList [0].squares [1].transform.position).magnitude;
		Vector3 moveLittle = new Vector3(extentsX,-extentsX,0);
		Vector3[] vecArr;

		float percent = ((extentsX * 200) / betweenSquaresDist) * 0.01f;


		foreach(Cell val in cells)
		{
			if(!val.occupied && val.canPositionateOnThisCell())
			{
				for(int i = 0;i < piecesList.Count;i++)
				{
					offset = (val.transform.position + moveLittle) - piecesList[i].squares[0].transform.position;

					vecArr = new Vector3[piecesList[i].squares.Length];

					vecArr[0] = piecesList[i].squares[0].transform.position + offset;
					for(int j = 1;j < piecesList[i].squares.Length;j++)
					{
						vecArr[j] = (((piecesList[i].squares[j].transform.position -
							piecesList[i].squares[0].transform.position) * percent) +
							piecesList[i].squares[0].transform.position) + offset;
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
	public Cell[] getCellNeighborsOfSameColor(Cell cell)
	{
		List<Cell> finalList = new List<Cell>();
		List<Cell> pendingList = new List<Cell>();

		pendingList.Add(cell);
			
		while(pendingList.Count > 0)
		{
			searchNeigboursOfSameColor(pendingList[0],ref finalList,ref pendingList);
		}

		return finalList.ToArray();
	}

	public Vector2 getCellXYPosition(Cell cell)
	{
		return new Vector2 (cells.IndexOf(cell)%columns,cells.IndexOf(cell)/columns);
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
	protected void searchNeigboursOfSameColor(Cell cell,ref List<Cell> final, ref List<Cell> pending)
	{
		Vector2 cellXY = getCellXYPosition (cell);
		int cX = (int)cellXY.x;
		int cY = (int)cellXY.y;
		Cell tempC = null;

		tempC = getCellAt(cX,cY-1);
		if(tempC != null)
		{
			if(tempC.contentColor == cell.contentColor && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX-1,cY);
		if(tempC != null)
		{
			if(tempC.contentColor == cell.contentColor && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX+1,cY);
		if(tempC != null)
		{
			if(tempC.contentColor == cell.contentColor && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX,cY+1);
		if(tempC != null)
		{
			if(tempC.contentColor == cell.contentColor && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		pending.Remove(cell);
		final.Add(cell);
	}


	/*
	 * Busca la celdas que sean del mismo tipo en toda la grid y los agrega a 'selected'
	 * 
	 * @params cell{Cell}: Celda de la que se tomara su color comop parametro para evaluar
	 */
	public Cell[] getCellsOfSameType(Cell cell)
	{
		return getCellsOfSameType(cell.contentType);
	}

	public Cell[] getCellsOfSameType(Piece.EType cellType)
	{
		List<Cell> selection = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].contentType == cellType)
			{
				selection.Add(cells[i]);
			}
		}
		return selection.ToArray();
	}

	/*
	 * regresa todas las celdas
	 */
	public Cell[] getAllShowedCels()
	{
		List<Cell> selection = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].cellType != Cell.EType.EMPTY_VISIBLE_CELL && cells[i].cellType != Cell.EType.EMPTY)
			{
				selection.Add(cells[i]);
			}
		}
		return selection.ToArray();
	}

	/*
	 * Busca la celdas que sean del mismo color en toda la grid y los agrega a 'selected'
	 * 
	 * @params cell{Cell}: Celda de la que se tomara su color comop parametro para evaluar
	 */
	public Cell[] getCellsOfSameColor(Cell cell)
	{
		return getCellsOfSameColor(cell.contentColor);
	}

	public Cell[] getCellsOfSameColor(Piece.EColor cellColor)
	{
		List<Cell> selection = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].contentColor == cellColor)
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

	public Piece.EColor colorRandom()
	{
		//HACK: Este rango existe por combinar tipos y colores
		return (Piece.EColor)Random.Range (1, 6);
	}

	public Piece.EType getPredominantColor()
	{		
		int[] quantity = new int[System.Enum.GetNames(typeof(Piece.EType)).Length];
		int index;
		int amount;

		for(int i = 0;i < cells.Count;i++)
		{
			quantity[(int)cells[i].contentType]++;
		}

		amount = 0;
		index = 0;
		for(int i = 0;i < quantity.Length;i++)
		{
			if ((Piece.EType)i != Piece.EType.NONE && (Piece.EType)i != Piece.EType.LETTER) 
			{
				if (quantity [i] > amount) 
				{
					index = i;
					amount = quantity [i];
				}
			}
		}

		//HACK: Este limite existe por combinar colores y tipo
		if(index >8 )
		{
			return Piece.EType.NONE;
		}

		return (Piece.EType)index;

	}

	public bool existType(Piece.EType type)
	{
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].contentType == type)
			{
				return true;
			}
		}
		return false;
	}

	public void createFrame()
	{
		bool left = false;
		bool right = false;
		bool top = false;
		bool bottom = false;

		for (int i = 0; i < rows; i++) 
		{
			for (int j = 0; j < columns; j++) 
			{
				left = false;
             	right = false;
             	top = false;
				bottom = false;

				if(cells[(i * rows) + j].cellType != Cell.EType.EMPTY)
				{
					//vecino de arriba
					if( i== 0 || (i != 0 && cells[((i * rows) + j)-rows].cellType == Cell.EType.EMPTY))
					{
						frame.instanceFrames (frame.top, cells [(i * rows) + j].transform);
						//print ("marco arriba " + ((i * rows) + j));
						top = true;
					}
					//vecino izquierdo
					if(j== 0 ||(j != 0 && cells[((i * rows) + j)-1].cellType == Cell.EType.EMPTY))
					{
						frame.instanceFrames (frame.left, cells [(i * rows) + j].transform);
						//print ("marco izquierdo " + ((i * rows) + j));
						left = true;
					}
					//vecino derecho
					if(j== columns-1 || (j < columns+1 && cells[((i * rows) + j)+1].cellType == Cell.EType.EMPTY))
					{
						frame.instanceFrames (frame.right, cells [(i * rows) + j].transform);
						if(top)
						{
							frame.instanceFrames (frame.rightTopShadow, cells [(i * rows) + j].transform,-1);
						}
						else
						{
							frame.instanceFrames (frame.rightShadow, cells [(i * rows) + j].transform,-1);
						}
						//print ("marco derecho " + ((i * rows) + j));
						right = true;
					}
					//vecino de abajo
					if(i == rows-1 ||(i < rows +1  && cells[((i * rows) + j)+rows].cellType == Cell.EType.EMPTY))
					{
						frame.instanceFrames (frame.bottom, cells [(i * rows) + j].transform);
						if(left)
						{
							frame.instanceFrames (frame.bottonLeftShadow, cells [(i * rows) + j].transform,-1);
						}
						else
						{							
							frame.instanceFrames (frame.bottonShadow, cells [(i * rows) + j].transform,-1);
						}
						//print ("marco abajo " + ((i * rows) + j));
						bottom = true;
					}

					if(left && top)
					{
						frame.instanceFrames (frame.leftTop, cells [(i * rows) + j].transform);
					}

					if(top && right)
					{
						frame.instanceFrames (frame.topRight, cells [(i * rows) + j].transform);
					}

					if(right && bottom)
					{
						frame.instanceFrames (frame.rightBottom, cells [(i * rows) + j].transform);
						frame.instanceFrames (frame.bottonRightShadow, cells [(i * rows) + j].transform,-1);
					}

					if (bottom && left)
					{
						frame.instanceFrames (frame.bottomLeft, cells [(i * rows) + j].transform);
					}
				}
			}
		}
	}


}