﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl22 : TutorialBase 
{
	public Image powerUpDommy;
	public GameObject fromPosition;
	public InputBombAndDestroy inputBlock;

	protected bool doAnimation;

	protected override void Start()
	{
		inputBlock.OnPlayer += animationController;
		base.Start ();
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial22");
			CancelInvoke ("powerUpAnim");
			powerUpDommy.color = new Color(1,1,1,0);
			doAnimation = false;
		}
		else
		{
			if(!doAnimation)
			{
				doAnimation = true;
				powerUpAnim ();
			}
		}
	}
	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.HINT_USED);

			freeHint = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SQUARE_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV22_PHASE1),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			Invoke ("writeLetterByLetter", initialAnim * 2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.BLOCK_USED);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeHint = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV22_PHASE2),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			doAnimation = false;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

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
		case(2):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation || cellManager.getAllEmptyCells().Length < 3) 
		{
			DOTween.Kill ("Tutorial22");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;
		Vector3 posTo = cellManager.getAllEmptyCells()[3].transform.position;

		posTo.x += cellManager.cellSize;
		posTo.y -= cellManager.cellSize;

		powerUpDommy.transform.position = posFrom;
		powerUpDommy.transform.localScale = Vector3.zero;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1,1,1), 0.5f).SetId("Tutorial22");
		powerUpDommy.DOColor (new Color(1,1,1,0.75f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial22");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial22");

			}
		).SetId("Tutorial22");

		Invoke ("powerUpAnim",3.5f);
	}
}
