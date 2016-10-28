using UnityEngine;
using System.Collections;

public class TutorialLvl6 : TutorialBase
{
	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV6_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.HINT_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV6_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			phase = 2;
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
			return true;
		}

		return base.phaseObjectiveAchived ();
	}
}
