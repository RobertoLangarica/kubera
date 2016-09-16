using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl3 : TutorialBase
{
	public Image powerUpDommy;
	public GameObject fromPosition;

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
			phaseEvent.Add (ENextPhaseEvent.HINT_USED);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeHint = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim",1);

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WORD_HINT_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 3;
			doAnimation = false;
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV3_PHASE4),
				new string[3]{ "{{b}}", "{{/b}}", "/n" }, new string[3]{ "<b>", "</b>", "\n" });
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 4;
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

	protected void powerUpAnim()
	{
		if (!doAnimation || cellManager.getAllEmptyCells().Length < 9) 
		{
			DOTween.Kill ("Tutorial3");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;
		Vector3 posTo = cellManager.getAllEmptyCells()[6].transform.position;

		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1.4f, 1.4f, 1.4f), 0.5f).SetId("Tutorial3");
		powerUpDommy.DOColor (new Color(1,1,1,0.5f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo,1).OnComplete(
					()=>{

						powerUpDommy.transform.DOScale (new Vector3 (1, 1, 1), 1f).OnComplete(
							()=>{

								powerUpDommy.DOColor (new Color(1,1,1,0),0.5f).SetId("Tutorial3");
							}

						).SetId("Tutorial3");

					}

				).SetId("Tutorial3");

			}
		).SetId("Tutorial3");

		Invoke ("powerUpAnim",3.5f);
	}
}