using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialLvl22 : TutorialBase 
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
			phaseEvent.Add(ENextPhaseEvent.BLOCK_USED);

			freeBlocks = true;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = false;

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SQUARE_BUTTON);

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV22_PHASE1),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.TAP);

			freeBlocks = false;
			freeBombs = false;
			freeRotates = false;
			freeDestroy = false;
			freeWildCard = false;

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_BUTTON);

			instructions [1].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV22_PHASE2),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});

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
		}

		return base.phaseObjectiveAchived ();
	}
}
