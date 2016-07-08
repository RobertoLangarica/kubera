﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialLvl37 : TutorialBase 
{
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
			hideHand ();
			phasesPanels [0].SetActive (true);
			phaseEvent = ENextPhaseEvent.ROTATE_USED;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = true;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = true;
			freeDestroy = false;
			freeWildCard = false;

			instructions [0].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE1A);

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE1B);

			phase = 1;
			startGamePopUp.OnPopUpCompleted += startTutorialAnimation;
			return true;
		case(1):
			hideHand ();
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent = ENextPhaseEvent.PIECE_ROTATED;

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

			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE2);		
			phase = 2;
			finishMovements ();
			StopCoroutine ("playPressAndContinueWithMethod");
			hideHand ();
			phase2Animation ();
			return true;
		case(2):
			hideHand ();
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
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

			instructions [3].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE3A);
		
			instructions [4].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE3B);			
			phase = 3;
			finishMovements ();
			hideHand ();
			return true;
		case(3):
			hideHand ();
			phasesPanels [2].SetActive (false);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			freeBlocks = false;
			freeBombs = false;
			freeRotates = true;
			freeDestroy = false;
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
			Invoke ("callForTutorialManager",0.7f);
			return false;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}	

	protected void callForTutorialManager ()
	{
		FindObjectOfType<TutorialManager> ().moveTutorialToNextPhase ();
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
			hideObject ();
			playTapAnimation ();
			showHandAt (handPositions [1].transform.position, Vector3.zero, false);
			OnMovementComplete -= hideHand;

			Invoke ("phase2Animation", 1);
		}
	}
}
