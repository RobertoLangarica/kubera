using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialBlock : TutorialBase 
{
	public Image powerUpDommy;
	public GameObject fromPosition;
	public InputBlockPowerUp inputBlock;

	public GameObject blockButton;
	protected Transform previousParent;

	public HorizontalLayoutGroup layout;

	protected bool doAnimation;
	protected Vector3 posTo;

	protected int currentCount = 0;

	protected override void Start()
	{
		inputBlock.OnPlayer += animationController;
		base.Start ();
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial13");
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
			previousPhase = 0;
			phaseEvent.Add (ENextPhaseEvent.BLOCK_USED);

			freeBlocks = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SQUARE_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV13_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			//Invoke ("writeLetterByLetter", initialAnim * 2);

			activateMask ();

			disablePowerUps ();

			powerUpManager.getPowerupByType (PowerupBase.EType.BLOCK).powerUpButton.gameObject.SetActive(true);

			previousParent = blockButton.transform.parent;
			blockButton.transform.SetParent (transform);

			layout.enabled = false;

			tutorialMask.SetActive (true);

			phase = 2;
			return true;
		case(1):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);*/
			previousPhase = 0;
			currentPhase = 1;
			phaseEvent.Add (ENextPhaseEvent.BLOCK_USED);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeBlocks = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV13_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			doAnimation = false;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			phase = 2;
			return true;		
		case(2):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);*/
			previousPhase = 0;
			currentPhase = 2;
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			freeBlocks = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV13_PHASE3),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [2];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			deactivateMask ();

			enablePowerUps ();

			blockButton.transform.SetParent (previousParent);

			tutorialMask.SetActive (false);

			doAnimation = false;
			inputBlock.OnPlayer -= animationController;

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

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial13");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;

		if (posTo == Vector3.zero  && cellManager.getAllEmptyCells().Length >= 4) 
		{
			posTo = cellManager.getAllEmptyCells () [3].transform.position;
			posTo.x += cellManager.cellSize;
			posTo.y -= cellManager.cellSize;
		}

		powerUpDommy.transform.position = posFrom;
		powerUpDommy.transform.localScale = Vector3.zero;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1,1,1), 0.5f).SetId("Tutorial13");
		powerUpDommy.DOColor (new Color(1,1,1,0.75f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial13");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial13");

			}
		).SetId("Tutorial13");

		Invoke ("powerUpAnim",3.5f);
	}
}