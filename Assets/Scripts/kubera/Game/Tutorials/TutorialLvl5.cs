﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl5 : TutorialBase
{
	public string tutorialWord = "FELIZ";

	protected bool isWaitingForText;

	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);

			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV5_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV5_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			isWaitingForText = true;

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.HINT_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV5_PHASE3);
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
			return true;
		case(2):
			return true;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected override void OnWritingFinished ()
	{
		if (isWaitingForText) 
		{
			isWaitingForText = false;
			Invoke ("getTutorialWord",1);
		}

		base.OnWritingFinished ();
	}

	protected void getTutorialWord()
	{
		List<Letter> result = new List<Letter> ();
		List<Letter> letters = gameManager.getGridCharacters ();

		for (int i = 0; i < letters.Count; i++) 
		{
			Letter temp = letters [i];
			if (temp.abcChar.character != "W") 
			{
				result.Add (temp);
			}
		}

		letters.Clear ();

		for (int i = 0; i < tutorialWord.Length; i++) 
		{
			for (int j = 0; j < result.Count; j++) 
			{
				if (tutorialWord [i] == result [j].abcChar.character[0]) 
				{
					letters.Add (result[j]);
					break;
				}
			}
		}

		wordManager.cancelHint = false;
		wordManager.updateGridLettersState (letters, WordManager.EWordState.HINTED_WORDS);
	}
}
