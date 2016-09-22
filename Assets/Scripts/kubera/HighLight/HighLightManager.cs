using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HighLightManager : Manager<HighLightManager>
{
	public enum EHighLightType
	{
		BOMB_POWERUP,
		DESTROY_POWERUP,
		ROTATE_POWERUP,
		SQUARE_POWERUP,
		WILDCARD_POWERUP,
		BOMB_SPECIFIC_COLOR,
		DESTROY_SPECIFIC_COLOR,
		WORD_HINT,
		NO_SPACE_FOR_PIECES,
		PIECES_AREA,
		EMPTY_CELLS,
		SUBMIT_WORD,
		OBJECTIVE,
		MOVEMENTS,
		BOMB_BUTTON,
		DESTROY_BUTTON,
		ROTATE_BUTTON,
		SQUARE_BUTTON,
		WILDCARD_BUTTON,
		WORD_HINT_BUTTON,
		SPECIFIC_CELL,
		GEMS,
		POWER_UPS_AREA,
		ALL_POPUPS
	}

	public enum EHighLightStatus
	{
		NORMAL,
		WRONG,
		HINT
	}

	public const string HIGHLIGHT_TAG = "HighLight";

	public Color normalHighLight;
	public Color wrongHighLight;
	public Color hintHighLight;

	protected List<HighLight> activeHighLight = new List<HighLight>();
	protected CellsManager _cellManager;
	protected HUDManager _hudManager;
	protected WordManager _wordManager;
	protected PowerUpManager _powerUpManager;

	protected EHighLightType currentType;

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

	protected WordManager wordManager
	{
		get
		{
			if (_wordManager == null) 
			{
				_wordManager = FindObjectOfType<WordManager> ();
			}

			return _wordManager;
		}
	}

	protected PowerUpManager powerUpManager
	{
		get
		{
			if (_powerUpManager == null) 
			{
				_powerUpManager = FindObjectOfType<PowerUpManager> ();
			}

			return _powerUpManager;
		}
	}

	/**
	 * NOTA: El que prenda el highLight se tiene que encargar de apagar su propio HighLight
	*/
	public void setHighLightOfType(EHighLightType type,Object obj = null)
	{
		Cell[] tempCell = null;

		currentType = type;
		
		switch (type) 
		{
		case(EHighLightType.BOMB_POWERUP):
		case(EHighLightType.DESTROY_POWERUP):
			tempCell = cellManager.getCellsOfSameType (Piece.EType.PIECE);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell [i].transform,EHighLightStatus.NORMAL);
			}
			break;
		case(EHighLightType.BOMB_SPECIFIC_COLOR):
			tempCell = cellManager.getCellNeighborsOfSameColor (obj as Cell);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform,EHighLightStatus.NORMAL);
			}
			break;
		case(EHighLightType.DESTROY_SPECIFIC_COLOR):
			tempCell = cellManager.getCellsOfSameColor (obj as Cell);

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform,EHighLightStatus.NORMAL);
			}
			break;
		case(EHighLightType.ROTATE_POWERUP):
		case(EHighLightType.PIECES_AREA):
			turnOnHighLights (hudManager.rotationImagePositions[0].parent,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.SQUARE_POWERUP):
		case(EHighLightType.EMPTY_CELLS):
			tempCell = cellManager.getAllEmptyCells ();

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform,EHighLightStatus.NORMAL);
			}
			break;
		case(EHighLightType.WILDCARD_POWERUP):
		case(EHighLightType.WORD_HINT):
			tempCell = cellManager.getAllShowedCels ();

			for (int i = 0; i < tempCell.Length; i++) 
			{
				turnOnHighLights (tempCell[i].transform,EHighLightStatus.NORMAL);
			}
			break;
		case(EHighLightType.NO_SPACE_FOR_PIECES):
			turnOnHighLights (hudManager.rotationImagePositions[0].parent.parent,EHighLightStatus.WRONG);
			break;
		case(EHighLightType.SUBMIT_WORD):
			turnOnHighLights (wordManager.wordCompleteButton.transform,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.OBJECTIVE):
			turnOnHighLights (hudManager.goalText.transform.parent.parent,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.MOVEMENTS):
			turnOnHighLights (hudManager.movementsText.transform.parent,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.BOMB_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.BOMB).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.DESTROY_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.DESTROY).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.ROTATE_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.ROTATE).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.SQUARE_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.BLOCK).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.WILDCARD_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.WILDCARD).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.WORD_HINT_BUTTON):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.HINT_WORD).powerUpButton,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.SPECIFIC_CELL):
			turnOnHighLights ((Transform)obj,EHighLightStatus.HINT);
			break;
		case(EHighLightType.POWER_UPS_AREA):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.HINT_WORD).powerUpButton.parent.parent,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.GEMS):
			turnOnHighLights (hudManager.GemsChargeGO.transform.parent,EHighLightStatus.NORMAL);
			break;
		case(EHighLightType.ALL_POPUPS):
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.HINT_WORD).powerUpButton,EHighLightStatus.NORMAL);
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.WILDCARD).powerUpButton,EHighLightStatus.NORMAL);
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.BLOCK).powerUpButton,EHighLightStatus.NORMAL);
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.DESTROY).powerUpButton,EHighLightStatus.NORMAL);
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.BOMB).powerUpButton,EHighLightStatus.NORMAL);
			turnOnHighLights (powerUpManager.getPowerupByType(PowerupBase.EType.ROTATE).powerUpButton,EHighLightStatus.NORMAL);
			break;
		}
	}

	protected void turnOnHighLights(Transform objectToSearch,EHighLightStatus status)
	{
		HighLight tempHL = null;

		for (int i = 0; i < objectToSearch.childCount; i++) 
		{
			if (objectToSearch.GetChild(i).tag == HIGHLIGHT_TAG) 
			{
				tempHL = objectToSearch.GetChild (i).gameObject.GetComponent<HighLight> ();
				if (tempHL.activateHighLight (currentType, status)) 
				{
					activeHighLight.Add (tempHL);
				}
			}
		}
	}

	public void turnOffHighLights(EHighLightType type)
	{
		for (int i = activeHighLight.Count -1; i > -1; i--) 
		{
			if (activeHighLight [i].deactivateType (type)) 
			{
				activeHighLight.RemoveAt (i);
			}
		}
	}

	public void turnOffAllHighLights()
	{
		for (int i = activeHighLight.Count -1; i > -1; i--) 
		{
			activeHighLight [i].completlyDeactivate ();

			activeHighLight.RemoveAt (i);
		}
	}
}