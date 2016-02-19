using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ABC;

//[ExecuteInEditMode]
public class CellsManager : MonoBehaviour 
{
	public delegate void linesCreated(int lines);
	[HideInInspector]public linesCreated OnlinesCounted;

	public delegate void letterCreated(ABCChar abcChar,UIChar uiChar,bool isBlackLetter);
	[HideInInspector]public letterCreated OnLetterCreated;

	public GameObject cellPrefab;
	public GameObject obstacleLetterPrefab;
	public GameObject singleSquarePiece;

	//Medidas del grid
	public int matrixWidth;
	public int matrixHeight;

	public int pointPerLine = 10;


	//Bandera para el powerUp de Destruccion por color
	//false: destruye todos los del mismo color en la escena
	//true: destruye todos los del mismo color que este juntos
	public bool selectNeighbors;

	//Las celdas seleccionadas por color
	protected List<Cell> selected = new List<Cell>();
	//Todas las celdas del grid
	protected List<Cell> cells = new List<Cell>();

	protected int totalLinesCreated;

	public GameObject uiLetter;

	void Start () 
	{
		CreateGrid(10,10,PersistentData.instance.currentLevel.grid);
	}

	/*
	 * Se crea la grid y se asigna cada celda como hijo de este gameObject.
	 * Las celdas tienen un espacio de 3% de su tamaño de espacio en tre ellas
	 */
	protected void CreateGrid(int width, int height,string cellsMatrix)
	{
		Vector3 nPos = transform.position;
		GameObject go = null;
		string[] levelGridData = cellsMatrix.Split(',');

		for(int i = 0;i < height;i++)
		{
			for(int j = 0;j < width;j++)
			{
				go = GameObject.Instantiate(cellPrefab,nPos,Quaternion.identity) as GameObject;
				go.GetComponent<SpriteRenderer> ().sortingOrder = -1;
				go.transform.SetParent(transform);
				cells.Add(go.GetComponent<Cell>());

				cells[cells.Count-1].setTypeToCell(int.Parse(levelGridData[cells.Count-1]));

				if(cells[cells.Count-1].pieceType == EPieceType.LETTER_OBSTACLE)
				{
					turnPiecesToBlackLetters(cells[cells.Count-1]);
				}
				else if(cells[cells.Count-1].pieceType != EPieceType.NONE)
				{
					addBlockToInitialOccupiedCell(cells[cells.Count-1]);
				}

				nPos.x += cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x + 0.03f;
			}
			nPos.y -= cellPrefab.GetComponent<SpriteRenderer>().bounds.size.y + 0.03f;
			nPos.x = transform.position.x;
		}
	}

	/*
	 * Destruye los objetos del Grid y limpia las celdas
	 */
	protected void DestroyGrid()
	{
		foreach(Cell val in cells)
		{
			DestroyImmediate(val.gameObject);
		}
		cells.Clear();
	}
	public void resetGrid(int columns, int rows)
	{
		matrixWidth = columns;
		matrixHeight = rows;
		
		resetGrid();
	}
	
	public void resetGrid()
	{
		DestroyGrid();
		CreateGrid(10,10,PersistentData.instance.currentLevel.grid);
	}

	/*
	 * Regresa la celda dependiendo la posicion del mundo que se le manda
	 * 
	 * @params vec{Vector3}: Un punto en el mundo del cual se toman su 'x' y su 'y' para buscar la celda
	 * 
	 * @return {Cell}: La celda que tiene la posicion de mundo que se le envio dentro de sus limites
	 */
	public Cell getCellOnVec(Vector3 vec)
	{
		Vector3 tempV2 = Vector3.zero;
		float size = 0;

		foreach(Cell val in cells)
		{
			tempV2 = val.transform.position;
			size = val.gameObject.GetComponent<SpriteRenderer>().bounds.size.x;
			if(vec.x > tempV2.x && vec.x < (tempV2.x + size) &&
			   vec.y < tempV2.y && vec.y > (tempV2.y - size))
			{
				return val;
			}
		}
		return null;
	}

	/*
	 * Regresa una Celda dependiendo la posicion (x,y) dentro del grid
	 * 
	 * @params xPos{int}: Posicion 'x' del grid para buscar la celda
	 * @params yPos{int}: Posicion 'y' del grid para buscar la celda
	 */
	protected Cell getCellAt(int xPos,int yPos)
	{
		if(xPos < 0 || yPos < 0 || xPos >= matrixWidth || yPos >= matrixHeight)
		{
			return null;
		}
		return cells[(matrixWidth*yPos)+xPos];
	}

	public Cell[] evaluateHorizontalLines()
	{
		int[] widthCount = new int[matrixWidth];
		int wIndex = 0;

		List<Cell> result = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].occupied && cells[i].pieceType != EPieceType.LETTER && cells[i].available)
			{
				widthCount[wIndex]++;
			}
			if(wIndex == matrixWidth)
			{
				wIndex++;
			}
		}
		for(int i = 0;i < matrixHeight;i++)
		{
			if(widthCount[i] == matrixWidth)
			{
				for(int j = (i*matrixWidth);j < (matrixWidth*(i+1));j++)
				{
					result.Add(cells[j]);
				}
				totalLinesCreated++;
			}
		}

		notifyLinesCount();

		return result.ToArray();
	}

	public Cell[] evaluateVerticalLines()
	{
		int[] heightCount = new int[matrixHeight];

		int hIndex = 0;

		List<Cell> result = new List<Cell>();

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].occupied && cells[i].pieceType != EPieceType.LETTER && cells[i].available)
			{
				heightCount[hIndex]++;
			}
			hIndex ++;
			if(hIndex == matrixWidth)
			{
				hIndex = 0;
			}
		}

		for(int i = 0;i < matrixWidth;i++)
		{
			if(heightCount[i] == matrixHeight)
			{
				for(int j = 0;j < matrixHeight;j++)
				{
					result.Add(cells[j*matrixHeight]);
				}
				totalLinesCreated++;
			}
		}

		notifyLinesCount();

		return result.ToArray();
	}

	/*
	 * Evalua el estado de la grid para determinar si se ha creado o no una linea de piezas
	 */
	public Cell[] evaluateVerticalAndHorizontalLines()
	{
		List<Cell> result = new List<Cell>();

		result.AddRange(evaluateHorizontalLines());

		result.AddRange(evaluateVerticalLines());

		return result.ToArray();
	}

	public void notifyLinesCount()
	{
		if(OnlinesCounted != null)
		{
			OnlinesCounted(totalLinesCreated);
			totalLinesCreated = 0;
		}
	}

	/*
	 * Evalua si la figura se puede colocar donde se esta soltando la figura
	 * 
	 * @params arr{GameObject[]}: El arreglo de las piezas que conforman la figura. 
	 * 							  Estas piezas se evaluaran de manera individual para saber si la pieza puede ser posicionada
	 * 
	 * @return {bool}: Regresa verdadero si es posible colocarla, de lo contrario regresa falso
	 */
	public bool CanPositionate(GameObject[] arr)
	{
		Cell tempC = null;

		for(int i = 0;i < arr.Length;i++)
		{
			tempC = getCellOnVec(arr[i].transform.position);
			if(tempC == null)
			{
				return false;
			}
			else
			{
				if(tempC.occupied)
				{
					return false;
				}
				if(!tempC.canPositionateOnThisCell())
				{
					return false;
				}
			}
		}
		return true;
	}
	
	/*
	 * Evalua si la figura se puede colocar donde se esta soltando la figura
	 * 
	 * @params arr{Vecto3[]}: El arreglo de las posiciones de las piezas que conforman la figura. 
	 * 							  Estas piezas se evaluaran de manera individual para saber si la pieza puede ser posicionada
	 * 
	 * @return {bool}: Regresa verdadero si es posible colocarla, de lo contrario regresa falso
	 */
	public bool CanPositionate(Vector3[] arr)
	{
		Cell tempC = null;
		
		for(int i = 0;i < arr.Length;i++)
		{
			tempC = getCellOnVec(arr[i]);
			if(tempC == null)
			{
				return false;
			}
			else
			{
				if(tempC.occupied)
				{
					return false;
				}
				if(!tempC.canPositionateOnThisCell())
				{
					return false;
				}
			}
		}
		return true;
	}

	/*
	 * Coloca la pieza que se le envia en la celda que esta debajo de cada pieza.
	 * Asihgna la pieza a la celda, la ocupa y le cambia el color; todos los cambios para evaluar sobre las celdas y no sobre los objetos
	 * 
	 * @params piece{Piece}: La pieza que se colocara dentro e la grid. De esta misma se toman los datos para la celda
	 * 
	 * @return {Vector3}: La posicion del a celda a la cuaal se tiene que mover la pieza. Se regresa para separa las logicas del Tween de movimiento
	 * 					  con la asignacion de valores
	 */
	public Vector3 Positionate(Piece piece)
	{
		Cell tempC = null;

		for(int i = 0;i < piece.pieces.Length;i++)
		{
			tempC = getCellOnVec(piece.pieces[i].transform.position);
			tempC.occupied = true;
			tempC.piece=piece.pieces[i];
			tempC.pieceType = piece.currentType;
		}
		
		tempC = getCellOnVec(piece.pieces[0].transform.position);

		Vector3 ofsetBetweenPieces = piece.pieces[0].transform.position - piece.transform.position;

		Vector3 nVec = new Vector3(tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
		                           -tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);

		//***********************************************Crear AudioManager
		//gameObject.GetComponent<AudioSource>().Play();
		return (tempC).transform.position + nVec - ofsetBetweenPieces;
	}

	/*
	 * Cambia las piezas por letras de la linea que se le envia.
	 * 
	 * @params isHorizontal{bool}: Es una bandera para indicar si la linea que se cambiara es horizontal o no
	 * @params index{int}: Es el indice de en donde inicia la linea, este indice corresponde al de la lista de cells
	 */
	protected void createLettersOn(Cell[] cellsToTransform)
	{
		for(int i = 0;i < cellsToTransform.Length;i++)
		{
			turnPiecesToLetters(cellsToTransform[i]);
		}
	}

	protected void turnPiecesToLetters(Cell newCell)
	{
		newCell.pieceType = EPieceType.LETTER;

		if(newCell.piece != null)
		{
			Transform tempTransform = newCell.transform;
		
			Destroy (newCell.piece);

			GameObject go = Instantiate (uiLetter)as GameObject;

			go.transform.SetParent (GameObject.Find("CanvasOfLetters").transform,false);

			Vector3 nVec = new Vector3(newCell.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
				-newCell.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);
			
			go.GetComponent<RectTransform> ().transform.position = tempTransform.position+ nVec;
			newCell.piece = go;

			ABCChar tempAbcChar = newCell.piece.GetComponent<ABCChar>();
			
			UIChar tempUiChar = newCell.piece.GetComponent<UIChar>();

			newCell.piece.GetComponent<BoxCollider2D>().enabled = true;

			go.GetComponent<BoxCollider2D>().size =  go.GetComponent<RectTransform> ().rect.size;

			if(OnLetterCreated != null)
			{
				OnLetterCreated(tempAbcChar,tempUiChar,false);
			}
			tempUiChar.isFromGrid = true;
		}
	}

	protected void turnPiecesToBlackLetters(Cell cell)
	{
		Transform tempTransform = cell.transform;

		//tempTransform.position = new Vector3 (tempTransform.position.x, tempTransform.position.y, 0);
		GameObject go = Instantiate (uiLetter)as GameObject;


		go.transform.SetParent (GameObject.Find("CanvasOfLetters").transform,false);

		Vector3 nVec = new Vector3(cell.transform.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
			-cell.transform.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);
		
		go.GetComponent<RectTransform> ().transform.position = tempTransform.position + nVec;
		cell.piece = go;

		ABCChar tempAbcChar = cell.piece.GetComponent<ABCChar>();

		UIChar tempUiChar = cell.piece.GetComponent<UIChar>();

		cell.piece.GetComponent<BoxCollider2D>().enabled = true;

		go.GetComponent<BoxCollider2D>().size =  go.GetComponent<RectTransform> ().rect.size;

		if(OnLetterCreated != null)
		{
			OnLetterCreated(tempAbcChar,tempUiChar,true);
		}
		tempUiChar.isFromGrid = true;
	}

	protected void addBlockToInitialOccupiedCell(Cell cell)
	{
		GameObject go = GameObject.Instantiate (singleSquarePiece) as GameObject;
		SpriteRenderer sprite = go.GetComponent<Piece> ().pieces [0].GetComponent<SpriteRenderer> ();

		Vector3 nVec = new Vector3 (sprite.bounds.size.x * 0.5f,
			-sprite.bounds.size.x * 0.5f, 0) + cell.transform.position;

		go.transform.position = nVec;

		go.GetComponent<Piece> ().currentType = cell.pieceType;

		go.transform.position = Positionate (go.GetComponent<Piece> ());

		go.GetComponent<BoxCollider2D> ().enabled = false;
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
		cell.piece = go;

		ABCChar tempAbcChar = cell.piece.GetComponent<ABCChar>();

		UIChar tempUiChar = cell.piece.GetComponent<UIChar>();

		cell.piece.GetComponent<BoxCollider2D>().enabled = true;

		if(OnLetterCreated != null)
		{
			OnLetterCreated(tempAbcChar,tempUiChar,false);
		}
	}


	/*
	 * Analiza si aun es posible colocar alguna de las piezas disponibles en la grid
	 * 
	 * @params listPieces{List<GameObject>}: La lista de las piezas restantes, disponibles para el jugador
	 * 
	 * @return {bool}: Regresa verdadero si aun se puede colocar una pieza, de lo contrario regresa falso
	 */
	public bool VerifyPosibility(List<GameObject> listPieces)
	{
		Vector3 ofset = Vector3.zero;
		float size = cellPrefab.gameObject.GetComponent<SpriteRenderer>().bounds.size.x *0.5f;
		Vector3 moveLittle = new Vector3(size,-size,0);
		Vector3[] vecArr;
		Piece lPPiece = null;

		foreach(Cell val in cells)
		{
			if(!val.occupied)
			{
				for(int i = 0;i < listPieces.Count;i++)
				{
					lPPiece = listPieces[i].GetComponent<Piece>();
					ofset = (val.transform.position + moveLittle) - lPPiece.pieces[0].transform.position;
					vecArr = new Vector3[lPPiece.pieces.Length];
					for(int j = 0;j < lPPiece.pieces.Length;j++)
					{
						vecArr[j] = lPPiece.pieces[j].transform.position + ofset;
					}
					if(CanPositionate(vecArr))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	/*
	 * La funcion busca la celda de la pieza que se le envia y manda a llamar a 
	 * selectCellOfColor con la celda indicada
	 * 
	 * @params cellPiece{GameObject}: El objeto que se busca en las celdas
	 */
	public void selectCellsOfColor(GameObject cellPiece)
	{
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].piece != null)
			{
				if(cells[i].piece.Equals(cellPiece))
				{
					selectCellsOfColor(cells[i]);
					return;
				}
			}
		}
	}

	/*
	 * La funcion evalua de que manera es en la que se deben de seleccionar por color las celdas,
	 * esto en lo que se prueban los PowerUps, existiran seleccion de coplores en vecinos y de colores en toda la grid
	 * 
	 * @params cell{Cell}: La celda que se usara como base para tomar su color y buscar las demas
	 */
	public void selectCellsOfColor(Cell cell)
	{
		List<Cell> finalList = new List<Cell>();
		List<Cell> pendingList = new List<Cell>();

		if(selectNeighbors)
		{
			pendingList.Add(cell);
				
			while(pendingList.Count > 0)
			{
				searchNeigboursOfSameColor(pendingList[0],ref finalList,ref pendingList);
			}
			selected = finalList;
		}
		else
		{
			searchCellsOfSameColor(cell);
		}
	}

	/*
	 * Activa los collider de las piezas que estan colocadas en la grid
	 */
	public void activatePositionedPiecesCollider()
	{
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].piece != null)
			{
				cells[i].piece.GetComponent<Collider2D>().enabled = true;
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
			if(cells[i].piece != null)
			{
				cells[i].piece.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	/*
	 * Destruye todas las celdas que estan en la lista de 'selected'
	 */
	public void destroySelectedCells()
	{
		for(int i = 0;i < selected.Count;i++)
		{
			selected[i].destroyCell();
		}

		selected = new List<Cell>();
	}

	public void turnSelectedCellsToLetters()
	{
		for(int i = 0;i < selected.Count;i++)
		{
			turnPiecesToLetters(selected[i]);
		}

		selected = new List<Cell>();
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
		int cX = cells.IndexOf(cell)%matrixWidth;
		int cY = cells.IndexOf(cell)/matrixWidth;
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

	/*
	 * Busca la celdas que sean del mismo color en toda la grid y los agrega a 'selected'
	 * 
	 * @params cell{Cell}: Celda de la que se tomara su color comop parametro para evaluar
	 */
	public List<Cell> searchCellsOfSameColor(Cell cell)
	{
		selected.Clear();
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].pieceType == cell.pieceType)
			{
				selected.Add(cells[i]);
			}
		}
		return selected;
	}

	public List<Cell> searchCellsOfSameColor(EPieceType cellType)
	{
		selected.Clear();
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].pieceType == cellType)
			{
				selected.Add(cells[i]);
			}
		}
		return selected;
	}

	public Cell[] allEmptyCells()
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