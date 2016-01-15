using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridBuilder : MonoBehaviour 
{
	public InputField columns;
	public InputField rows;
	private CellsManager cells;
	
	void Start () 
	{
		cells = FindObjectOfType<CellsManager>();
	}
	
	public void OnValueChange()
	{
		cells.resetGrid(int.Parse(columns.text),int.Parse(rows.text));
	}
}
