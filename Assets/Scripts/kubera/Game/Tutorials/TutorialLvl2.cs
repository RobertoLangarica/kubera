using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialLvl2 : TutorialBase 
{
	public List<ArrowAnimation> arrows;

	protected bool doAnimation;

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
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			freeHint = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) 
			{
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE2),
				new string[2]{ "{{points}}", "{{neededPoints}}" }, new string[2]{hudManager.points.text,hudManager.goalText.text.Split('/')[1].Split(' ')[0]});
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			Debug.Log ("CAso2");
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) 
			{
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.MOVEMENTS);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE3),
				new string[1]{"/n"}, new string[1]{"\n"});
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();
			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			arrows[0].startAnimation ();
			
			phase = 3;
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE4);
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();
			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			arrows[0].stopAnimation();
			arrows[1].startAnimation ();

			phase = 4;
			doAnimation = false;
			return true;
		case(4):
			phasesPanels [3].SetActive (false);
			phasesPanels [4].SetActive (true);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.MOVEMENTS);

			currentInstruction =  MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE5),
				new string[3]{"{{b}}","{{/b}}","/n"}, new string[3]{"<b>","</b>","\n"});
			instructionsText = instructions [4];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();
			arrows[1].stopAnimation();
			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);      

			phase = 5;
			doAnimation = false;
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
		case(4):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}
}