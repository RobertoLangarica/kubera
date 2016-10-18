using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl8 : TutorialBase 
{
	public Image powerUpDommy;
	public GameObject fromPosition;

	protected bool doAnimation;
	public InputBombAndDestroy inputBomb;
	protected Vector3 posFrom;
	protected Vector3 posTo;  
	protected override void Start()
	{
		inputBomb.OnPlayer += animationController;

		base.Start ();
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial8");
			CancelInvoke ("powerUpAnim");
			powerUpDommy.transform.localScale = new Vector3 (1, 1, 1);
			powerUpDommy.color =new Color(1,1,1,0);
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
			if(cellManager == null)
			{
				cellManager = FindObjectOfType<CellsManager> ();
			}
			
			posFrom = fromPosition.transform.position;
			posTo = cellManager.getAllShowedCels()[11].transform.position;

			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			Invoke ("writeLetterByLetter", initialAnim * 2);

			phase = 1;
			return true;
		case(1):
			if(inputBomb == null)
			{
				inputBomb = FindObjectOfType<InputBombAndDestroy> ();
			}
			inputBomb.OnPlayer -= animationController;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.TAP);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_BUTTON);

			freeBombs = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV8_PHASE2),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

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
		}

		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial8",true);
			return;
		}


		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1.4f, 1.4f, 1.4f), 0.5f).SetId("Tutorial8");
		powerUpDommy.DOColor (new Color(1,1,1,0.5f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo,1).OnComplete(
					()=>{

						powerUpDommy.transform.DOScale (new Vector3 (1, 1, 1), 1f).OnComplete(
							()=>{

								powerUpDommy.DOColor (new Color(1,1,1,0),0.5f).SetId("Tutorial8");
							}

						).SetId("Tutorial8");

					}

				).SetId("Tutorial8");

			}
		).SetId("Tutorial8");
		Invoke ("powerUpAnim",3.5f);
	}
}