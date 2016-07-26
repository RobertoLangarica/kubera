﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialLvl64 : TutorialBase 
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
			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			freeWildCard = true;

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_BUTTON);

			instructions [0].text =	MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1),
				new string[3]{ "{{b}}","{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_SPECIFIC_LETER_SELECTED);
			phaseObj = "Z";

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_BUTTON);

			freeWildCard = true;

			instructions [1].text =	MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.TAP);

			freeWildCard = true;

			instructions [2].text =	MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});

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
		}

		return base.phaseObjectiveAchived ();
	}
}