using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl64 : TutorialBase 
{
	public KeyBoardManager keyBoard;

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
			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			freeWildCard = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_POWERUP);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1),
				new string[3]{ "{{b}}","{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});
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

			phasesPanels [1].transform.SetParent (keyBoard.transform);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_BUTTON);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_POWERUP);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);

			phasesPanels [1].transform.SetParent (phasesPanels [1].transform.parent);

			phaseEvent.Add(ENextPhaseEvent.SUBMIT_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE3),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 3;
			return true;
		case(3):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.TAP);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE4),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

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
			return true;
		case(2):
			Debug.Log (keyBoard.getSelectedWildCard ().abcChar.character);
			if (keyBoard.getSelectedWildCard ().abcChar.character == "Z") 
			{
				phase = 3;
				return true;
			} 
			else 
			{
				return true;
			}
			return false;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial64");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;
		Vector3 posTo = wordManager.letterContainer.transform.position;

		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1.4f, 1.4f, 1.4f), 0.5f).SetId("Tutorial64");
		powerUpDommy.DOColor (new Color(1,1,1,0.5f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo,1).OnComplete(
					()=>{

						powerUpDommy.transform.DOScale (new Vector3 (1, 1, 1), 1f).OnComplete(
							()=>{

								powerUpDommy.DOColor (new Color(1,1,1,0),0.5f).SetId("Tutorial64");
							}

						).SetId("Tutorial64");

					}

				).SetId("Tutorial64");

			}
		).SetId("Tutorial64");

		Invoke ("powerUpAnim",3.5f);
	}
}