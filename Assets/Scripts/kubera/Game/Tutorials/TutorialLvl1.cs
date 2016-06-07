using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl1 : TutorialBase
{
	public GoalPopUp goalPopUp;

	protected List<Cell> cells;

	protected override void Start()
	{
		base.Start ();
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

			goalPopUp.OnPopUpCompleted += phase0Animation;

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
			for (int i = 0; i < 2; i++) 
			{
				cells [i].occupied = true;
				cells [i].cellType = Cell.EType.OBSTACLE_LETTER;
				cells [i].contentType = Piece.EType.LETTER_OBSTACLE;
			}
			for (int i = 4; i < 8; i++) 
			{
				cells [i].occupied = true;
				cells [i].cellType = Cell.EType.OBSTACLE_LETTER;
				cells [i].contentType = Piece.EType.LETTER_OBSTACLE;
			}

			instructions [3].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE3);

			phase = 3;
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

			for (int i = 0; i < 2; i++) 
			{
				cells [i].occupied = false;
				cells [i].cellType = Cell.EType.NORMAL;
				cells [i].contentType = Piece.EType.NONE;
			}
			for (int i = 4; i < 8; i++) 
			{
				cells [i].occupied = false;
				cells [i].cellType = Cell.EType.NORMAL;
				cells [i].contentType = Piece.EType.NONE;
			}

			instructions [4].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE4);
			phase = 4;
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

	private void phase0Animation(PopUpBase thisPopUp, string action)
	{
		showHandAt (handPositions [0].transform.position);
		Invoke ("phase1Animation",2);
	}

	private void phase1Animation()
	{
		playPressAnimation ();
		moveHandFromGameObjects (handPositions[0],handPositions[1]);
		OnMovementComplete += hideHand;
	}

	private void phase2Animation()
	{
		showHandAt (handPositions [2].transform.position);
		OnMovementComplete -= hideHand;
	}
}