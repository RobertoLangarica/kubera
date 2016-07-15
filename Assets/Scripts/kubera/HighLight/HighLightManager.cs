using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighLightManager : Manager<HighLightManager>
{
	public enum EHIghLightType
	{
		BOMB_POWERUP,
		DESTROY_POWERUP,
		ROTATE_POWERUP,
		SQUARE_POWERUP,
		WILDCARD_POWERUP,
		BOMB_SPECIFIC_COLOR,
		DESTROY_SPECIFIC_COLOR
	}

	public const string HIGHLIGHT_TAG = "HighLight";

	protected List<GameObject> activeHighLight = new List<GameObject>();
	protected CellsManager _cellManager;
	protected HUDManager _hudManager;

	protected CellsManager cellManager
	{
		get
		{
			if (_cellManager == null) 
			{
				_cellManager = FindObjectOfType<CellsManager> ();
			}

			return _cellManager;
		}
	}

	protected HUDManager hudManager
	{
		get
		{
			if (_hudManager == null) 
			{
				_hudManager = FindObjectOfType<HUDManager> ();
			}

			return _hudManager;
		}
	}

	public void setHighLightOfType(EHIghLightType type,Object obj = null)
	{
		Cell[] tempCell = null;

		switch (type) 
		{
		case(EHIghLightType.BOMB_POWERUP):
		case(EHIghLightType.DESTROY_POWERUP):
			tempCell = cellManager.getCellsOfSameType (Piece.EType.PIECE);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform);
			}
			break;
		case(EHIghLightType.BOMB_SPECIFIC_COLOR):
			tempCell = cellManager.getCellNeighborsOfSameColor (obj as Cell);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform);
			}
			break;
		case(EHIghLightType.DESTROY_SPECIFIC_COLOR):
			tempCell = cellManager.getCellsOfSameColor (obj as Cell);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform);
			}
			break;
		case(EHIghLightType.ROTATE_POWERUP):
			turnOnHighLights (hudManager.showingPiecesContainer);
			break;
		}
	}

	protected void turnOnHighLights(Transform objectToSearch)
	{
		for (int i = 0; i < objectToSearch.childCount; i++) 
		{
			if (objectToSearch.GetChild(i).tag == HIGHLIGHT_TAG) 
			{
				objectToSearch.GetChild (i).gameObject.SetActive(true);
				activeHighLight.Add (objectToSearch.GetChild (i).gameObject);
			}
		}
	}

	public void turnOffHighLights()
	{
		for (int i = 0; i < activeHighLight.Count; i++) 
		{
			activeHighLight [i].SetActive (false);
		}

		activeHighLight.Clear ();
		activeHighLight = new List<GameObject> ();
	}
}