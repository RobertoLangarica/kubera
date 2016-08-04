using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialLvl1 : TutorialBase
{
	protected List<Cell> cells;

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
			phaseEvent.Add(ENextPhaseEvent.POSITIONATE_PIECE);

			instructions [0].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE1);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.EMPTY_CELLS);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CREATE_WORD);

			instructions [1].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE2),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.EMPTY_CELLS);

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.SUBMIT_WORD);

			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE3);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SUBMIT_WORD);

			phase = 3;
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CLEAR_A_LINE);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SUBMIT_WORD);

			instructions [3].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE4);
			phase = 4;
			return true;
		case(4):
			phasesPanels [3].SetActive (false);
			phasesPanels [4].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CREATE_A_LINE);

			instructions [4].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE5);
			phase = 5;
			return true;
		case(5):
			phasesPanels [4].SetActive (false);
			phasesPanels [5].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.OBJECTIVE);

			instructions [5].text = MultiLanguageTextManager.instance.multipleReplace (
					MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE6),
				new string[2]{"{{points}}","{{neededPoints}}"}, new string[2]{hudManager.points.text,hudManager.goalText.text.Split('/')[1].Split(' ')[0]});
			
			phase = 6;
			return true;
		case(6):
			phasesPanels [5].SetActive (false);
			phasesPanels [6].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.OBJECTIVE);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [6].text =	MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE7);
				
			phase = 7;
			return true;
		case(7):
			phasesPanels [6].SetActive (false);
			phasesPanels [7].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [7].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE8),
				new string[1]{"/n"}, new string[1]{"\n"});

			phase = 8;
			return true;
		case(8):
			phasesPanels [7].SetActive (false);
			phasesPanels [8].SetActive (true);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [8].text =	MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE9),
				new string[3]{ "{{b}}","{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});

			phase = 9;
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
			if (wordManager.wordsValidator.isCompleteWord ()) 
			{
				return true;
			}
			return false;
		case(3):
			return true;
		case(4):
			if (cellManager.getAvailableVerticalAndHorizontalLines().Count > 0) 
			{
				return true;
			}
			return false;
		case(5):
			return true;
		case(6):
			return true;
		case(7):
			return true;
		case(8):
			return true;
		}
		
		return base.phaseObjectiveAchived ();
	}
}