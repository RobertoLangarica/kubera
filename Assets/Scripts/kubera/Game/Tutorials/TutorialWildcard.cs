using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialWildcard : TutorialBase 
{
	public KeyBoardManager keyBoard;

	public Image powerUpDommy;
	public GameObject fromPosition;
	public InputBombAndDestroy inputBomb;

	protected bool doAnimation;
	protected Vector3 posTo;

	protected int correctPhase;

	protected string letter_1;
	protected string letter_2;
	protected string letter_3;

	protected override void Start()
	{
		inputBomb.OnPlayer += animationController;

		base.Start ();

		/*letter_1 = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_LETTER1);
		letter_2 = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_LETTER2);
		letter_3 = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_LETTER3);*/
	}

	protected void animationController(bool stop)
	{
		if(stop)
		{
			DOTween.Kill ("Tutorial64");
			CancelInvoke ("powerUpAnim");
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
			phaseEvent.Add (ENextPhaseEvent.WILDCARD_USED);
			phaseEvent.Add(ENextPhaseEvent.WILDCARD_OVER_OBSTACLE);

			freeWildCard = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1),
				new string[2]{ "{{b}}", "{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim", 1);

			Invoke ("writeLetterByLetter", initialAnim * 2);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);

			phasesPanels [1].transform.SetParent (keyBoard.transform);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			//Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [3].SetActive (false);
			phasesPanels [2].SetActive (true);

			phaseEvent.Add(ENextPhaseEvent.WILDCARD_OVER_OBSTACLE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE3),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [2].SetActive (false);
			phasesPanels [0].SetActive (false);
			phasesPanels [3].SetActive (true);

			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE4),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 1;
			return true;
		}
		/*switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			freeWildCard = true;

			firstAnim ();

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE1),
				new string[3]{ "{{b}}","{{/b}}","/n"}, new string[3]{ "<b>", "</b>","\n"});
			instructionsText = instructions [0];
			instructionsText.text = "";

			doAnimation = true;
			Invoke ("powerUpAnim",1);

			Invoke ("writeLetterByLetter",initialAnim*2);

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);

			phasesPanels [1].transform.SetParent (keyBoard.transform);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE2),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [7].SetActive (false);
			phasesPanels [2].SetActive (true);

			phasesPanels [1].transform.SetParent (phasesPanels [0].transform.parent);

			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE3),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = true;
			inputBomb.OnPlayer += animationController;
			Invoke ("powerUpAnim",1);

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);

			phasesPanels [3].transform.SetParent (keyBoard.transform);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE4),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 4;
			return true;
		case(4):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [3].SetActive (false);
			phasesPanels [7].SetActive (false);
			phasesPanels [4].SetActive (true);

			phasesPanels [3].transform.SetParent (phasesPanels [0].transform.parent);

			phaseEvent.Add(ENextPhaseEvent.WILDCARD_USED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE5),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [4];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = true;
			inputBomb.OnPlayer += animationController;
			Invoke ("powerUpAnim",1);

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 5;
			return true;
		case(5):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [4].SetActive (false);
			phasesPanels [5].SetActive (true);

			phasesPanels [5].transform.SetParent (keyBoard.transform);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_BUTTON);

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE6),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [5];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			doAnimation = false;
			inputBomb.OnPlayer -= animationController;

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 6;
			return true;
		case(6):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [5].SetActive (false);
			phasesPanels [7].SetActive (false);
			phasesPanels [6].SetActive (true);

			phasesPanels [5].transform.SetParent (phasesPanels [0].transform.parent);

			phaseEvent.Add(ENextPhaseEvent.SUBMIT_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.multipleReplace (
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE7),
				new string[2]{ "{{b}}","{{/b}}"}, new string[2]{ "<b>", "</b>"});
			instructionsText = instructions [6];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 7;
			return true;
		case(7):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (false);
			phasesPanels [4].SetActive (false);
			phasesPanels [5].SetActive (false);
			phasesPanels [6].SetActive (false);
			phasesPanels [7].SetActive (true);

			phasesPanels [1].transform.SetParent (phasesPanels [0].transform.parent);
			phasesPanels [3].transform.SetParent (phasesPanels [0].transform.parent);
			phasesPanels [5].transform.SetParent (phasesPanels [0].transform.parent);

			phaseEvent.Add(ENextPhaseEvent.KEYBOARD_LETER_SELECTED);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			freeWildCard = true;

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV64_PHASE8);
			instructionsText = instructions [7];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter",shakeDuraion*1.5f);

			phase = 8;
			return true;
		}*/

		return base.canMoveToNextPhase ();
	}

	public override bool phaseObjectiveAchived ()
	{
		switch (phase) 
		{
		case(1):
			if (!keyBoard.isShowing) 
			{
				phase = 3;
			}
			return true;
		case(2):
		case(3):
			return true;
		}

		/*switch (phase) 
		{
		case(1):
		case(3):
		case(5):
			return true;
		case(2):
			if (keyBoard.getSelectedWildCard ().abcChar.character == letter_1) {
				phase = 2;
				return true;
			}
			correctPhase = 1;
			phase = 7;
			return true;
		case(4):
			if (keyBoard.getSelectedWildCard ().abcChar.character == letter_2) 
			{
				phase = 4;
				return true;
			} 
			phase = 7;
			correctPhase = 3;
			return true;
		case(6):
			if (keyBoard.getSelectedWildCard ().abcChar.character == letter_3) 
			{
				phase = 6;
				return true;
			} 
			phase = 7;
			correctPhase = 5;
			return true;
		case(8):
			switch(correctPhase)
			{
			case(1):
				if (keyBoard.getSelectedWildCard ().abcChar.character == letter_1) {
					phase = 2;
					return true;
				}
				return true;
			case(3):
				if (keyBoard.getSelectedWildCard ().abcChar.character == letter_2) 
				{
					phase = 4;
					return true;
				} 
				return true;
			case(5):
				if (keyBoard.getSelectedWildCard ().abcChar.character == letter_3) 
				{
					phase = 6;
					return true;
				} 
				return true;

			}
			return false;
		}*/

		return base.phaseObjectiveAchived ();
	}

	protected void powerUpAnim()
	{
		if (!doAnimation) 
		{
			DOTween.Kill ("Tutorial64");
			return;
		}

		Vector3 posFrom = fromPosition.transform.position;
		Vector3 posTo = Vector3.zero;

		if (phase == 1 && cellManager.getCellsOfSameType(Piece.EType.LETTER).Length != 0) 
		{
			posTo = cellManager.getCellsOfSameType(Piece.EType.LETTER)[0].transform.position;
		}
		if (phase == 3 && cellManager.getCellsOfSameType(Piece.EType.PIECE).Length != 0) 
		{
			posTo = cellManager.getCellsOfSameType(Piece.EType.PIECE)[0].transform.position;
		}
		if (phase == 5 && cellManager.getAllEmptyCells().Length != 0) 
		{
			posTo = cellManager.getAllEmptyCells()[0].transform.position;
		}

		posTo.x += cellManager.cellSize * 0.5f;
		posTo.y -= cellManager.cellSize * 0.5f;

		powerUpDommy.transform.position = posFrom;
		powerUpDommy.transform.localScale = Vector3.zero;

		//Los valores de las animaciones los paso Liloo
		powerUpDommy.transform.DOScale (new Vector3 (1,1,1), 0.5f).SetId("Tutorial64");
		powerUpDommy.DOColor (new Color(1,1,1,0.75f),0.5f).OnComplete(
			()=>{

				//TODO: intentar que sea linea curva

				powerUpDommy.DOColor (new Color(1,1,1,0),1).SetId("Tutorial64");
				powerUpDommy.transform.DOMove (posTo,1).SetId("Tutorial64");

			}
		).SetId("Tutorial64");

		Invoke ("powerUpAnim",3.5f);
	}
}