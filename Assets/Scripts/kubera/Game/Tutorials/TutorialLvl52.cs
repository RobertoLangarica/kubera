using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialLvl52 : TutorialBase
{
	public Image powerUpDommy;
	public GameObject fromPosition;

	public InputBombAndDestroy inputBomb;

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
			phaseEvent.Add(ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.BOMB_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV52_PHASE1),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim",1);

			Invoke ("writeLetterByLetter",initialAnim*2);

			Sprite[] masksAtlas = Resources.LoadAll<Sprite> ("Masks");
			masks [0].sprite = Sprite.Create(masksAtlas[6].texture,masksAtlas[6].rect,new Vector2(0.5f,0.5f));

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
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
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			masks [0].gameObject.SetActive (false);

			phase = 2;
			return true;	
		case(2):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.BOMB_USED);

			freeBombs = true;

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV52_PHASE3),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

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
