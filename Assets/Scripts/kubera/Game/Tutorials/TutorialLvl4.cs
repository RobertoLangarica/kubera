using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl4 : TutorialBase 
{
	public PieceManager pieceManager;

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
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";
			instructionIndex = 0;

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",initialAnim*1.5f);

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",initialAnim*1.5f);

			phase = 3;
			return true;
		case(3):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV4_PHASE4);
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",initialAnim*1.5f);

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
			if (fourPieceWasUsed ()) 
			{
				phase = 3;
			}
			return true;
		case(2):
			if (fourPieceWasUsed ()) 
			{
				phase = 3;
			}
			return true;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected bool fourPieceWasUsed()
	{
		List<Piece> temp = pieceManager.getShowingPieces ();

		if (temp [0].squares.Length == 2 && temp [1].squares.Length == 2) 
		{
			return true;	
		} 
		else if (temp [0].squares.Length != 2 || temp [1].squares.Length != 2)
		{
			return false;
		}

		return false;
	}
}