using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialRotate : TutorialBase 
{
	public Image powerUpDommy;
	public GameObject fromPosition;

	public Canvas lastPhaseCanvas;
	public GameObject rotateButton;
	protected Transform previousParent;

	public HorizontalLayoutGroup layout;

	protected bool doAnimation;
	protected Vector3 posTo;
	public InputBombAndDestroy inputBomb;

	protected int currentCount = 0;

	protected override void Start()
	{
		inputBomb.OnPlayer += animationController;
		base.Start ();
	}

	protected override void Update()
	{
		base.Update ();

		if (wordManager.letters.Count != currentCount) 
		{
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
			DOTween.Kill ("Tutorial37");
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
			phasesPanels [0].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.ROTATE_USED);

			freeRotates = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.ROTATE_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			//Invoke ("writeLetterByLetter", initialAnim * 2);

			moveCellsToTheFront ();
			movePiecesToFront ();
			moveToFront ();

			previousParent = rotateButton.transform.parent;
			rotateButton.transform.SetParent (transform);

			layout.enabled = false;

			tutorialMask.SetActive (true);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			inputBomb.OnPlayer -= animationController;
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.TAP);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeRotates = true;

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.ROTATE_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV37_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			//Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);
			lastPhaseCanvas.sortingLayerName = "Selected";

			returnCellsToLayer ();
			returnPieces ();
			returnBack ();

			rotateButton.transform.SetParent (previousParent);

			tutorialMask.SetActive (false);

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
			DOTween.Kill ("Tutorial37");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;

		if (posTo == Vector3.zero) 
		{
			posTo = hudManager.rotationImagePositions [1].transform.position;
		}

		print (hudManager.rotationImagePositions [1].transform.position);
		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1f, 1f, 1f), 0.5f).SetId("Tutorial37");
		powerUpDommy.DOColor (new Color(1,1,1,0.5f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial37");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial37");

			}
		).SetId("Tutorial37");

		Invoke ("powerUpAnim",3.5f);
	}
}
