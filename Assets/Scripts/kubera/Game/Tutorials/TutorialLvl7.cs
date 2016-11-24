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

	protected override void Start ()
	{
		base.Start ();

		tutorialWord = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_WORD);
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
			phaseEvent.Add (ENextPhaseEvent.TAP);

			firstAnim ();

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE1).Replace ("{{score}}",
				hudManager.goalText.text.Split('/')[1].Split(' ')[0]);
			instructionsText = instructions [0];
			instructionsText.text = "";

			Invoke ("writeLetterByLetter",initialAnim*2);

			Sprite[] masksAtlas = Resources.LoadAll<Sprite> ("Masks");
			masks [0].sprite = Sprite.Create(masksAtlas[3].texture,masksAtlas[3].rect,new Vector2(0.5f,0.5f));

			phase = 1;
			return true;
		case(1):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [0].SetActive (false);
			phasesPanels [1].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.POSITIONATE_PIECE);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE2);
			instructionsText = instructions [1];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			masks [0].gameObject.SetActive (false);

			phase = 2;
			return true;
		case(2):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [1].SetActive (false);
			phasesPanels [2].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.SELECT_LETTER);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE3).Replace("{{neededScore}}",(total-current).ToString());
			instructionsText = instructions [2];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			phase = 3;
			return true;
		case(3):
			//Deteniendo escritura previa
			CancelInvoke ("writeLetterByLetter");
			isWriting = false;

			phasesPanels [2].SetActive (false);
			phasesPanels [3].SetActive (true);
			phaseEvent.Add (ENextPhaseEvent.SUBMIT_WORD);

			if (instructionIndex < currentInstruction.Length) {
				changeInstruction = true;
				foundStringTag = false;
			}

			currentInstruction = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.TUTORIAL_LV7_PHASE4).Replace("{{neededScore}}",(total-current).ToString());
			instructionsText = instructions [3];
			instructionsText.text = "";
			instructionIndex = 0;

			shakeToErrase ();

			Invoke ("writeLetterByLetter", initialAnim * 1.5f);

			isWaitingForText = true;

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
			count++;
			if (count < 2) 
			{
				return false;
			}
			return true;
		case(3):
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
			Invoke ("getTutorialWord",12);
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
