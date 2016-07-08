using UnityEngine;
using System.Collections;

public class TutorialLvl2 : TutorialBase 
{
	protected override void Start()
	{
		base.Start ();

	}

	public override bool canMoveToNextPhase ()
	{
		switch (phase) 
		{
		case(0):
			hideHand ();
			phasesPanels [0].SetActive (true);
			phaseEvent = ENextPhaseEvent.WORD_SPECIFIC_LETTER_TAPPED;

			allowGridTap = false;
			allowWordTap = true;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE1),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 1;
			phaseObj = "H";
			startGamePopUp.OnPopUpCompleted += startTutorialAnimation;
			return true;
		case(1):
			hideHand ();
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent = ENextPhaseEvent.SUBMIT_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE2A);

			instructions [2].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE2B),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 2;
			phase2Animation ();
			return true;
		case(2):
			hideHand ();
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			instructions [3].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE3),
				new string[1]{ "{{score}}"}, new string[1]{hudManager.goalText.text.Split('/')[1]});
			phase = 3;
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
		}

		return base.phaseObjectiveAchived ();
	}	

	private void startTutorialAnimation(PopUpBase thisPopUp, string action)
	{
		phase1Animation ();
	}

	private void phase1Animation()
	{
		if (phase == 1) 
		{
			playTapAnimation ();
			showHandAt (handPositions [0].transform.position, Vector3.zero, false);

			Invoke ("phase1Animation", 1);
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
}