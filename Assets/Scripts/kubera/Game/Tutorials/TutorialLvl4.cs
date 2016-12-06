using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl4 : TutorialBase 
{
	public PieceManager pieceManager;

	protected bool bLineUsed;
	protected bool bLUsed;
	protected bool bLittleLUsed;

	protected override void Start()
	{
		base.Start ();

	}

	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			previousPhase = 0;
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			//Invoke ("writeLetterByLetter",initialAnim*2);

			moveCellsToTheFront ();
			movePiecesToFront ();
			moveToFront ();
			tutorialMask.SetActive (true);

			phase = 1;
			previousPhase = 0;
			return true;
		case(1):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [3].SetActive (false);
			phasesPanels [2].SetActive (false);*/
			//phasesPanels [1].SetActive (true);
			currentPhase = 1;
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();
			previousPhase = 1;

			//Invoke ("writeLetterByLetter",initialAnim*1.5f);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (false);
			phasesPanels [3].SetActive (false);*/
			//phasesPanels [2].SetActive (true);
			currentPhase = 2;
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();
			previousPhase = 2;

			//Invoke ("writeLetterByLetter",initialAnim*1.5f);

			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (false);*/
			//phasesPanels [3].SetActive (true);
			currentPhase = 3;
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE4);
			instructionsText = instructions [3];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();
			previousPhase = 3;

			//Invoke ("writeLetterByLetter",initialAnim*1.5f);

			phase = 3;
			return true;
		}

		return base.canMoveToNextPhase ();
	}

	public override bool phaseObjectiveAchived ()
	{
		bool result = false;

		switch (phase) 
		{
		case(1):
		case(2):
		case(3):
			if (lineUsed () && !bLineUsed) 
			{
				bLineUsed = true;
				phase = 3;
				result = true;
				break;
			}
			if (littleLUsed () && !bLittleLUsed) 
			{
				bLittleLUsed = true;
				phase = 1;
				result = true;
				break;
			}
			if (bigLUsed () && !bLUsed) 
			{
				bLUsed = true;
				phase = 2;
				result = true;
				break;
			}
			result = false;
			break;
		}

		if (bLUsed && bLineUsed && bLittleLUsed) 
		{
			Invoke ("ShowBonification",2);
		}

		return result;

		return base.phaseObjectiveAchived ();
	}

	protected void ShowBonification()
	{
		phasesPanels [0].SetActive (false);
		phasesPanels [1].SetActive (false);
		phasesPanels [2].SetActive (false);
		phasesPanels [3].SetActive (false);

		gameManager.callWinBonificationFromTutorial ();

		returnCellsToLayer ();
		returnPieces ();
		returnBack ();

		tutorialMask.SetActive (false);
	}

	protected bool lineUsed()
	{
		List<Piece> temp = pieceManager.getShowingPieces ();

		if (temp.Count == 3) {return true;}

		for (int i = 0; i < temp.Count; i++) 
		{
			if (temp [i].name.Contains("4A1")) 
			{
				return false;
			}
		}

		return true;
	}

	protected bool littleLUsed()
	{
		List<Piece> temp = pieceManager.getShowingPieces ();

		if (temp.Count == 3) {return true;}

		for (int i = 0; i < temp.Count; i++) 
		{
			if (temp [i].name.Contains("3B2")) 
			{
				return false;
			}
		}

		return true;
	}

	protected bool bigLUsed()
	{
		List<Piece> temp = pieceManager.getShowingPieces ();

		if (temp.Count == 3) {return true;}

		for (int i = 0; i < temp.Count; i++) 
		{
			if (temp [i].name.Contains("4D2")) 
			{
				return false;
			}
		}

		return true;
	}
}