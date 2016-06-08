using UnityEngine;
using System.Collections;

public class TutorialLvl4 : TutorialBase 
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
			phasesPanels [0].SetActive (true);
			phaseEvent = ENextPhaseEvent.DELETE_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = true;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [0].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE1A);

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE1B);

			phase = 1;
			goalPopUp.OnPopUpCompleted += startTutorialAnimation;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE2);		
			phase = 2;
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
}