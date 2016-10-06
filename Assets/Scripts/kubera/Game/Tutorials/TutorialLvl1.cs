using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialLvl1 : TutorialBase
{
	public PieceManager pieceManager;
	public InputPiece inputPiece;
	public ArrowAnimation arrow;

	protected bool doAnimation;
	protected int dommyIndex = -1;
	protected GameObject powerUpDommy;
	protected List<Cell> cells;

	protected Vector3 posFrom;
	protected Vector3 posTo;

	protected override void Start()
	{
		inputPiece.OnPlayer += animationController;

		base.Start ();
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial1");
			DestroyImmediate(powerUpDommy);
			CancelInvoke ("powerUpAnim");

			doAnimation = false;
		}
		else
		{
			if(!doAnimation)
			{
				doAnimation = true;
				Invoke ("powerUpAnim", 0.2f);
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
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.EMPTY_CELLS);

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			inputPiece.OnPlayer -= animationController;
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.CREATE_WORD);

			if (instructionIndex < currentInstruction.Length) 
			{
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE2),
				new string[2]{ "{{b}}", "{{/b}}" }, new string[2]{ "<b>", "</b>" });
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.EMPTY_CELLS);

			doAnimation = false;
			DOTween.Kill ("Tutorial1");
			DestroyImmediate(powerUpDommy);

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("highLightSubmitButton",1.5f);

			Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			arrow.startAnimation ();

			phase = 3;
			return true;
		}

		return base.canMoveToNextPhase ();
	}

	protected void highLightSubmitButton()
	{
		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SUBMIT_WORD);
	}

	public override bool phaseObjectiveAchived ()
	{
		switch (phase) 
		{
		case(1):
			return true;
		case(2):
			if (wordManager.wordsValidator.isCompleteWord ()) 
			{
				return true;
			}
			return false;
		}
		
		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation || cellManager.getAllEmptyCells().Length < 11 ) 
		{
			DOTween.Kill ("Tutorial1");
			DestroyImmediate(powerUpDommy);
			return;
		}


		posFrom = pieceManager.getShowingPieces () [0].transform.position;
		posTo = cellManager.getAllEmptyCells()[8].transform.position;
		posTo.x += cellManager.cellSize * 0.5f;

		changeDommy ();

		Vector3 originalScale = inputPiece.selectedScale;

		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (originalScale.x*1.2f,originalScale.y*1.2f,originalScale.z*1.2f), 0.5f).SetId("Tutorial1");
		powerUpDommy.GetComponent<Piece>().moveAlphaByTween(0.5f,0.5f,"Tutorial1",
			()=>{

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo,1).OnComplete(
					()=>{

						powerUpDommy.transform.DOScale (new Vector3 (originalScale.x,originalScale.y,originalScale.z), 1f).OnComplete(
							()=>{

								powerUpDommy.GetComponent<Piece>().moveAlphaByTween(0,0.5f,"Tutorial1",()=>{DestroyImmediate(powerUpDommy);});
							}

						).SetId("Tutorial1");

					}

				).SetId("Tutorial1");

			});

		Invoke ("powerUpAnim",3.5f);
	}

	protected void changeDommy()
	{
		if (dommyIndex < 2) 
		{
			dommyIndex++;
		} 
		else 
		{
			dommyIndex = 0;
		}
		powerUpDommy = GameObject.Instantiate (pieceManager.getShowingPieces () [dommyIndex].gameObject) as GameObject;
		powerUpDommy.transform.localScale = Vector3.zero;
		powerUpDommy.GetComponent<Collider2D> ().enabled = false;
	}
}