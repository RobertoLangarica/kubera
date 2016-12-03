using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl8 : TutorialBase 
{
	public TutorialManager tutoManager;

	protected bool x2Showed;
	protected bool x3Showed;

	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		int total = int.Parse (hudManager.goalText.text.Split ('/') [1].Split (' ') [0]);
		int current = int.Parse (hudManager.goalText.text.Split('/')[0]);

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);


			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			//Invoke ("writeLetterByLetter", initialAnim * 2);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

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
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			x2Showed = true;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);
			
			phase = 2;
			goToPhase3 ();
			return true;		
		case(2):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [0].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			x3Showed = true;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			phase = 3;
			goToPhase3 ();
			return true;	
		case(3):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.SELECT_LETTER);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE4).Replace ("{{neededScore}}", (total - current).ToString ());;
			instructionsText = instructions [3];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			phase = 5;
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
		case(4):
			phase = 3;
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected void goToPhase3()
	{
		if (x2Showed && x3Showed) 
		{
			phase = 4;

			tutoManager.registerForNextPhase (ENextPhaseEvent.SELECT_LETTER);
		}
	}
}