﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialLvl64 : TutorialBase 
{
	public KeyBoardManager keyboard;

	protected Vector3 offset;

	protected override void Start()
	{
		base.Start ();

		offset = new Vector3(0,handObject.gameObject.GetComponent<Image> ().sprite.bounds.extents.y,0);
	}

	public override bool canMoveToNextPhase ()
	{
		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent = ENextPhaseEvent.WILDCARD_USED;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = true;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = true;

			instructions [0].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1A);

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1B);

			phase = 1;
			goalPopUp.OnPopUpCompleted += startTutorialAnimation;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent = ENextPhaseEvent.KEYBOARD_SPECIFIC_LETER_SELECTED;
			phaseObj = "Z";

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = false;

			instructions [2].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2A),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			instructions [3].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2B),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 2;
			finishMovements ();
			StopCoroutine ("playPressAndContinueWithMethod");
			getKeyboarLetterPosition ();
			phase2Animation ();
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent = ENextPhaseEvent.SUBMIT_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = false;

			instructions [4].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE3A);

			instructions [5].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE3B),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 3;
			finishMovements ();
			phase3Animation ();
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent = ENextPhaseEvent.TAP;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = false;

			instructions [6].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE4A);

			instructions [7].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE4B);			
			phase = 4;
			hideHand ();
			return true;
		case(4):
			phasesPanels [3].SetActive (false);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = true;
			freeWildCard = false;

			return true;			
		}

		return base.canMoveToNextPhase ();
	}

	public override bool phaseObjectiveAchived ()
	{
		switch (phase) 
		{
		case(1):
			return true;
		case(2):
			if (wordManager.wordsValidator.isCompleteWord ()) 
			{
				return true;
			}
			break;
		case(3):
			return true;
		case(4):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}		

	private void startTutorialAnimation(PopUpBase thisPopUp, string action)
	{
		Invoke ("phase0Animation",0.5f);
		showHandAt (handPositions [0].transform.position,Vector3.zero,false);
	}

	private void phase0Animation()
	{
		if (phase == 1) 
		{
			showHandAt (handPositions [0].transform.position,Vector3.zero,false);
			playReleaseAnimation ();

			StartCoroutine (playPressAndContinueWithMethod ("phase1Animation", 0.5f));
		}
	}

	private void phase1Animation()
	{
		if (phase == 1) 
		{
			playPressAnimation ();
			showObjectAtHand (offset);
			moveHandFromGameObjects (handPositions[0],handPositions[1],offset);
			OnMovementComplete += hideHand;

			Invoke ("phase0Animation", 2);
		}
	}

	private void phase2Animation()
	{
		if (phase == 2) 
		{
			playTapAnimation ();
			showHandAt (handPositions [1].transform.position, Vector3.zero, false);

			Invoke ("phase2Animation", 1);
		}
	}

	private void phase3Animation()
	{
		if (phase == 3) 
		{
			playTapAnimation ();
			showHandAt (handPositions [2].transform.position, Vector3.zero, false);

			Invoke ("phase3Animation", 1);
		}
	}

	private void getKeyboarLetterPosition()
	{
		Letter[] temp = keyboard.container.GetComponentsInChildren<Letter> ();

		for (int i = 0; i < temp.Length; i++) 
		{
			if (temp [i].abcChar.character == phaseObj) 
			{
				RectTransform rectT = temp [i].GetComponent<RectTransform> ();
				handPositions [1].transform.position = new Vector3(rectT.anchoredPosition.x,rectT.anchoredPosition.y,0) + new Vector3 (
					-temp [i].gameObject.GetComponent<Image> ().sprite.bounds.extents.x,-temp [i].gameObject.GetComponent<Image> ().sprite.bounds.size.y,0);
				Debug.Log (temp [i].transform.position);
				Debug.Log (temp[i].transform.localPosition);
				break;
			}
		}
	}

}
