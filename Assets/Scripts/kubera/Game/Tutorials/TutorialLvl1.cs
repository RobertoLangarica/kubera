using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialLvl1 : TutorialBase
{
	public PieceManager pieceManager;
	public InputPiece inputPiece;

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
		}

		return base.canMoveToNextPhase ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation || cellManager.getAllEmptyCells().Length < 3 ) 
		{
			DOTween.Kill ("Tutorial1");
			DestroyImmediate(powerUpDommy);
			return;
		}


		posFrom = pieceManager.getShowingPieces () [0].transform.position;
		if (posTo == Vector3.zero) 
		{
			posTo = cellManager.getAllEmptyCells () [4].transform.position;
			posTo.x += cellManager.cellSize;
		}

		changeDommy ();

		Vector3 originalScale = inputPiece.selectedScale;

		powerUpDommy.transform.position = posFrom;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (originalScale.x*1,originalScale.y*1,originalScale.z*1), 0.5f).SetId("Tutorial1");
		powerUpDommy.GetComponent<Piece> ().moveAlphaByTween (0.75f, 0.75f, "Tutorial1",
			() => {

				//TODO: intentar que sea linea curva
				powerUpDommy.transform.DOMove (posTo, 1);

				powerUpDommy.GetComponent<Piece> ().moveAlphaByTween (0, 1, "Tutorial1", () => {
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