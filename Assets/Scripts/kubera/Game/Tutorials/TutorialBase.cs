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

	public float writingSpeed = 0.05f;
	protected bool isWriting = false;
	protected float writingTimer;

	protected bool foundStringTag;
	protected bool changeInstruction;

	protected int instructionIndex;
	protected string currentInstruction;

	protected RectTransform instructionsContainer;
	protected Text instructionsText;

	protected bool hasMask;

	protected int previousPhase;
	protected int currentPhase;
	
	public enum ENextPhaseEvent
	{
		CREATE_WORD,
		SUBMIT_WORD,
		DELETE_WORD,
		SELECT_LETTER,
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
		HINT_USED,
		ALL_PIECES_USED,
		WILDCARD_OVER_OBSTACLE
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

	public GameObject tutorialMask;

	[HideInInspector]public int phase;
	[HideInInspector]public string phaseObj;
	[HideInInspector]public List<ENextPhaseEvent> phaseEvent;

	public delegate void DAnimationNotification ();

	protected bool alphaLowered;
	protected DAnimationNotification OnMovementComplete;
	public WordManager wordManager;
	public GameManager gameManager;
	public CellsManager cellManager;
	public HUDManager hudManager;
	public PowerUpManager powerUpManager;

	public Button settingsButton;
	public Button gemsButton;

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		if (isWriting) 
		{
			writingTimer += Time.deltaTime;

			if (writingTimer >= writingSpeed) 
			{
				writeLetterByLetter ();
				writingTimer = 0;
			}
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
		int tempprev = previousPhase;
		int tempcurr = currentPhase;

		instructionsContainer = instructions [tempprev].rectTransform;
		instructionsContainer.DOAnchorPos (new Vector2(instructionsContainer.rect.width,0),0.5f).OnComplete(
		()=>
			{
				phasesPanels[tempprev].SetActive(false);
				phasesPanels[tempcurr].SetActive(true);

				instructionsContainer = instructions [tempcurr].rectTransform;
				instructionsContainer.anchoredPosition = new Vector2(-instructionsContainer.rect.width,0);
				instructionsContainer.DOAnchorPos (Vector2.zero,0.5f);
			});
		/*instructionsContainer = instructionsText.transform.parent;
		instructionsContainer.DOShakePosition (shakeDuraion,shakeStrength);*/
	}

	protected void writeLetterByLetter()
	{
		isWriting = true;

		//Ignora escribir la siguiente letra cuando se cambia de instruccion
		if (changeInstruction) 
		{
			changeInstruction = false;
			return;
		}


		if (currentInstruction [instructionIndex].ToString () == "\\") 
		{
			instructionsText.text = ((string)instructionsText.text).Insert (instructionIndex, "\n");
			instructionIndex += 2;			
		} 
		else if (currentInstruction [instructionIndex].ToString () == "<" && !foundStringTag) 
		{
			int ndx = currentInstruction.IndexOf ('>', instructionIndex);
			string textTag = currentInstruction.Substring (instructionIndex, (ndx + 1) - instructionIndex);

			foundStringTag = true;

			instructionsText.text = ((string)instructionsText.text).Insert (instructionIndex, textTag);
			instructionIndex += textTag.Length;

			textTag = textTag.Insert (1, "/");
			instructionsText.text = ((string)instructionsText.text).Insert (instructionIndex, textTag);
		} 
		else if (currentInstruction [instructionIndex].ToString () == "<" && foundStringTag) 
		{
			int ndx = currentInstruction.IndexOf ('>', instructionIndex);
			string textTag = currentInstruction.Substring (instructionIndex, (ndx + 1) - instructionIndex);

			instructionIndex += textTag.Length;
			foundStringTag = false;
		}
		else 
		{
			instructionsText.text = ((string)instructionsText.text).Insert (instructionIndex, currentInstruction [instructionIndex].ToString ());
			instructionIndex++;
		}

		if (instructionIndex >= currentInstruction.Length) 
		{
			isWriting = false;
			OnWritingFinished ();
		}
	}

	protected virtual void OnWritingFinished()
	{
	}

	protected void activateMask()
	{
		hasMask = true;
		tutorialMask.SetActive (true);

		settingsButton.enabled = false;
		gemsButton.enabled = false;
	}

	protected void deactivateMask()
	{
		hasMask = false;
		tutorialMask.SetActive (false);

		settingsButton.enabled = true;
		gemsButton.enabled = true;
	}

	protected void disablePowerUps()
	{
		for(int i = 0;i < powerUpManager.powerups.Count;i++)
		{
			if(powerUpManager.powerups [i].powerUpButton.gameObject.activeSelf)
			{
				powerUpManager.powerups [i].powerUpButton.gameObject.SetActive (false);
				phasesPanels.Add (powerUpManager.powerups [i].powerUpButton.gameObject);
			}
		}
	}

	protected void enablePowerUps()
	{
		for(int i = instructions.Count;i < phasesPanels.Count;i++)
		{
			phasesPanels[i].SetActive (true);
		}
	}
}