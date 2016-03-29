using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	public Text scoreText;
	public GameObject retryPopUp;
	public GameObject notEnoughLifesPopUp;

	public GameObject bonificationPiecePrefab;

	protected int pointsCount = 0;

	protected int totalMoves;
	protected int remainingMoves;

	public int secondChanceMovements = 5;
	public int secondChanceBombs = 2;
	protected int secondChanceTimes = 0;
	protected int bombsUsed = 0;
	public float piecePositionedDelay = 0.1f;

	public int consonantPoints= 3;
	public int vocalPoints = 1; 

	public Transform gridLettersContainer;
	public float gridLettersSizeMultiplier = 0.9f;

	protected List<Cell> cellToLetter;

	protected int currentWildCardsActivated;

	private WordManager		wordManager;
	private CellsManager	cellManager;
	private PowerUpManager	powerupManager;
	private PieceManager 	pieceManager;
	private HUDManager 	 	hudManager;
	private AudioManager 	audioManager;
	private InputPiece 		inputPiece;
	private InputWords 		inputWords;
	private GoalManager		goalManager;

	private Level currentLevel;
	private List<Letter> gridCharacters = new List<Letter>();


	void Start()
	{
		wordManager = FindObjectOfType<WordManager>();
		wordManager.setMaxAllowedLetters(PersistentData.instance.maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;
		//LetterSize
		Vector3 cellSizeReference = cellManager.cellPrefab.GetComponent<SpriteRenderer>().bounds.size;
		Vector3 lettersizeDelta = (Camera.main.WorldToScreenPoint(cellSizeReference) -Camera.main.WorldToScreenPoint(Vector3.zero)) * gridLettersSizeMultiplier;
		lettersizeDelta.x = Mathf.Abs(lettersizeDelta.x);
		lettersizeDelta.y = Mathf.Abs(lettersizeDelta.y);
		wordManager.gridLettersSizeDelta = new Vector2(lettersizeDelta.x, lettersizeDelta.y);

		cellManager = FindObjectOfType<CellsManager>();

		powerupManager = FindObjectOfType<PowerUpManager>();
		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		pieceManager = FindObjectOfType<PieceManager>();

		hudManager = FindObjectOfType<HUDManager> ();

		audioManager = FindObjectOfType<AudioManager>();

		inputPiece = FindObjectOfType<InputPiece>();
		inputPiece.OnDrop += OnPieceDropped;

		inputWords = FindObjectOfType<InputWords>();

		goalManager = FindObjectOfType<GoalManager>();
		goalManager.currentCondition = "hola";
		goalManager.OnGoalAchieved += OnLevelGoalAchieved;

		//TODO: Leer las gemas de algun lado
		UserDataManager.instance.playerGems = 300;

		//TODO: el release no manda un random
		if(PersistentData.instance.currentLevel == null)
		{
			configureLevel(PersistentData.instance.getRandomLevel());
		}
		else
		{
			configureLevel(PersistentData.instance.currentLevel);	
		}
	}

	private void configureLevel(Level level)
	{
		currentLevel = level;

		readLettersFromLevel(level);

		pieceManager.initializePiecesFromCSV(level.pieces);
		pieceManager.initializePiecesToShow ();
		hudManager.showPieces (pieceManager.getShowingPieces ());

		cellToLetter = new List<Cell> ();

		goalManager.initializeFromString(currentLevel.goal);

		remainingMoves = totalMoves = currentLevel.moves;

		hudManager.setGems(UserDataManager.instance.playerGems);
		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		allowGameInput(false);

		float[] scoreToStar = new float[3];
		scoreToStar [0] = currentLevel.scoreToStar1;
		scoreToStar [1] = currentLevel.scoreToStar2;
		scoreToStar [2] = currentLevel.scoreToStar3;
		hudManager.setStarsData (scoreToStar);
		hudManager.setPoints(0);
	
		cellManager.resizeGrid(10,10);
		populateGridFromLevel(level);

		getWinCondition();
		actualizeHUDInfo();
	}

	protected void readLettersFromLevel(Level level)
	{
		wordManager.initializePoolFromCSV(level.lettersPool,WordManager.EPoolType.NORMAL);

		if(level.obstacleLettersPool.Length > 0)
		{
			wordManager.initializePoolFromCSV(level.obstacleLettersPool,WordManager.EPoolType.OBSTACLE);
		}

		if(level.tutorialLettersPool.Length > 1)
		{
			wordManager.initializePoolFromCSV(level.tutorialLettersPool.Split('-')[0],WordManager.EPoolType.TUTORIAL);
		}
	}

	protected void populateGridFromLevel(Level level)
	{
		GameObject cellContent = null;
		string[] levelGridData = level.grid.Split(',');
		int cellType = 0;

		List<Letter> tutorialLetters = new List<Letter>();

		for(int i = 0;i < levelGridData.Length;i++)
		{
			cellType = int.Parse(levelGridData[i]);

			cellManager.setCellType (i, cellType);

			//Inicializamos el contenido
			if((cellType & 0x2) == 0x2)
			{
				//Cuadro de color
				Piece content = pieceManager.getSingleSquarePiece(cellType>>6);
				cellManager.occupyAndConfigureCell(i,content.gameObject,content.currentType,true);
			}
			else if((cellType & 0x8) == 0x8)
			{	
				//Obstaculo
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.OBSTACLE);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,Piece.EType.LETTER_OBSTACLE,true);
				obstaclesCount++;

				gridCharacters.Add(letter);
			}
			else if((cellType & 0x20) == 0x20)
			{	
				//De tutorial
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.TUTORIAL);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,Piece.EType.LETTER,true);
				tutorialLetters.Add(letter);

				gridCharacters.Add(letter);
			}
		}

		if(tutorialLetters.Count > 0)
		{
			selectLettersFromGrid(tutorialLetters, level.tutorialLettersPool.Split('-')[1].Split(','));
		}
	}

	protected void selectLettersFromGrid(List<Letter> gridLetters,string[] charsToLookAt)
	{
		for(int i = 0; i < charsToLookAt.Length; i++)
		{
			for(int j = 0;j < gridLetters.Count;j++)
			{
				if(!gridLetters[j].isPreviouslySelected() && gridLetters[j].abcChar.character == charsToLookAt[i])
				{
					wordManager.addLetterFromGrid(gridLetters[j]);
					break;
				}
			}	
		}
	}

	void Update()
	{
		hudManager.setLettersPoints (wordManager.wordPoints);
	}

	private void OnPieceDropped(GameObject obj)
	{
		Piece piece = obj.GetComponent<Piece>();

		if(!tryToDropOnGrid(piece))
		{
			inputPiece.returnSelectedToInitialState (0.1f);
		}

		inputPiece.reset();
	}

	public bool tryToDropOnGrid(Piece piece)
	{
		if (cellManager.canPositionateAll (piece.squares)) 
		{
			putPiecesOnGrid (piece);
			audioManager.PlayPiecePositionedAudio();
			List<List<Cell>> cells = cellManager.getCompletedVerticalAndHorizontalLines ();
			//Puntos por las lineas creadas
			linesCreated (cells.Count);
			convertLinesToLetters(cells);
			StartCoroutine(afterPiecePositioned(piece));
			actualizeHUDInfo ();
			return true;
		}

		return false;
	}

	private void putPiecesOnGrid(Piece piece)
	{
		List<Cell> cells = new List<Cell> ();
		cells = cellManager.getCellsUnderPiece (piece);
		Vector3 piecePosition;

		for(int i=0; i< cells.Count; i++)
		{ 
			cellManager.occupyAndConfigureCell (cells [i], piece.squares [i], piece.currentType);

			//Cada cuadro reeparentado para dejar de usar su contenedor actual
			//y manipularlo individualmente
			piece.squares[i].transform.SetParent(piece.transform.parent);

			piecePosition =  cells[i].transform.position + (new Vector3 (cells[i].GetComponent<SpriteRenderer> ().bounds.extents.x,
				-cells[i].GetComponent<SpriteRenderer> ().bounds.extents.y, 0));
			
			piece.squares[i].transform.DOMove (piecePosition, piecePositionedDelay);
		}

		//Solo se posicionan los cuadros de la pieza
		/*for(int i = 0;i < piece.squares.Length;i++)
		{
			piece.squares[i].transform.SetParent(piece.transform.parent);
		}*/
	}

	IEnumerator afterPiecePositioned(Piece piece)
	{
		yield return new WaitForSeconds (piecePositionedDelay*1.05f);

		if(pieceManager.isAShowedPiece(piece))
		{
			pieceManager.removeFromShowedPieces (piece);

			if (pieceManager.getShowingPieces ().Count == 0) 
			{
				pieceManager.initializePiecesToShow ();
				hudManager.showPieces (pieceManager.getShowingPieces ());
			}

			//Damos puntos por cada cuadro en la pieza
			onActionDone(piece.squares.Length);
			showScoreTextOnHud (piece.transform.position, piece.squares.Length);
		}

		Destroy(piece.gameObject);
	}

	private void convertLinesToLetters(List<List<Cell>> cells)
	{
		for (int i = 0; i < cells.Count; i++) 
		{
			for(int j=0; j<cells[i].Count; j++)
			{
				if (cells [i] [j].contentType != Piece.EType.LETTER) 
				{
					Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL);

					Vector3 cellPosition =  cells [i] [j].transform.position + (new Vector3 (cells [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x,
						-cells [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

					cellManager.occupyAndConfigureCell (cells [i] [j], letter.gameObject, Piece.EType.LETTER);
					letter.gameObject.transform.DOMove (cellPosition, 0);
					gridCharacters.Add(letter);
				}
			}
		}
	}

	//TODO: checar el nombre de la funcion
	protected void onActionDone(int pointsForTheAction,int movementsUsed = 1)
	{
		addPoints (pointsForTheAction);
		substractMoves(movementsUsed);
		actualizeHUDInfo ();

		if (isGoalAchieved()) 
		{
			gameWon ();
		}
		else
		{
			checkIfLoose ();
		}
	}

	protected void addPoints(int amount)
	{
		pointsCount += amount;
	}

	protected void substractMoves(int amount)
	{
		remainingMoves-=amount;
	}

	public void activeMoney(bool show,int howMany=0)
	{
		if(show)
		{
			hudManager.activateChargeGems (true);
			hudManager.setChargeGems (howMany);
		}
		else
		{
			hudManager.activateChargeGems (false);	
		}
	}

	public void OnDeleteWord()
	{
		wordManager.removeAllLetters(false);
	}

	public void OnRetrieveWord()
	{
		//Contamos obstaculos y si la meta es usar letras entonces vemos si se usan
		for(int i = 0;i < wordManager.letters.Count;i++)
		{
			if (wordManager.letters[i].type == Letter.EType.OBSTACLE) 
			{
				obstaclesUsed++;
			}

			//TODO: Hay que declarar las winConditions como constantes GOAL_LETTERS
			if (myWinCondition [0] == "letters") 
			{
				for (int j = 0; j < goalLetters.Count; j++) 
				{
					if (goalLetters [j] == wordManager.letters [i].abcChar.character) 
					{
						print (goalLetters [j]);
						print ( wordManager.letters [i].abcChar.character);

						hudManager.destroyLetterFound (goalLetters [j]);

						goalLetters.RemoveAt (j);
						break;//Solo se cuenta una vez
					}
				}
			}
		}

		if (myWinCondition [0] == "word" || myWinCondition [0] == "ant"|| myWinCondition [0] == "sin") 
		{
			for (int j = 0; j < goalWords.Count; j++) 
			{
				if(wordManager.getCurrentWordOnList().ToLower() == goalWords[j].ToLower())
				{
					wordFound = true;
				}
			}
		}

		wordsMade++;

		//Los puntos se leen antes de limpiar porque sin letras no hay puntos
		onActionDone (wordManager.wordPoints);
		removeLettersFromGrid(wordManager.letters, true);

		wordManager.removeAllLetters();

	}

	/**
	 * @param letters : Listado de letras
	 * @param useReferenceInstead : Elimina las letras que se tengan de referencia y no las de la lista 
	 * */
	private void removeLettersFromGrid(List<Letter> letters, bool useReferenceInstead = false)
	{
		Letter letter;
		for(int i = 0; i < letters.Count; i++)
		{
			if(useReferenceInstead)
			{
				letter = letters[i].letterReference;
			}
			else
			{
				letter = letters[i];
			}

			if(letter != null)
			{
				cellManager.getCellUnderPoint(letter.transform.position).clearCell();
				gridCharacters.Remove(letter);
				GameObject.DestroyImmediate(letter.gameObject);
			}
		}
	}

	public void linesCreated(int totalLines)
	{
		//TODO: Estos puntajes que sean configurables en el editor
		if(totalLines > 0)
		{
			audioManager.PlayLeLineCreatedAudio();
		}

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

	protected void getWinCondition()
	{
		//TODO: Mostrar solo la informacion necesaria
		hudManager.setLettersCondition(goalManager.goalLetters);
		hudManager.setObstaclesCondition (goalManager.obstaclesCount);
		hudManager.setWordCondition (goalManager.goalWordsToShow[0]);
		hudManager.setSynCondition (goalManager.goalWordsToShow[0]);
		hudManager.setAntCondition(goalManager.goalWordsToShow[0]);
		hudManager.setPointsCondition(goalManager.goalPoints, pointsCount);
		hudManager.setWordsCondition(goalManager.goalWordsCount);

		//Se muestra el objetivo al inicio del nivel
		hudManager.showGoalAsLetters((goalManager.currentCondition == GoalManager.LETTERS));


		//hudManager.showGoalPopUp(myWinCondition[0],word,quantity,goalLetters);
	}

	protected void actualizePointsWinCondition ()
	{
		if(myWinCondition[0] == "points")
		{
			int quantity = 0;
			quantity = int.Parse (myWinCondition [1]);
			hudManager.setPointsCondition (quantity,pointsCount);
		}
	}

	protected void actualizeWordsCompletedWinCondition()
	{
		if(myWinCondition[0] == "words")
		{
			int quantity = 0;
			quantity = int.Parse (myWinCondition [1]);
			hudManager.setWordsCondition (quantity,wordsMade);
		}
	}


	protected bool isGoalAchieved ()
	{
		switch (myWinCondition[0]) {
		case "points":
			if(pointsCount >= int.Parse( myWinCondition[1]))
			{
				return true;
			}
			break;

		case "words":
			if (wordsMade >= int.Parse (myWinCondition [1])) 
			{
				return true;
			}
			break;
		case "letters":
			if (goalLetters.Count == 0) 
			{
				return true;
			}
			break;
		case "obstacles":
			if (obstaclesCount == obstaclesUsed) 
			{
				return true;
			}
			break;
		case "word":
			if (wordFound) 
			{
				return true;
			}
			break;
		case "ant":
			if (wordFound) 
			{
				return true;
			}
			break;
		case "sin":
			if (wordFound) 
			{
				return true;
			}
			break;
		default:
			break;
		}
		return false;
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		if(!wordManager.checkIfAWordIsPossible(gridCharacters))
		{
			audioManager.PlayLoseAudio();
		}
	}

	protected void checkIfLoose()
	{
		if(!cellManager.checkIfOnePieceCanFit(pieceManager.getShowingPieces()) || remainingMoves == 0)
		{
			if(remainingMoves == 0)
			{
				allowGameInput(false);
			}

			Debug.Log ("No puede poner piezas");
			while(true)
			{
				bool pass = true;
				for(int i=0; i < gridCharacters.Count; i++)
				{
					if(!gridCharacters[i])
					{
						gridCharacters.RemoveAt(i);
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

	private void OnLevelGoalAchieved()
	{
		Debug.Log("Gano de verdad.");
		unlockPowerUp();
		Invoke("winBonification",piecePositionedDelay*2);
	}

	protected void winBonification()
	{
		audioManager.PlayWonAudio();

		allowGameInput (false);

		//Se limpian las letras 
		wordManager.removeAllLetters();

		add1x1Block();
	}

	protected void add1x1Block()
	{
		Cell[] emptyCells = cellManager.getAllEmptyCells();
		Cell cell;

		if (remainingMoves != 0 && emptyCells.Length > 0) 
		{
			cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];

			substractMoves (1);
			GameObject go = GameObject.Instantiate (bonificationPiecePrefab) as GameObject;

			go.GetComponent<Piece> ().currentType = cellManager.colorRandom ();

			cellManager.occupyAndConfigureCell(cell,go,go.GetComponent<Piece> ().currentType,true);

			showScoreTextOnHud (cell.transform.position, 1);
			addPoints(1);

			StartCoroutine (add1x1BlockMore ());
		}
		else 
		{
			if(remainingMoves != 0)
			{
				addPoints (1);
				substractMoves (1);
				StartCoroutine (add1x1BlockMore ());
			}
			else
			{
				if(cellManager.getPredominantColor() != Piece.EType.NONE)
				{
					cellToLetter.AddRange (cellManager.getCellsOfSameType (cellManager.getPredominantColor ()));
				}
				StartCoroutine (addWinLetterAfterBlockMore ());
			}
		}

		actualizeHUDInfo ();
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
			Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL);
			cellManager.occupyAndConfigureCell (cellToLetter [random],letter.gameObject,Piece.EType.LETTER,true);
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
		if(cellManager.getPredominantColor() != Piece.EType.NONE)
		{
			cellToLetter = new List<Cell>(cellManager.getCellsOfSameType(cellManager.getPredominantColor()));
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
		cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.LETTER));
		cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.LETTER_OBSTACLE));
		StartCoroutine (destroyLetter ());
	}

	IEnumerator destroyLetter()
	{
		int random = Random.Range (0, cellToLetter.Count);
		showDestroyedLetterScore(cellToLetter[random]);
		cellToLetter [random].destroyCell ();
		cellToLetter.RemoveAt (random);

		yield return new WaitForSeconds (.2f);
		if (cellToLetter.Count > 0) 
		{			
			StartCoroutine (destroyLetter ());
		}
	}

	protected void showDestroyedLetterScore(Cell cell)
	{
		int amount = consonantPoints;
		if(cell.content.GetComponent<ABCChar>().isVocal())
		{
			amount = vocalPoints;
		}

		showScoreTextOnHud (cell.transform.position, amount);
		addPoints(amount);

		actualizeHUDInfo ();
	}

	protected void allowGameInput(bool allowInput = true)
	{
		inputPiece.allowInput = allowInput;
		inputWords.allowInput = allowInput;
	}

	//TODO: Hay que tener congruencia en los nombres y si ya se usa unlock entonces las variables
	// que sean unlock o viceversa que se use unblock
	protected void unlockPowerUp()
	{
		if(currentLevel.unblockBlock)
		{
			UserDataManager.instance.onePiecePowerUpAvailable = true;
		}
		if(currentLevel.unblockBomb)
		{
			UserDataManager.instance.destroyNeighborsPowerUpAvailable = true;
		}
		if(currentLevel.unblockDestroy)
		{
			UserDataManager.instance.destroyPowerUpAvailable = true;
		}
		if(currentLevel.unblockRotate)
		{
			UserDataManager.instance.rotatePowerUpAvailable = true;
		}
		if(currentLevel.unblockWildcard)
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

		if(tryToUseGems(secondChancePrice))
		{
			secondChanceTimes++;

			remainingMoves += secondChanceMovements;
			actualizeHUDInfo ();

			//inputGameController.activateSecondChanceLocked();
		}
	}

	protected bool tryToUseGems(int gemsPrice = 0)
	{
		//TODO: TransactionManager?
		if(checkIfExistEnoughGems(gemsPrice))
		{
			UserDataManager.instance.playerGems -= gemsPrice;
			hudManager.setGems(UserDataManager.instance.playerGems);
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

	public void tryToActivatePowerup(int powerupTypeIndex)
	{
		//TODO: Chequeo con transaction manager para ver que onda con las gemas
		allowGameInput(false);

		powerupManager.activatePowerUp((PowerupBase.EType) powerupTypeIndex);
	}

	private void OnPowerupCanceled()
	{
		allowGameInput(true);
	}

	private void OnPowerupCompleted()
	{
		//TODO: consumimos gemas
		allowGameInput(true);
	}

	public void activateSettings(bool activate)
	{
		audioManager.PlayButtonAudio();

		hudManager.activateSettings (activate);
	}

	public void closeObjectivePopUp()
	{
		audioManager.PlayButtonAudio();
		hudManager.hideGoalPopUp ();
		allowGameInput ();
	}
		
	public void activateMusic()
	{
		audioManager.PlayButtonAudio();

		if(audioManager.mainAudio)
		{
			audioManager.mainAudio = false;
			UserDataManager.instance.musicSetting = false;
		}
		else
		{
			audioManager.mainAudio = true;
			UserDataManager.instance.musicSetting = true;
		}
	}

	public void activateSounds()
	{
		audioManager.PlayButtonAudio();
		
		if(audioManager.soundEffects)
		{
			audioManager.soundEffects = false;
			UserDataManager.instance.soundEffectsSetting = false;
		}
		else
		{
			audioManager.soundEffects = true;
			UserDataManager.instance.soundEffectsSetting = true;
		}
	}

	public void quitGame()
	{
		audioManager.PlayButtonAudio();

		hudManager.quitGamePopUp ();
	}

	protected void actualizeHUDInfo()
	{
		hudManager.setMovements (remainingMoves);
		hudManager.setPoints (pointsCount);

		actualizeWordsCompletedWinCondition ();
		actualizePointsWinCondition ();
	}

	protected void showScoreTextOnHud(Vector3 pos,int amount)
	{
		hudManager.showScoreTextAt(pos,amount);
	}
}