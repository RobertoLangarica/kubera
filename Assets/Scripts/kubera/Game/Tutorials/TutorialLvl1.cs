using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl1 : TutorialBase
{
	public GameObject secondObject;

	protected List<Cell> cells;
	protected Vector3 offset;

	protected override void Start()
	{
		base.Start ();

		offset = new Vector3(0,handObject.gameObject.GetComponent<Image> ().sprite.bounds.size.y,0);
	}

	public override bool canMoveToNextPhase ()
	{
		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent = ENextPhaseEvent.CREATE_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = true;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE1),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 1;

			goalPopUp.OnPopUpCompleted += startTutorialAnimation;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent = ENextPhaseEvent.SUBMIT_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = true;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE2A);
			
			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE2B);

			phase = 2;
			finishMovements ();
			StopCoroutine ("playPressAndContinueWithMethod");
			phase2Animation ();
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent = ENextPhaseEvent.CREATE_A_LINE;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = true;
			allowPowerUps = false;

			cells = new List<Cell> (cellManager.getAllEmptyCells ());
			for (int i = 0; i < 2; i++) {
				cells [i].occupied = true;
				cells [i].cellType = Cell.EType.OBSTACLE_LETTER;
				cells [i].contentType = Piece.EType.LETTER_OBSTACLE;
			}
			Debug.Log (cells.Count);
			for (int i = 4; i < 8; i++) {
				cells [i].occupied = true;
				cells [i].cellType = Cell.EType.OBSTACLE_LETTER;
				cells [i].contentType = Piece.EType.LETTER_OBSTACLE;
			}

			instructions [3].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE3);

			phase = 3;
			handObject = secondObject;
			offset *= 0.5f;
			phase3Animation ();
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			for (int i = 0; i < 2; i++) {
				cells [i].occupied = false;
				cells [i].cellType = Cell.EType.NORMAL;
				cells [i].contentType = Piece.EType.NONE;
			}
			for (int i = 4; i < 8; i++) {
				cells [i].occupied = false;
				cells [i].cellType = Cell.EType.NORMAL;
				cells [i].contentType = Piece.EType.NONE;
			}

			instructions [4].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE4),
				new string[1]{ "{{score}}"}, new string[1]{hudManager.goalText.text.Split('/')[1]});
			phase = 4;

			finishMovements ();
			hideHand ();
			return true;
		}

		return base.canMoveToNextPhase ();
	}

	public override bool phaseObjectiveAchived ()
	{
		switch (phase) 
		{
		case(1):
			if (wordManager.wordsValidator.isCompleteWord ()) 
			{
				return true;
			}
			break;
		case(2):
			return true;
		case(3):
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
			hideObject ();
			playTapAnimation ();
			showHandAt (handPositions [2].transform.position, Vector3.zero, false);
			OnMovementComplete -= hideHand;

			Invoke ("phase2Animation", 1);
		}
	}

	private void phase3Animation()
	{
		if (phase == 3) 
		{
			showHandAt (handPositions [3].transform.position,Vector3.zero,false);
			playReleaseAnimation ();

			StartCoroutine (playPressAndContinueWithMethod ("phase3_2Animation", 0.5f));
		}
	}

	private void phase3_2Animation()
	{
		if (phase == 3) 
		{
			playPressAnimation ();
			showObjectAtHand (offset);
			moveHandFromGameObjects (handPositions[3],handPositions[4],offset);
			OnMovementComplete += hideHand;

			Invoke ("phase3Animation", 2);
		}
	}
}


/*
Tutorial 1
-Bajar la mano en la M
Subir la M
*-Pner el puntaje del nivel
*/