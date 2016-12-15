using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialDestroy : TutorialBase 
{
	public Image powerUpDommy;
	public GameObject fromPosition;

	public InputBombAndDestroy inputDestroy;

	public Canvas lastPhaseCanvas;
	public GameObject destroyButton;
	protected Transform previousParent;

	public HorizontalLayoutGroup layout;

	protected bool doAnimation;
	protected Vector3 posTo;

	protected int currentCount = 0;

	protected override void Start()
	{
		inputDestroy.OnPlayer += animationController;

		base.Start ();
	}

	protected override void Update()
	{
		base.Update ();

		if (wordManager.letters.Count != currentCount && hasMask)
		{
			Debug.Log ("!!!!!!!!!!!!!!");
			currentCount = wordManager.letters.Count;
			for (int i = 0; i < wordManager.letters.Count; i++) 
			{
				wordManager.letters [i].letterCanvas.overrideSorting = true;
				wordManager.letters [i].letterCanvas.sortingLayerName = "WebView";
			}
		}
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial80");
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
			phaseEvent.Add (ENextPhaseEvent.DESTROY_USED);

			freeDestroy = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.DESTROY_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV80_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			//Invoke ("writeLetterByLetter", initialAnim * 2);

			moveCellsToTheFront ();
			movePiecesToFront ();
			moveToFront ();

			disablePowerUps ();

			powerUpManager.getPowerupByType (PowerupBase.EType.DESTROY).powerUpButton.GetComponent<Button> ().enabled = true;

			previousParent = destroyButton.transform.parent;
			destroyButton.transform.SetParent (transform);

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
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			freeDestroy = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV80_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputDestroy.OnPlayer -= animationController;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			returnCellsToLayer ();
			returnPieces ();
			returnBack ();

			enablePowerUps ();

			tutorialMask.SetActive (false);

			lastPhaseCanvas.sortingLayerName = "Selected";

			destroyButton.transform.SetParent (previousParent);

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
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial80");
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
		powerUpDommy.transform.DOScale (new Vector3 (1,1,1), 0.5f).SetId("Tutorial80");
		powerUpDommy.DOColor (new Color(1,1,1,0.75f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial80");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial80");

			}
		).SetId("Tutorial80");

		Invoke ("powerUpAnim",3.5f);
	}
}