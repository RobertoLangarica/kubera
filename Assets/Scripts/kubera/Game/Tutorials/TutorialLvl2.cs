using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialLvl2 : TutorialBase 
{
	public PieceManager pieceManager;
	public InputPiece inputPiece;

	protected bool doAnimation;
	protected bool isWaitingForText;

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
			DOTween.Kill ("Tutorial2");
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
			phaseEvent.Add (ENextPhaseEvent.CREATE_A_LINE);

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE1);
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.ALL_PIECES_USED);

			if (instructionIndex < currentInstruction.Length) 
			{
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.WILDCARD_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE3);
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();
			Invoke ("writeLetterByLetter", shakeDuraion * 1.5f);

			isWaitingForText = true;
			
			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV2_PHASE4);
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
			return true;
		case(2):
			return true;
		case(3):
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	protected override void OnWritingFinished ()
	{
		if (isWaitingForText) 
		{
			isWaitingForText = false;
			Invoke ("showPieces",1);
		}

		base.OnWritingFinished ();
	}

	protected void showPieces()
	{
		pieceManager.initializePiecesToShow ();
		hudManager.showPieces (pieceManager.getShowingPieces ());

		if (AudioManager.GetInstance ()) {
			AudioManager.GetInstance ().Stop ("pieceCreated");
			AudioManager.GetInstance ().Play ("pieceCreated");
		}

		canMoveToNextPhase ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation || cellManager.getAllEmptyCells().Length < 7 ) 
		{
			DOTween.Kill ("Tutorial2");
			DestroyImmediate(powerUpDommy);
			return;
		}


		posFrom = pieceManager.getShowingPieces () [0].transform.position;
		if (posTo == Vector3.zero) 
		{
			posTo = cellManager.getAllEmptyCells () [2].transform.position;
			posTo.x += cellManager.cellSize;
		}

		changeDommy ();

		Vector3 originalScale = inputPiece.selectedScale;

		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (originalScale.x*1,originalScale.y*1,originalScale.z*1), 0.5f).SetId("Tutorial2");
		powerUpDommy.GetComponent<Piece> ().moveAlphaByTween (0.75f, 0.75f, "Tutorial2",
			() => {

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo, 1);

				powerUpDommy.GetComponent<Piece> ().moveAlphaByTween (0, 1, "Tutorial2", () => {
					DestroyImmediate (powerUpDommy);
				});
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