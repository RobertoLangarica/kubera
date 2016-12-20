using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialBomb : TutorialBase
{
	public Image powerUpDommy;
	public GameObject fromPosition;

	public InputBombAndDestroy inputBomb;

	public GameObject bombButton;
	protected Transform previousParent;

	public HorizontalLayoutGroup layout;

	protected bool doAnimation;
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
			DOTween.Kill ("Tutorial52");
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
			phaseEvent.Add (ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV52_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			//Invoke ("writeLetterByLetter", initialAnim * 2);

			activateMask ();

			Debug.Log ("desactivando powerups");
			disablePowerUps ();

			powerUpManager.getPowerupByType (PowerupBase.EType.BOMB).powerUpButton.gameObject.SetActive (true);

			previousParent = bombButton.transform.parent;
			bombButton.transform.SetParent (transform);

			layout.enabled = false;

			tutorialMask.SetActive (true);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);*/
			previousPhase = 0;
			currentPhase = 1;
			phaseEvent.Add (ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.BOMB_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV52_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			deactivateMask ();

			enablePowerUps ();

			bombButton.transform.SetParent (previousParent);

			tutorialMask.SetActive (false);

			phase = 2;
			return true;	
		case(2):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);*/
			previousPhase = 1;
			currentPhase = 2;
			phaseEvent.Add (ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV52_PHASE3),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [2];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

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
		case(2):
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial52");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;

		if (posTo == Vector3.zero) 
		{
			posTo = cellManager.getCellsOfSameType(Piece.EType.PIECE)[2].transform.position;
			posTo.x += cellManager.cellSize;
			posTo.y -= cellManager.cellSize;
		}

		powerUpDommy.transform.position = posFrom;
		powerUpDommy.transform.localScale = Vector3.zero;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1,1,1), 0.5f).SetId("Tutorial52");
		powerUpDommy.DOColor (new Color(1,1,1,0.75f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial52");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial52");

			}
		).SetId("Tutorial52");

		Invoke ("powerUpAnim",3.5f);
	}
}
