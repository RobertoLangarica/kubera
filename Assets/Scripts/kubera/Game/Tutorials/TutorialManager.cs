using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
	public Button deleteBtn;
	public Button submitBtn;
	public InputPowerUpRotate inputRotate;
	public KeyBoardManager keyBoard;

	public List<TutorialBase> allTutorials = new List<TutorialBase> ();

	protected TutorialBase currentTutorial;

	private InputBlockPowerUp inputBlock;
	private InputPiece inputPiece;
	private InputBombAndDestroy inputBomb;
	private InputWords inputWords;

	private WordManager wordManager;
	private GameManager gameManager;
	private PowerUpManager powerUpManager;
	private LinesCreatedAnimation linesAnimation;

	void Start()
	{
		inputBlock = FindObjectOfType<InputBlockPowerUp> ();
		inputPiece = FindObjectOfType<InputPiece> ();
		inputBomb = FindObjectOfType<InputBombAndDestroy> ();
		inputWords = FindObjectOfType<InputWords> ();

		wordManager = FindObjectOfType<WordManager> ();

		gameManager = FindObjectOfType<GameManager> ();

		powerUpManager = FindObjectOfType<PowerUpManager> ();

		linesAnimation = FindObjectOfType<LinesCreatedAnimation> ();

		selectTutorial ();
	}

	void Update()
	{
		if (currentTutorial == null) 
		{
			selectTutorial ();
		}

		if (currentTutorial.phaseEvent == TutorialBase.ENextPhaseEvent.TAP && (Input.touchCount >= 1 || Input.GetMouseButtonDown(0))) 
		{
			canCompletePhase ();
		}
	}

	protected void selectTutorial()
	{
		for (int i = 0; i < allTutorials.Count; i++) 
		{
			if (PersistentData.GetInstance().getRandomLevel().name == allTutorials [i].levelName) 
			{
				currentTutorial = allTutorials [i];
				currentTutorial.gameObject.SetActive (true);
			}
		}
	
		if (currentTutorial != null) 
		{
			moveTutorialToNextPhase ();
		} 
		else 
		{
			this.enabled = false;
		}
	}

	public void makeAPowerUpFree(PowerupBase powerUp)
	{
		bool flag = false;

		switch (powerUp.type) 
		{
		case(PowerupBase.EType.BLOCK):
			flag = currentTutorial.freeBlocks;
			break;
		case(PowerupBase.EType.BOMB):
			flag = currentTutorial.freeBombs;
			break;
		case(PowerupBase.EType.DESTROY):
			flag = currentTutorial.freeDestroy;
			break;
		case(PowerupBase.EType.ROTATE):
			flag = currentTutorial.freeRotates;
			break;
		case(PowerupBase.EType.WILDCARD):
			flag = currentTutorial.freeWildCard;
			break;
		}

		powerUp.isFree = flag;
	}

	public void  registerForNextPhase()
	{
		switch (currentTutorial.phaseEvent) 
		{
		case(TutorialBase.ENextPhaseEvent.CREATE_WORD):
			inputWords.onDragFinish += canCompletePhase;
			inputWords.onTap += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.SUBMIT_WORD):
		case(TutorialBase.ENextPhaseEvent.CLEAR_A_LINE):
			submitBtn.onClick.AddListener (foo);
			break;
		case(TutorialBase.ENextPhaseEvent.CREATE_A_LINE):
			linesAnimation.OnCellFlipped += canCompletePhase; 
			break;
		case(TutorialBase.ENextPhaseEvent.WORD_SPECIFIC_LETTER_TAPPED):
			inputWords.onTapToDelete -= wordManager.onLetterTap; 
			inputWords.onTapToDelete += canCompletePhase; 
			break;
		case(TutorialBase.ENextPhaseEvent.GRID_SPECIFIC_LETTER_TAPPED):
			inputWords.onTap -= wordManager.OnGridLetterTapped; 
			inputWords.onTap += canCompletePhase; 
			break;
		case(TutorialBase.ENextPhaseEvent.KEYBOARD_SPECIFIC_LETER_SELECTED):
			keyBoard.OnLetterSelected += canCompletePhase;
			keyBoard.OnLetterSelected -= keyBoard.setLetterToWildCard;
			break;
		case(TutorialBase.ENextPhaseEvent.DELETE_WORD):
			deleteBtn.onClick.AddListener (() => {canCompletePhase();});
			break;
		case(TutorialBase.ENextPhaseEvent.PIECE_ROTATED):
			inputRotate.OnPieceRotated += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.BOMB_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.BOMB).OnPowerupCompleted += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.BLOCK_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.BLOCK).OnPowerupCompleted += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.ROTATE_USED):
			inputRotate.OnRotateArrowsActivated += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.DESTROY_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.DESTROY).OnPowerupCompleted += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.WILDCARD_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.WILDCARD).OnPowerupCompleted += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.POSITIONATE_PIECE):
			gameManager.OnPiecePositionated += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.EARNED_POINTS):
			gameManager.OnPointsEarned += canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.MOVEMENT_USED):
			gameManager.OnMovementRemoved += canCompletePhase;
			break;
		}
	}

	public void unregisterForNextPhase()
	{
		switch (currentTutorial.phaseEvent) 
		{
		case(TutorialBase.ENextPhaseEvent.CREATE_WORD):
			inputWords.onDragFinish -= canCompletePhase;
			inputWords.onTap -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.SUBMIT_WORD):
		case(TutorialBase.ENextPhaseEvent.CLEAR_A_LINE):
			submitBtn.onClick.RemoveListener (foo);
			break;
		case(TutorialBase.ENextPhaseEvent.CREATE_A_LINE):
			linesAnimation.OnCellFlipped -= canCompletePhase; 
			break;
		case(TutorialBase.ENextPhaseEvent.WORD_SPECIFIC_LETTER_TAPPED):
			inputWords.onTapToDelete -= canCompletePhase; 
			inputWords.onTapToDelete += wordManager.onLetterTap; 
			break;
		case(TutorialBase.ENextPhaseEvent.GRID_SPECIFIC_LETTER_TAPPED):
			inputWords.onTap += wordManager.OnGridLetterTapped; 
			inputWords.onTap -= canCompletePhase; 
			break;
		case(TutorialBase.ENextPhaseEvent.KEYBOARD_SPECIFIC_LETER_SELECTED):
			keyBoard.OnLetterSelected -= canCompletePhase;
			keyBoard.OnLetterSelected += keyBoard.setLetterToWildCard;
			break;
		case(TutorialBase.ENextPhaseEvent.DELETE_WORD):
			deleteBtn.onClick.RemoveListener (() => canCompletePhase());
			break;
		case(TutorialBase.ENextPhaseEvent.PIECE_ROTATED):
			inputRotate.OnPieceRotated -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.BOMB_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.BOMB).OnPowerupCompleted -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.BLOCK_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.BLOCK).OnPowerupCompleted -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.ROTATE_USED):
			inputRotate.OnRotateArrowsActivated -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.DESTROY_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.DESTROY).OnPowerupCompleted -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.WILDCARD_USED):
			powerUpManager.getPowerupByType (PowerupBase.EType.WILDCARD).OnPowerupCompleted -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.POSITIONATE_PIECE):
			gameManager.OnPiecePositionated -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.EARNED_POINTS):
			gameManager.OnPointsEarned -= canCompletePhase;
			break;
		case(TutorialBase.ENextPhaseEvent.MOVEMENT_USED):
			gameManager.OnMovementRemoved -= canCompletePhase;
			break;
		}
	}

	public void foo()
	{
		canCompletePhase ();
	}

	public void canCompletePhase(GameObject go,bool byDrag = false)
	{
		if (currentTutorial.phaseEvent == TutorialBase.ENextPhaseEvent.WORD_SPECIFIC_LETTER_TAPPED) 
		{
			if (go.GetComponent<Letter> ().abcChar.character == currentTutorial.phaseObj) 
			{
				wordManager.onLetterTap (go,byDrag);
			}
		}
		if (currentTutorial.phaseEvent == TutorialBase.ENextPhaseEvent.GRID_SPECIFIC_LETTER_TAPPED) 
		{
			if (go.GetComponent<Letter> ().abcChar.character == currentTutorial.phaseObj) 
			{
				wordManager.OnGridLetterTapped (go,byDrag);
			}
		}
		canCompletePhase ();
	}

	public void canCompletePhase(Cell cell,Letter letter)
	{
		canCompletePhase ();
	}

	public void canCompletePhase(string str)
	{
		if (currentTutorial.phaseEvent == TutorialBase.ENextPhaseEvent.KEYBOARD_SPECIFIC_LETER_SELECTED) 
		{
			if (str == currentTutorial.phaseObj) 
			{
				keyBoard.setLetterToWildCard (str);
			}
		}

		canCompletePhase ();
	}

	public void canCompletePhase()
	{
		if (currentTutorial.phaseObjectiveAchived ()) 
		{
			moveTutorialToNextPhase ();
		}
	}

	public void moveTutorialToNextPhase()
	{
		unregisterForNextPhase ();

		currentTutorial.canMoveToNextPhase ();

		registerForNextPhase ();

		for (int i = 0; i < powerUpManager.powerups.Count; i++) 
		{
			makeAPowerUpFree (powerUpManager.powerups[i]);
		}
	}
}