﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialLvl3 : TutorialBase
{
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
			phaseEvent.Add(ENextPhaseEvent.GRID_SPECIFIC_LETTER_TAPPED);

			allowGridTap = true;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE1),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 1;
			phaseObj = "Z";
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.SUBMIT_WORD);

			allowGridTap = false;
			allowWordTap = false;
			allowLetterDrag = false;
			allowErraseWord = false;
			allowDragPieces = false;
			allowPowerUps = false;

			instructions [1].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE2A);

			instructions [2].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE2B),
				new string[3]{ "'", "{{b}}", "{{/b}}" }, new string[3]{ "\"", "<b>", "</b>" });

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);

			allowGridTap = true;
			allowWordTap = true;
			allowLetterDrag = true;
			allowErraseWord = true;
			allowDragPieces = true;
			allowPowerUps = true;

			instructions [3].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE3),
				new string[1]{ "{{score}}"}, new string[1]{hudManager.goalText.text.Split('/')[1]});
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
}