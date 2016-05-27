﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialBase : MonoBehaviour 
{
	public enum ENextPhaseEvent
	{
		CREATE_WORD,
		SUBMIT_WORD,
		DELETE_WORD,
		CREATE_A_LINE,
		WORD_SPECIFIC_LETTER_TAPPED,
		GRID_SPECIFIC_LETTER_TAPPED,
		KEYBOARD_SPECIFIC_LETER_SELECTED,
		PIECE_ROTATED,
		TAP,
		BOMB_USED,
		BLOCK_USED,
		ROTATE_USED,
		DESTROY_USED,
		WILDCARD_USED
	}

	[HideInInspector]public bool allowGridTap;
	[HideInInspector]public bool allowWordTap;
	[HideInInspector]public bool allowLetterDrag;
	[HideInInspector]public bool allowErraseWord;
	[HideInInspector]public bool allowDragPieces;
	[HideInInspector]public bool allowPowerUps;

	//PowerUps
	[HideInInspector]public bool freeBombs;
	[HideInInspector]public bool freeBlocks;
	[HideInInspector]public bool freeRotates;
	[HideInInspector]public bool freeDestroy;
	[HideInInspector]public bool freeWildCard;

	public string levelName;

	public List<Text> instructions;

	public List<GameObject> phasesPanels;

	[HideInInspector]public int phase;
	[HideInInspector]public string phaseObj;
	[HideInInspector]public ENextPhaseEvent phaseEvent;

	protected WordManager wordManager;
	protected GameManager gameManager;
	protected CellsManager cellManager;

	protected virtual void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		wordManager = FindObjectOfType<WordManager> ();
		cellManager = FindObjectOfType<CellsManager> ();
	}

	public virtual bool canMoveToNextPhase()
	{
		return false;
	}

	public virtual bool phaseObjectiveAchived()
	{
		return false;
	}
}