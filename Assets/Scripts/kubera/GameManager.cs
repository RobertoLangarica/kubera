﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	public GameObject toBuilderButton;

	//Texto del PoUp
	public Text scoreText;

	public GameObject bonificationPiece;

	public GameObject retryPopUp;
	public GameObject notEnoughLifesPopUp;

	protected int pointsCount = 0;
	protected int wordsMade = 0;
	protected bool wordFound;
	protected List<string> letters;
	protected string[] words;
		
	protected int blackLettersUsed = 0;

	protected string[] myWinCondition;

	protected int totalMoves;
	protected int remainingMoves;

	protected int winBombs;

	public int secondChanceMovements = 5;
	public int secondChanceBombs = 2;
	protected int secondChanceTimes = 0;
	protected int bombsUsed = 0;

	public bool canRotate;
	public bool destroyByColor;

	protected List<Cell> cellToLetter;

	protected PowerUpBase activatedPowerUp;
	protected int currentWildCardsActivated;

	protected PersistentData persistentData;
	protected WordManager wordManager;
	protected CellsManager cellManager;
	protected PowerUpManager powerUpManager;
	protected PieceManager pieceManager;
	protected HUD hud;

	protected InputPieceController inputPiece;

	void Awake () 
	{
		wordManager = FindObjectOfType<WordManager>();
		cellManager = FindObjectOfType<CellsManager>();
		powerUpManager = FindObjectOfType<PowerUpManager> ();
		pieceManager = FindObjectOfType<PieceManager>();
		hud = FindObjectOfType<HUD> ();

		cellManager.OnlinesCounted += linesCreated;
		cellManager.OnLetterCreated += registerNewLetterCreated;

		inputPiece = FindObjectOfType<InputPieceController>();
		inputPiece.OnDrop += OnPieceDropped;

		wordManager.OnSend += sendVectorToCellManager;
	}

	void Start()
	{
		persistentData = FindObjectOfType<PersistentData>();

		myWinCondition = persistentData.currentLevel.winCondition.Split (new char[1]{ '-' });
		cellToLetter = new List<Cell> ();

		myWinCondition = persistentData.currentLevel.winCondition.Split ('-');
		remainingMoves = totalMoves = persistentData.currentLevel.moves;

		hud.setMovments (remainingMoves);


		UserDataManager.instance.playerGems = 300;

		hud.setGems(UserDataManager.instance.playerGems);
		hud.setLevel (persistentData.levelNumber);
		hud.setSecondChanceLock (false);

		powerUpManager.activateAvailablePowers();
		checkIfNeedToUnlockPowerUp();


		float[] scoreToStar = new float[3];
		scoreToStar [0] = PersistentData.instance.currentLevel.scoreToStar1;
		scoreToStar [1] = PersistentData.instance.currentLevel.scoreToStar2;
		scoreToStar [2] = PersistentData.instance.currentLevel.scoreToStar3;
		hud.setMeterData (scoreToStar);

		//print (myWinCondition [0]);
		letters = new List<string> ();
		if(myWinCondition[0] == "letters")
		{
			int i;
			string[] s = myWinCondition [1].Split (',');
			string[] temp;

			for(i=0; i< s.Length; i++)
			{
				temp = s [i].Split (new char[1]{ '_' });
				int amount = int.Parse (temp [0]);

				for(int j=0; j<amount; j++)
				{
					//print (temp [1]);
					letters.Add (temp [1]);
				}
			}
		}
		if(myWinCondition[0] == "word")
		{
			words = myWinCondition [1].Split ('_');
		}

		toBuilderButton.SetActive(false);
		if(persistentData.fromLevelBuilder)
		{
			PersistentData.instance.fromLevelBuilder = false;
			toBuilderButton.SetActive(true);
		}
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.A))
		{
			secondWind();
		}
	}

	private void OnPieceDropped(GameObject piece)
	{
		inputPiece.returnSelectedToInitialState(0.1f);
		inputPiece.reset();
	}

	/*
	 * Se incrementa el puntaje del jugador
	 * 
	 * @params point{int}: La cantidad de puntos que se le entregara al jugador
	 */
	protected void addPoints(int point,bool checkWinConditionBolean = true)
	{
		pointsCount += point;

		hud.setPoints (pointsCount);

		if (checkWinConditionBolean) 
		{
			checkWinCondition ();
		}
	}

	protected bool useGems(int gemsPrice = 0)
	{
		if(gemsPrice == 0 && activatedPowerUp != null)
		{
			gemsPrice = activatedPowerUp.gemsPrice;
		}

		/*if(inputGameController.secondChanceBombsOnly && activatedPowerUp.typeOfPowerUp == EPOWERUPS.DESTROY_NEIGHBORS_POWERUP)
		{
			gemsPrice = 0;
			bombsUsed++;
			if(bombsUsed == secondChanceBombs)
			{
				inputGameController.deactivateSecondChanceLock();
			}
		}*/

		if(checkIfExistEnoughGems(gemsPrice))
		{
			UserDataManager.instance.playerGems -= gemsPrice;

			hud.setGems(UserDataManager.instance.playerGems);

			deactivateCurrentPowerUp();

			Debug.Log("Se cobrara");
			return true;
		}
		Debug.Log("Fondos insuficientes");
		return false;
	}

	/**
	 * checa si existen suficientes gemas para hacer la transaccion
	 **/
	public bool checkIfExistEnoughGems(int gemsPrice)
	{
		if(UserDataManager.instance.playerGems >= gemsPrice)
		{
			return true;
		}
		return false;
	}

	protected void piecePositionatedCorrectly(int length)
	{
		remainingMoves--;

		hud.setMovments (remainingMoves);

		addPoints(length);
	}

	public void activeMoney(bool show,int howMany=0)
	{
		if(show)
		{
			hud.activateChargeGems (true);
			hud.setChargeGems (howMany);
		}
		else
		{
			hud.activateChargeGems (false);	
		}
	}

	protected bool canCompleteWordWithWildCards()
	{
		if (currentWildCardsActivated == 0) 
		{
			return true;
		}

		if(UserDataManager.instance.playerGems >= (powerUpManager.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated) && activatedPowerUp)
		{
			UserDataManager.instance.playerGems -= (powerUpManager.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated);

			if(activatedPowerUp.typeOfPowerUp == EPOWERUPS.WILDCARD_POWERUP)
			{
				deactivateCurrentPowerUp();
			}
			currentWildCardsActivated = 0;
			return true;
		}
		currentWildCardsActivated = 0;
		return false;
	}

	public void verifyWord()
	{
		int amount = 0;
		int multiplierHelper = 1;
		bool letterFound;
		bool canUseAllWildCards;

		canUseAllWildCards = canCompleteWordWithWildCards();

		if(wordManager.words.completeWord && canUseAllWildCards)
		{
			//useGems(powerUpManager.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated);
			
			for(int i = 0;i < wordManager.chars.Count;i++)
			{
				letterFound = false;
				switch(wordManager.chars[i].pointsValue)
				{
				case("x2"):
					{multiplierHelper *= 2;}
					break;
				case("x3"):
					{multiplierHelper *= 3;}
					break;
				case("x4"):
					{multiplierHelper *= 4;}
					break;
				case("x5"):
					{multiplierHelper *= 5;}
					break;
				default:
					{amount += int.Parse(wordManager.chars[i].pointsValue);}
					break;
				}
				if (wordManager.chars [i].typeOfLetter == "0") 
				{
					blackLettersUsed++;
				}
				
				if (myWinCondition [0] == "letters") 
				{
					for (int j = 0; j < letters.Count; j++) {
						if (letters [j].ToLower () == wordManager.chars [i].character.ToLower () && !letterFound) {
							letters.RemoveAt (j);
							letterFound = true;
						}
					}
				}
			}

			if (myWinCondition [0] == "word") 
			{
				for (int j = 0; j < words.Length; j++) 
				{
					if(wordManager.getFullWord().ToLower() == words[j].ToLower())
					{
						wordFound = true;
					}
				}
			}

			amount *= multiplierHelper;

			wordsMade++;
			////FindObjectOfType<InputGameController>().checkToLoose();
		}

		wordManager.resetValidation();
		addPoints(amount);
	}

	protected void sendVectorToCellManager(Vector3 vector3)
	{
		cellManager.getCellOnVec(vector3).clearCell();
	}

	public void linesCreated(int totalLines)
	{
		switch(totalLines)
		{
		case(1):
			{
				addPoints(5);
			}
			break;
		case(2):
			{
				addPoints(15);
			}
			break;
		case(3):
			{
				addPoints(30);
				UserDataManager.instance.playerGems += 1;
			}
			break;
		case(4):
			{
				addPoints(50);
				UserDataManager.instance.playerGems += 2;
			}
			break;
		case(5):
			{
				addPoints(75);
				UserDataManager.instance.playerGems += 4;
			}
			break;
		case(6):
			{
				addPoints(105);
				UserDataManager.instance.playerGems += 6;
			}
			break;
		}
	}

	protected void registerNewLetterCreated(ABCChar abcChar,UIChar uiChar,bool isBlackLetter)
	{
		if(isBlackLetter)
		{
			abcChar.initializeFromScriptableABCChar(pieceManager.giveBlackLetterInfo());
		}
		else
		{
			abcChar.initializeFromScriptableABCChar(pieceManager.giveLetterInfo());
		}

		uiChar.typeOfLetter = abcChar.typeOfLetter;
		uiChar.changeColorAndSetValues(abcChar.character.ToLower ());

		pieceManager.listChar.Add(abcChar);
	}

	/**
	 * Activa el poder rotar a las imagenes por Boton
	 **/
	public void activateRotationByPowerUp()
	{
		/*if(inputGameController.secondChanceBombsOnly)
		{
			return;
		}*/

		pieceManager.activateRotation (true);

		deactivateCurrentPowerUp();
		canRotate = true;
		//inputGameController.setCanRotate (canRotate);
		activatedPowerUp = powerUpManager.getPowerUp(EPOWERUPS.ROTATE_POWERUP);

		activeMoney(true,0);
	}

	public void addWildCardInCurrentWord()
	{
		/*if(inputGameController.secondChanceBombsOnly)
		{
			return;
		}*/

		if (!useGems (powerUpManager.getPowerUp (EPOWERUPS.WILDCARD_POWERUP).gemsPrice)) 
		{
			return;
		}

		deactivateCurrentPowerUp();

		currentWildCardsActivated++;
		wordManager.addCharacter(".");
		wordManager.activateButtonOfWordsActions (true);
		activatedPowerUp = powerUpManager.getPowerUp(EPOWERUPS.WILDCARD_POWERUP);

		//activeMoney(true,activatedPowerUp.gemsPrice);
	}

	public void createOneSquareBlock(Transform myButtonPosition)
	{
		/*if(inputGameController.secondChanceBombsOnly)
		{
			return;
		}*/

		deactivateCurrentPowerUp();

		//inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.BLOCK_POWERUP).oneTilePower(myButtonPosition));
		activatedPowerUp = powerUpManager.getPowerUp(EPOWERUPS.BLOCK_POWERUP);

		activeMoney(true,activatedPowerUp.gemsPrice);
	}

	public void activateDestroyAColorPowerUp(Transform myButtonPosition)
	{
		/*if(inputGameController.secondChanceBombsOnly)
		{
			return;
		}*/

		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbors = false;

		//inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP).activateDestroyMode(myButtonPosition));
		//inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = powerUpManager.getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP);

		activeMoney(true,activatedPowerUp.gemsPrice);
	}

	public void activateDestroyNeighborsOfSameColor(Transform myButtonPosition)
	{
		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbors = true;

		//inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP).activateDestroyMode(myButtonPosition));
		//inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = powerUpManager.getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP);

		/*if(!inputGameController.secondChanceBombsOnly)
		{
			activeMoney(true,activatedPowerUp.gemsPrice);
		}*/
	}

	protected void deactivateCurrentPowerUp()
	{
		if (canRotate) 
		{
			canRotate = false;
			//inputGameController.setCanRotate (canRotate);
		}

		if(destroyByColor)
		{
			destroyByColor = false;
		}

		activeMoney(false);
	}

	protected void setDestroyByColor(bool activate)
	{
		destroyByColor = activate;
	}

	protected void setRotationOfPieces(bool activate)
	{
		canRotate = activate;
		if (!activate) 
		{
			activeMoney (false);
		}
	}

	protected void checkWinCondition ()
	{
		bool win = false;
		switch (myWinCondition[0]) {
		case "points":
			if(pointsCount >= int.Parse( myWinCondition[1]))
			{
				win = true;
			}
			break;

		case "words":
			if (wordsMade >= int.Parse (myWinCondition [1])) 
			{
				win = true;
			}
			break;
		case "letters":
			if (letters.Count == 0) 
			{
				win = true;
			}
			break;
		case "blackLetters":
			if (blackLettersUsed >= int.Parse (myWinCondition [1])) 
			{
				win = true;
			}
			break;
		case "word":
			if (wordFound) 
			{
				win = true;
			}
			break;
		default:
			break;
		}

		if (win) 
		{
			print ("win");
			UnlockPowerUp();
			winBonification ();
		}
		else
		{
			checkToLoose ();
		}
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		FindObjectOfType<WordManager>().checkIfAWordisPossible(pieceManager.listChar);
	}

	public void checkToLoose()
	{
		if(!cellManager.VerifyPosibility(pieceManager.piecesInBar) || remainingMoves == 0)
		{
			Debug.Log ("Perdio");
			while(true)
			{
				bool pass = true;
				for(int i=0; i < pieceManager.listChar.Count; i++)
				{
					if(!pieceManager.listChar[i])
					{
						pieceManager.listChar.RemoveAt(i);
						i--;
						pass = false;
					}
				}
				if(pass)
				{
					break;
				}
			}
			StartCoroutine(check());

		}
	}

	protected void winBonification()
	{
		allowGameInput (false);

		//Se limpian las letras 
		//verifyWord();

		bombsForWinBonification();

		if(cellManager.colorOfMoreQuantity() != EPieceType.NONE)
		{
			add1x1Block();
		}
		else
		{
			destroyAndCountAllLetters();
		}
	}

	protected void add1x1Block()
	{
		Cell[] emptyCells = cellManager.allEmptyCells();
		Cell cell;

		if (remainingMoves != 0 && emptyCells.Length > 0) 
		{
			cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];


			remainingMoves--;
			hud.setMovments (remainingMoves);

			GameObject go = GameObject.Instantiate (bonificationPiece) as GameObject;
			SpriteRenderer sprite = go.GetComponent<Piece> ().pieces [0].GetComponent<SpriteRenderer> ();

			Vector3 nVec = new Vector3 (sprite.bounds.size.x * 0.5f,
				               -sprite.bounds.size.x * 0.5f, 0) + cell.transform.position;

			go.transform.position = nVec;

			go.GetComponent<Piece> ().currentType = cellManager.colorOfMoreQuantity ();

			go.transform.position = cellManager.Positionate (go.GetComponent<Piece> ());

			//cellManager.turnPieceToLetterByWinNotification (cell);
			StartCoroutine (add1x1BlockMore ());
		}
		else 
		{
			cellToLetter.AddRange (cellManager.searchCellsOfSameColor (cellManager.colorOfMoreQuantity ()));
			winBombs--;
			StartCoroutine (addWinLetterAfterBlockMore ());
		}
	}
	IEnumerator add1x1BlockMore()
	{
		yield return new WaitForSeconds (.2f);
		add1x1Block ();
	}

	IEnumerator addWinLetterAfterBlockMore()
	{
		int random = Random.Range (0, cellToLetter.Count);

		if (cellToLetter.Count > 0) 
		{
			cellManager.turnPieceToLetterByWinNotification (cellToLetter [random]);
			cellToLetter [random].pieceType = EPieceType.LETTER;
			cellToLetter.RemoveAt (random);

			yield return new WaitForSeconds (.2f);
			StartCoroutine (addWinLetterAfterBlockMore ());
		}
		else
		{
			useBombs();
		}
	}

	protected void useBombs()
	{
		if(winBombs > 0 && cellManager.colorOfMoreQuantity() != EPieceType.NONE)
		{
			cellToLetter = new List<Cell>();
			cellToLetter = cellManager.searchCellsOfSameColor(cellManager.colorOfMoreQuantity());
			winBombs--;
			StartCoroutine (addWinLetterAfterBlockMore ());
		}
		else
		{
			destroyAndCountAllLetters();
		}
	}

	protected void destroyAndCountAllLetters()
	{
		cellToLetter = new List<Cell>();
		cellToLetter.AddRange (cellManager.searchCellsOfSameColor (EPieceType.LETTER));
		cellToLetter.AddRange (cellManager.searchCellsOfSameColor (EPieceType.LETTER_OBSTACLE));
		winPoints ();
		StartCoroutine (destroyLetter ());
	}

	IEnumerator destroyLetter()
	{
		int random = Random.Range (0, cellToLetter.Count);
		cellToLetter [random].destroyCell ();
		cellToLetter.RemoveAt (random);

		yield return new WaitForSeconds (.2f);
		if (cellToLetter.Count > 0) 
		{			
			StartCoroutine (destroyLetter ());
		}
	}

	protected void winPoints()
	{
		int amount = 0;
		int multiplierHelper = 1;

		for (int i = 0; i < cellToLetter.Count; i++) 
		{
			switch (cellToLetter[i].piece.GetComponent<ABCChar>().pointsValue) 
			{
			case("x2"):
				{
					multiplierHelper *= 2;}
				break;
			case("x3"):
				{
					multiplierHelper *= 3;}
				break;
			case("x4"):
				{
					multiplierHelper *= 4;}
				break;
			case("x5"):
				{
					multiplierHelper *= 5;}
				break;
			default:
				{
					amount += int.Parse (cellToLetter[i].piece.GetComponent<ABCChar>().pointsValue);}
				break;
			}
		}

		amount *= multiplierHelper;
		addPoints(amount,false);
	}

	protected void allowGameInput(bool allowInput)
	{
		inputPiece.allowInput = allowInput;
	}

	protected void bombsForWinBonification()
	{
		if(remainingMoves > 0 && remainingMoves < 4)
		{
			winBombs = 1;
		}
		else if(remainingMoves > 3 && remainingMoves < 7)
		{
			winBombs = 2;
		}
		else if(remainingMoves > 6 && remainingMoves < 10)
		{
			winBombs = 3;
		}
		else if(remainingMoves > 10)
		{
			winBombs = 5;
		}
	}

	protected void checkIfNeedToUnlockPowerUp()
	{
		if(persistentData.currentLevel.unblockBlock)
		{
			powerUpManager.activatePower(EPOWERUPS.BLOCK_POWERUP);
		}
		if(persistentData.currentLevel.unblockBomb)
		{
			powerUpManager.activatePower(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP);
		}
		if(persistentData.currentLevel.unblockDestroy)
		{
			powerUpManager.activatePower(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP);
		}
		if(persistentData.currentLevel.unblockRotate)
		{
			powerUpManager.activatePower(EPOWERUPS.ROTATE_POWERUP);
		}
		if(persistentData.currentLevel.unblockWildcard)
		{
			powerUpManager.activatePower(EPOWERUPS.WILDCARD_POWERUP);
		}
	}

	protected void UnlockPowerUp()
	{
		if(persistentData.currentLevel.unblockBlock)
		{
			UserDataManager.instance.onePiecePowerUpAvailable = true;
		}
		if(persistentData.currentLevel.unblockBomb)
		{
			UserDataManager.instance.destroyNeighborsPowerUpAvailable = true;
		}
		if(persistentData.currentLevel.unblockDestroy)
		{
			UserDataManager.instance.destroyPowerUpAvailable = true;
		}
		if(persistentData.currentLevel.unblockRotate)
		{
			UserDataManager.instance.rotatePowerUpAvailable = true;
		}
		if(persistentData.currentLevel.unblockWildcard)
		{
			UserDataManager.instance.wildCardPowerUpAvailable = true;
		}
	}

	protected void secondWind()
	{
		int secondChancePrice = 0;

		switch(secondChanceTimes)
		{
		case(0):
			secondChancePrice = 10;
			break;
		case(1):
			secondChancePrice = 15;
			break;
		case(2):
			secondChancePrice = 20;
			break;
		default:
			secondChancePrice = 30;
			break;
		}

		if(useGems(secondChancePrice))
		{
			secondChanceTimes++;

			remainingMoves += secondChanceMovements;
			hud.setMovments (remainingMoves);

			//inputGameController.activateSecondChanceLocked();
		}
	}

	public void CancelRetry()
	{
		UserDataManager.instance.takeLifeFromPlayer();
	}

	public void Retry()
	{
		
	}

	public void RefillLifes()
	{
		UserDataManager.instance.refillAllPlayerLifes();
	}

	public void goBackToBuilder()
	{
		PersistentData.instance.fromGameToEdit = true;

		ScreenManager.instance.GoToScene("LevelBuilder");
	}
}
