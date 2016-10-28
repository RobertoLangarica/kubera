using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl8 : TutorialBase 
{
	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);


			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			Invoke ("writeLetterByLetter", initialAnim * 2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [2].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);
			
			phase = 2;
			return true;		
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [0].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 3;
			return true;	
		}

		return base.canMoveToNextPhase ();
	}

	public override bool phaseObjectiveAchived ()
	{
		switch (phase) 
		{
		case(1):
			if (cellManager.getCellsOfSameType (Piece.EType.LETTER).Length > 5) 
			{
				phase = 2;
			}
			return true;
		case(2):
			return true;
		case(3):
			if (cellManager.getCellsOfSameType (Piece.EType.LETTER).Length > 7) 
			{
				phase = 1;
			}
			return true;
		}

		return base.phaseObjectiveAchived ();
	}
}