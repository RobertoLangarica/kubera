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

	protected override void Start()
	{
		base.Start ();
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
			Invoke ("powerUpAnim",1);
			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CREATE_WORD);

			instructions [1].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE2),
				new string[2]{"{{b}}", "{{/b}}" }, new string[2]{"<b>","</b>"});

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.PIECES_AREA);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.EMPTY_CELLS);

			doAnimation = false;

			phase = 2;
			return true;
		case(2):
			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.SUBMIT_WORD);

			instructions [2].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE3);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SUBMIT_WORD);

			phase = 3;
			return true;
		case(3):
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CLEAR_A_LINE);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SUBMIT_WORD);

			instructions [3].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE4);
			phase = 4;
			return true;
		case(4):
			phasesPanels [3].SetActive (false);
			phasesPanels [4].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.CREATE_A_LINE);

			instructions [4].text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE5);
			phase = 5;
			return true;
		case(5):
			phasesPanels [4].SetActive (false);
			phasesPanels [5].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.OBJECTIVE);

			instructions [5].text = MultiLanguageTextManager.instance.multipleReplace (
					MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE6),
				new string[2]{"{{points}}","{{neededPoints}}"}, new string[2]{hudManager.points.text,hudManager.goalText.text.Split('/')[1].Split(' ')[0]});
			
			phase = 6;
			return true;
		case(6):
			phasesPanels [5].SetActive (false);
			phasesPanels [6].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.OBJECTIVE);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [6].text =	MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE7);
				
			phase = 7;
			return true;
		case(7):
			phasesPanels [6].SetActive (false);
			phasesPanels [7].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.EARNED_POINTS);

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [7].text = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE8),
				new string[1]{"/n"}, new string[1]{"\n"});

			phase = 8;
			return true;
		case(8):
			phasesPanels [7].SetActive (false);
			phasesPanels [8].SetActive (true);

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.MOVEMENTS);

			instructions [8].text =	MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV1_PHASE9),
				new string[3]{ "{{b}}","{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});

			phase = 9;
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
			if (wordManager.wordsValidator.isCompleteWord ()) 
			{
				return true;
			}
			return false;
		case(3):
			return true;
		case(4):
			if (cellManager.getAvailableVerticalAndHorizontalLines().Count > 0) 
			{
				return true;
			}
			return false;
		case(5):
			return true;
		case(6):
			return true;
		case(7):
			return true;
		case(8):
			return true;
		}
		
		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial1");
			return;
		}

		Vector3 posFrom = pieceManager.getShowingPieces () [0].transform.position;
		Vector3 posTo = cellManager.getAllEmptyCells()[8].transform.position;
		posTo.x += cellManager.cellSize * 0.5f;

		changeDommy ();

		Vector3 originalScale = inputPiece.selectedScale;
		SpriteRenderer tempSpt = powerUpDommy.GetComponent<SpriteRenderer> ();

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
	}
}