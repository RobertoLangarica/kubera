using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Tiene un hardcoding en GameManager dónde su utiliza el nombre del nivel para hacerle modificaciones
 * 
*/
public class TutorialLvl7 : TutorialBase 
{
	protected bool isWaitingForText;
	protected string tutorialWord;

	protected int count = 0;
	protected int currentCount = 0;

	public int wordTimer = 5;

	protected override void Start ()
	{
		base.Start ();

		tutorialWord = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_WORD);
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

	public override bool canMoveToNextPhase ()
	{
		phaseEvent.Clear ();

		int total = int.Parse (hudManager.goalText.text.Split ('/') [1].Split (' ') [0]);
		int current = int.Parse (hudManager.goalText.text.Split('/')[0]);

		switch (phase) 
		{
		case(0):
			phasesPanels [0].SetActive (true);
			previousPhase = 0;
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE1).Replace ("{{score}}",
				hudManager.goalText.text.Split('/')[1].Split(' ')[0]);
			currentInstruction = currentInstruction.Replace ("/n","\n");
			instructionsText = instructions [0];
			instructionsText.text = currentInstruction;

			//Invoke ("writeLetterByLetter",initialAnim*2);

			moveCellsToTheFront ();
			movePiecesToFront ();
			moveToFront ();
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
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [0].SetActive (false);
			phasesPanels [2].SetActive (true);*/
			previousPhase = 0;
			currentPhase = 2;
			phaseEvent.Add (ENextPhaseEvent.SELECT_LETTER);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE2).Replace ("{{neededScore}}", (total - current).ToString ());
			instructionsText = instructions [2];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			//CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			/*phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);*/
			previousPhase = 2;
			currentPhase = 3;
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE3).Replace ("{{neededScore}}", (total - current).ToString ());
			currentInstruction = currentInstruction.Replace ("/n", "\n");
			instructionsText = instructions [3];
			instructionsText.text = currentInstruction;
			instructionIndex = 0;

			shakeToErrase ();

			//Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			isWaitingForText = true;
			OnWritingFinished ();

			phase = 4;
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
		case(4):
			phasesPanels [3].SetActive (false);

			returnCellsToLayer ();
			returnPieces ();
			returnBack ();

			tutorialMask.SetActive (false);
			return true;
		}

		return base.phaseObjectiveAchived ();
	}

	void OnDestroy()
	{
		CancelInvoke ();
	}

	protected override void OnWritingFinished ()
	{
		if (isWaitingForText) 
		{
			isWaitingForText = false;
			Invoke ("getTutorialWord",wordTimer);
		}

		base.OnWritingFinished ();
	}

	protected void getTutorialWord()
	{
		List<Letter> result = new List<Letter> ();
		List<Letter> letters = gameManager.getGridCharacters ();

		for (int i = 0; i < tutorialWord.Length; i++) 
		{
			for (int j = 0; j < letters.Count; j++) 
			{
				if (tutorialWord [i] == letters [j].abcChar.character[0]) 
				{
					result.Add (letters[j]);
					letters.RemoveAt (j);
					break;
				}
			}
		}

		wordManager.cancelHint = false;
		wordManager.updateGridLettersState (result, WordManager.EWordState.HINTED_WORDS);
	}
}
