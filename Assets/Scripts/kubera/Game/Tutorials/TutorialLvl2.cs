using UnityEngine;
using System.Collections;

public class TutorialLvl2 : TutorialBase 
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
			phaseEvent.Add (ENextPhaseEvent.HINT_USED);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeHint = true;

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			instructions [0].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE1),
				new string[2]{"{{b}}","{{/b}}"}, new string[2]{"<b>","</b>"});

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeHint = true;

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			instructions [1].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>"});

			phase = 2;
			return true;
		case(2):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);

			freeHint = true;

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE3);
			
			phase = 3;
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);

			freeHint = true;

			instructions [3].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE4),
				new string[3]{ "{{b}}", "{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});
			
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
				phase = 2;
			}
			return true;
		case(2):
			return true;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}
}