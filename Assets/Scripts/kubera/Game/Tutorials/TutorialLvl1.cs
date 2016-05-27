using UnityEngine;
using System.Collections;

public class TutorialLvl1 : TutorialBase
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
			phaseEvent = ENextPhaseEvent.CREATE_WORD;

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = true;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace(
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE1),
				new string[3]{"'","{{b}}","{{/b}}"},new string[3]{"\"","<b>","</b>"});

			phase = 1;
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
}