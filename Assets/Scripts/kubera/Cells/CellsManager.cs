using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class CellsManager : MonoBehaviour 
{
	//public bool runCreationOnEditor = false;
	//public bool destroyGridOnEditor = false;

	public GameObject cellPrefab;

	public int width;
	public int height;

	public List<Cell> selected = new List<Cell>();

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

	protected void CreateGrid()
	{
		Vector3 nPos = transform.position;
		GameObject go = null;

		for(int i = 0;i < height;i++)
		{
			for(int j = 0;j < width;j++)
			{
				go = GameObject.Instantiate(cellPrefab,nPos,Quaternion.identity) as GameObject;
				go.transform.SetParent(transform);
				cells.Add(go.GetComponent<Cell>());
				nPos.x += cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x + 0.03f;
			}
			nPos.y += cellPrefab.GetComponent<SpriteRenderer>().bounds.size.y + 0.03f;
			nPos.x = transform.position.x;
		}
	}

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

	protected Cell getCellAt(int xPos,int yPos)
	{
		if(xPos < 0 || yPos < 0 || xPos >= width || yPos >= height)
		{
			return null;
		}
		return cells[(width*yPos)+xPos];
	}

	public bool LineCreated()
	{
		int[] widthCount = new int[height];
		int[] heightCount = new int[width];
		bool foundLine = false;

		int wIndex = 0;
		int hIndex = 0;

		for(int i = 0;i < cells.Count;i++)
		{
			if(cells[i].occupied && cells[i].color != ECOLORS_ID.LETER)
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
				//Debug.Log("Se encontro linea en fila: " + i);
				createLettersOn(true,i);
				foundLine = true;
			}
		}
		for(int i = 0;i < width;i++)
		{
			if(heightCount[i] == height)
			{
				//Debug.Log("Se encontro linea en columna: " + i);
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
			}
		}
		return true;
	}

	public bool CanPositionate(Vector3[] arr)
	{
		Cell tempC = null;
		//selected.Clear();
		
		for(int i = 0;i < arr.Length;i++)
		{
			tempC = getCellOnVec(arr[i]);
			//selected.Add(tempC);
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
			}
		}
		return true;
	}

	public Vector3 Positionate(Piece piece)
	{
		Cell tempC = null;

		for(int i = 0;i < piece.pieces.Length;i++)
		{
			tempC = getCellOnVec(piece.pieces[i].transform.position);
			tempC.occupied = true;
			tempC.piece=piece.pieces[i];
			tempC.color = piece.color;
		}
		
		tempC = getCellOnVec(piece.transform.position);
		Vector3 nVec = new Vector3(tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,
		                           -tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x*0.5f,0);

		gameObject.GetComponent<AudioSource>().Play();
		return (tempC).transform.position + nVec;
	}

	protected void createLettersOn(bool isHorizontal,int index)
	{
		if(isHorizontal)
		{
			for(int i = (index*width);i < (width*(index+1));i++)
			{
				cells[i].color = ECOLORS_ID.LETER;
				cells[i].piece.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
				cells[i].piece.GetComponent<BoxCollider2D>().enabled = true;
				cells[i].piece.GetComponent<Tile>().myLeterCase = PieceManager.instance.putLeter();
				cells[i].piece.GetComponent<SpriteRenderer>().sprite = PieceManager.instance.changeTexture(cells[i].piece.GetComponent<Tile>().myLeterCase);
				cells[i].piece.GetComponent<Tile>().cellIndex = cells[i];
				cells[i].piece.AddComponent<ABCChar>();
			
				cells[i].piece.GetComponent<ABCChar>().character = cells[i].piece.GetComponent<Tile>().myLeterCase;

				if(cells[i].piece.GetComponent<Tile>().myLeterCase == ".")
				{
					cells[i].piece.AddComponent<ABCChar>().wildcard = true;
				}
				PieceManager.instance.listChar.Add(cells[i].piece.GetComponent<ABCChar>());
			}
		}
		else
		{
			for(int i = 0;i < height;i++)
			{
				cells[index+(i*width)].color = ECOLORS_ID.LETER;
				cells[index+(i*width)].piece.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
				cells[index+(i*width)].piece.GetComponent<BoxCollider2D>().enabled = true;
				cells[index+(i*width)].piece.GetComponent<Tile>().myLeterCase = PieceManager.instance.putLeter();
				cells[index+(i*width)].piece.GetComponent<SpriteRenderer>().sprite = PieceManager.instance.changeTexture(cells[index+(i*width)].piece.GetComponent<Tile>().myLeterCase);
				cells[index+(i*width)].piece.GetComponent<Tile>().cellIndex = cells[index+(i*width)];
				cells[index+(i*width)].piece.AddComponent<ABCChar>();

				cells[index+(i*width)].piece.GetComponent<ABCChar>().character = cells[index+(i*width)].piece.GetComponent<Tile>().myLeterCase;

				if(cells[index+(i*width)].piece.GetComponent<Tile>().myLeterCase == ".")
				{
					cells[index+(i*width)].piece.AddComponent<ABCChar>().wildcard = true;
				}
				PieceManager.instance.listChar.Add(cells[index+(i*width)].piece.GetComponent<ABCChar>());
			}
		}
		FindObjectOfType<GameManager>().addPoints(10);
	}

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

	public void selectCellsOfColor(Cell cell)
	{
		List<Cell> finalList = new List<Cell>();
		List<Cell> pendingList = new List<Cell>();

		pendingList.Add(cell);

		while(pendingList.Count > 0)
		{
			searchNeigboursOfSameColor(pendingList[0],ref finalList,ref pendingList);
		}
		selected = finalList;
	}

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

	public void destroySelectedCells()
	{
		for(int i = 0;i < selected.Count;i++)
		{
			selected[i].destroyCell();
		}

		selected = new List<Cell>();
	}

	protected void searchNeigboursOfSameColor(Cell cell,ref List<Cell> final, ref List<Cell> pending)
	{
		int cX = cells.IndexOf(cell)%width;
		int cY = cells.IndexOf(cell)/width;
		Cell tempC = null;

		/*tempC = getCellAt(cX-1,cY-1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}*/
		tempC = getCellAt(cX,cY-1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		/*tempC = getCellAt(cX+1,cY-1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}*/
		tempC = getCellAt(cX-1,cY);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		tempC = getCellAt(cX+1,cY);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		/*tempC = getCellAt(cX-1,cY+1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}*/
		tempC = getCellAt(cX,cY+1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}
		/*tempC = getCellAt(cX+1,cY+1);
		if(tempC != null)
		{
			if(tempC.color == cell.color && pending.IndexOf(tempC) == -1 && final.IndexOf(tempC) == -1)
			{
				pending.Add(tempC);
			}
		}*/
		pending.Remove(cell);
		final.Add(cell);
	}
}