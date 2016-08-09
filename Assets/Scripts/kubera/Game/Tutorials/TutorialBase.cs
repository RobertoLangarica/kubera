﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialBase : MonoBehaviour 
{
	public float initialAnim = 1;

	public float shakeDuraion = 0.5f;
	public float shakeStrength = 20;

	public float writingSpeed = 0.1f;


	protected int instructionIndex;
	protected string currentInstruction;

	protected Transform instructionsContainer;
	protected Text instructionsText;
	
	public enum ENextPhaseEvent
	{
		CREATE_WORD,
		SUBMIT_WORD,
		DELETE_WORD,
		CREATE_A_LINE,
		WORD_SPECIFIC_LETTER_TAPPED,
		GRID_SPECIFIC_LETTER_TAPPED,
		KEYBOARD_SPECIFIC_LETER_SELECTED,
		KEYBOARD_LETER_SELECTED,
		PIECE_ROTATED,
		TAP,
		BOMB_USED,
		BLOCK_USED,
		ROTATE_USED,
		DESTROY_USED,
		WILDCARD_USED,
		POSITIONATE_PIECE,
		CLEAR_A_LINE,
		EARNED_POINTS,
		MOVEMENT_USED,
		HINT_USED
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
	[HideInInspector]public bool freeHint;

	public string levelName;

	public List<Text> instructions;

	public List<GameObject> phasesPanels;

	public startGamePopUp startGamePopUp;

	[HideInInspector]public int phase;
	[HideInInspector]public string phaseObj;
	[HideInInspector]public List<ENextPhaseEvent> phaseEvent;

	public delegate void DAnimationNotification ();

	protected bool alphaLowered;
	protected DAnimationNotification OnMovementComplete;
	protected WordManager wordManager;
	protected GameManager gameManager;
	protected CellsManager cellManager;
	protected HUDManager hudManager;

	protected virtual void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		wordManager = FindObjectOfType<WordManager> ();
		cellManager = FindObjectOfType<CellsManager> ();
		hudManager = FindObjectOfType<HUDManager> ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A)) 
		{
			firstAnim ();
		}
	}

	public virtual bool canMoveToNextPhase()
	{
		return false;
	}

	public virtual bool phaseObjectiveAchived()
	{
		return false;
	}

	protected void firstAnim()
	{
		Image firstPhase = phasesPanels [0].GetComponentInChildren<Image> ();

		float pos = firstPhase.rectTransform.parent.GetComponent<RectTransform> ().rect.height;
		firstPhase.rectTransform.offsetMax = new Vector2 (0,pos); //top
		firstPhase.rectTransform.offsetMin = new Vector2 (0,pos); //bottom

		firstPhase.rectTransform.DOAnchorPos (Vector2.zero,initialAnim).SetEase (Ease.InOutBack);
	}

	protected void shakeToErrase()
	{
		instructionsContainer.DOShakePosition (shakeDuraion,shakeStrength);
	}

	protected void writeLetterByLetter()
	{
		instructionsText.text = ((string)instructionsText.text).Insert(instructionIndex,currentInstruction [instructionIndex].ToString());

		instructionIndex++;

		if (instructionIndex < currentInstruction.Length) 
		{
			Invoke ("writeLetterByLetter",writingSpeed);
		}
	}
}