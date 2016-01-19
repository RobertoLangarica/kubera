using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class CellsManager : MonoBehaviour 
{
	//public bool runCreationOnEditor = false;
	//public bool destroyGridOnEditor = false;

	//Es el prefab que se va a utilizar para la grid
	public GameObject cellPrefab;

	public GameObject letterFromBeginingPrefab;

	//Medidas del grid
	public int width;
	public int height;

	//Bandera para el powerUp de Destruccion por color
	//false: destruye todos los del mismo color en la escena
	//true: destruye todos los del mismo color que este juntos
	public bool selectNeighbours;

	//Las celdas seleccionadas por color
	protected List<Cell> selected = new List<Cell>();
	//Todas las celdas del grid
	protected List<Cell> cells = new List<Cell>();

	void Start () 
	{
		CreateGrid();
	}

	void Update () 
	{
		/*if(Input.GetKeyDown(KeyCode.A))
		{
			//LineCreated();
			clearCellsOfColor(cells[0]);
		}
		if(runCreationOnEditor)
		{
			CreateGrid();
			runCreationOnEditor = false;
		}
		if(destroyGridOnEditor)
		{
			DestroyGrid();
			destroyGridOnEditor = false;
		}*/
	}

	/*
	 * Se crea la grid y se asigna cada celda como hijo de este gameObject.
	 * Las celdas tienen un espacio de 3% de su tamaño de espacio en tre ellas
	 */
	protected void CreateGrid()
	{
		Vector3 nPos = transform.position;
		GameObject go = null;
		string[] levelGridData = FindObjectOfType<GameManager>().currentLevel.grid.Split(',');

		for(int i = 0;i < height;i++)
		{
			for(int j = 0;j < width;j++)
			{
				go = GameObject.Instantiate(cellPrefab,nPos,Quaternion.identity) as GameObject;
				go.transform.SetParent(transform);
				cells.Add(go.GetComponent<Cell>());

				cells[cells.Count-1].setTypeToCell(int.Parse(levelGridData[cells.Count-1]),letterFromBeginingPrefab);

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
		width = columns;
		height = rows;
		
		resetGrid();
	}
	
	public void resetGrid()
	{
		DestroyGrid();
		CreateGrid();
	}

	/*
	 * Regresa la celda dependiendo la posicion del mundo que se le manda
	 * 
	 * @params vec{Vector3}: Un punto en el mundo del cual se toman su 'x' y su 'y' para buscar la celda
	 * 
	 * @return {Cell}: La celda que tiene la posicion de mundo que se le envio dentro de sus limites
	 */
	protected Cell getCellOnVec(Vector3 vec)
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
		if(xPos < 0 || yPos < 0 || xPos >= width || yPos >= height)
		{
			return null;
		}
		return cells[(width*yPos)+xPos];
	}

	/*
	 * Evalua el estado de la grid para determinar si se ha creado o no una linea de piezas
	 * 
	 * @return {bool}: Regresa verdadero si se creo almenos una linea o falso si no se creo ninguna
	 */
	public bool LineCreated()
	{
		int[] widthCount = new int[height];
		int[] heightCount = new int[width];
		bool foundLine = false;

		int wIndex = 0;
		int hIndex = 0;

		//NOTA: Falta evaluar las celdas vacias
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].occupied && cells[i].typeOfPiece != ETYPEOFPIECE_ID.LETTER)
			{
				widthCount[wIndex]++;
				heightCount[hIndex]++;
			}
			hIndex ++;
			if(hIndex == width)
			{
				hIndex = 0;
				wIndex++;
			}
		}
		for(int i = 0;i < height;i++)
		{
			if(widthCount[i] == width)
			{
				createLettersOn(true,i);
				foundLine = true;
			}
		}
		for(int i = 0;i < width;i++)
		{
			if(heightCount[i] == height)
			{
				createLettersOn(false,i);
				foundLine = true;
			}
		}

		if(foundLine)
		{
			FindObjectOfType<GameManager>().GetComponent<AudioSource>().Play();
			return true;
		}


		return false;
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
			tempC.typeOfPiece = piece.typeOfPiece;
		}
		
		tempC = getCellOnVec(piece.transform.position);
		Vector3 nVec = new Vector3(tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
		                           -tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);

		gameObject.GetComponent<AudioSource>().Play();
		return (tempC).transform.position + nVec;
	}

	/*
	 * Cambi las piezas por letras de la linea que se le envia
	 * 
	 * @params isHorizontal{bool}: Es una bandera para indicar si la linea que se cambiara es horizontal o no
	 * 
	 * @params index{int}: Es el indice de en donde inicia la linea, este indice corresponde al de la lista de cells
	 */
	protected void createLettersOn(bool isHorizontal,int index)
	{
		if(isHorizontal)
		{
			for(int i = (index*width);i < (width*(index+1));i++)
			{
				turnPiecesToLetters(i,0);
			}
		}
		else
		{
			for(int i = 0;i < height;i++)
			{
				turnPiecesToLetters((i*width),index);
			}
		}
		FindObjectOfType<GameManager>().addPoints(10);
	}

	protected void turnPiecesToLetters(int cellIndex,int lineIndex)
	{
		int newIndex = lineIndex+cellIndex;
		cells[newIndex].typeOfPiece = ETYPEOFPIECE_ID.LETTER;
		if(cells[newIndex].piece != null)
		{
			cells[newIndex].piece.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
			cells[newIndex].piece.GetComponent<BoxCollider2D>().enabled = true;
			cells[newIndex].piece.GetComponent<Tile>().myLeterCase = PieceManager.instance.putLeter();
			cells[newIndex].piece.GetComponent<SpriteRenderer>().sprite = PieceManager.instance.changeTexture(cells[newIndex].piece.GetComponent<Tile>().myLeterCase);
			cells[newIndex].piece.GetComponent<Tile>().cellIndex = cells[newIndex];
			cells[newIndex].piece.AddComponent<ABCChar>();
		
			cells[newIndex].piece.GetComponent<ABCChar>().character = cells[newIndex].piece.GetComponent<Tile>().myLeterCase;
		
			if(cells[newIndex].piece.GetComponent<Tile>().myLeterCase == ".")
			{
				cells[newIndex].piece.AddComponent<ABCChar>().wildcard = true;
			}
			PieceManager.instance.listChar.Add(cells[newIndex].piece.GetComponent<ABCChar>());
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

		if(selectNeighbours)
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
		int cX = cells.IndexOf(cell)%width;
		int cY = cells.IndexOf(cell)/width;
		Cell tempC = null;

		tempC = getCellAt(cX,cY-1);
		if(tempC != null)
		{
			if(tempC.typeOfPiece == cell.typeOfPiece && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX-1,cY);
		if(tempC != null)
		{
			if(tempC.typeOfPiece == cell.typeOfPiece && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX+1,cY);
		if(tempC != null)
		{
			if(tempC.typeOfPiece == cell.typeOfPiece && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX,cY+1);
		if(tempC != null)
		{
			if(tempC.typeOfPiece == cell.typeOfPiece && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
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
	protected void searchCellsOfSameColor(Cell cell)
	{
		selected.Clear();
		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].typeOfPiece == cell.typeOfPiece)
			{
				selected.Add(cells[i]);
			}
		}
	}
}