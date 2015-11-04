using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class CellsManager : MonoBehaviour 
{
	/*public bool runCreationOnEditor = false;
	public bool destroyGridOnEditor = false;*/

	public GameObject cellPrefab;

	public int width;
	public int height;

	protected List<Cell> cells = new List<Cell>();

	// Use this for initialization
	void Start () 
	{
		CreateGrid();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			LineCreated();
		}
		/*if(runCreationOnEditor)
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
		Vector3 nPos = Vector3.zero;
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
			nPos.x = 0;
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

	public void LineCreated()
	{
		int[] widthCount = new int[height];
		int[] heightCount = new int[width];

		int wIndex = 0;
		int hIndex = 0;

		Debug.Log ("Evaluando!!!");

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
				Debug.Log("Se encontro linea en fila: " + i);
				createLettersOn(true,i);
			}
		}
		for(int i = 0;i < width;i++)
		{
			if(heightCount[i] == height)
			{
				Debug.Log("Se encontro linea en columna: " + i);
				createLettersOn(false,i);
			}
		}
	}

	public Vector3 CanPositionate(GameObject[] arr)
	{
		Cell tempC = null;

		for(int i = 0;i < arr.Length;i++)
		{
			tempC = getCellOnVec(arr[i].transform.position);
			if(tempC == null)
			{
				return new Vector3(-100,-100,-100);
			}
			else
			{
				if(tempC.occupied)
				{
					return new Vector3(-100,-100,-100);
				}
			}
		}

		tempC = getCellOnVec(arr[0].transform.position);
		Vector3 nVec = new Vector3(tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x,
		                           tempC.gameObject.GetComponent<SpriteRenderer>().bounds.size.x,0);
		return (tempC).transform.position + nVec;
	}

	protected void createLettersOn(bool isHorizontal,int index)
	{
		if(isHorizontal)
		{
			for(int i = (index*width);i < (width*(index+1));i++)
			{
				cells[i].color = ECOLORS_ID.LETER;
			}
		}
		else
		{
			for(int i = 0;i < height;i++)
			{
				cells[index+(i*width)].color = ECOLORS_ID.LETER;
			}
		}
	}
}
