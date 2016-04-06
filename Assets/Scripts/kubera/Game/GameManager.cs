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

	protected bool gameOver = false;

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

	protected int sizeGridX = 8;
	protected int sizeGridY = 8;

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
		wordManager		= FindObjectOfType<WordManager>();
		cellManager		= FindObjectOfType<CellsManager>();
		powerupManager	= FindObjectOfType<PowerUpManager>();
		hudManager		= FindObjectOfType<HUDManager> ();
		audioManager	= FindObjectOfType<AudioManager>();
		pieceManager	= FindObjectOfType<PieceManager>();
		inputPiece		= FindObjectOfType<InputPiece>();
		inputWords		= FindObjectOfType<InputWords>();
		goalManager		= FindObjectOfType<GoalManager>();

		wordManager.setMaxAllowedLetters(PersistentData.instance.maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;

		//LetterSize
		Vector3 cellSizeReference = cellManager.cellPrefab.GetComponent<SpriteRenderer>().bounds.size;
		Vector3 lettersizeDelta = (Camera.main.WorldToScreenPoint(cellSizeReference) -Camera.main.WorldToScreenPoint(Vector3.zero)) * gridLettersSizeMultiplier;
		lettersizeDelta.x = Mathf.Abs(lettersizeDelta.x);
		lettersizeDelta.y = Mathf.Abs(lettersizeDelta.y);
		wordManager.gridLettersSizeDelta = new Vector2(lettersizeDelta.x, lettersizeDelta.y);

		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		inputPiece.OnDrop += OnPieceDropped;

		goalManager.OnGoalAchieved += OnLevelGoalAchieved;
		goalManager.OnLetterFound += hudManager.destroyLetterFound;

		hudManager.OnPopUpCompleted += popUpCompleted;

		wordManager.onWordChange += actualizeWordPoints;

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

		//TODO: Control de flujo de juego con un init
	}

	private void configureLevel(Level level)
	{
		currentLevel = level;

		initLettersFromLevel(level);
		initPiecesFromLevel(level);
		initGoalsFromLevel(level);


		remainingMoves = totalMoves = currentLevel.moves;
	
		cellManager.resizeGrid(sizeGridX,sizeGridY);
		populateGridFromLevel(level);

		//TODO: Si no es parte de la configuracion del nivel no debe ir aqui
		cellToLetter = new List<Cell> ();//Esta inicializacion va aqui?


		allowGameInput(false);//TODO: el input no es configuracion de nivel

		//Las cosas de la hud que no se icializen con info del nivel hay que quitarlas
		//Si hay que mandar a la hud a un estado default antes de iniciar el juego hay que hacerlo en alguna llamada explicita

		initHudValues();
		actualizeHUDInfo();
		actualizeWordPoints ();
	}

	protected void initLettersFromLevel(Level level)
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

	private void initPiecesFromLevel(Level level)
	{
		pieceManager.initializePiecesFromCSV(level.pieces);
		pieceManager.initializePiecesToShow ();	
	}

	private void initGoalsFromLevel(Level level)
	{
		goalManager.initializeFromString(currentLevel.goal);	
	}

	protected void populateGridFromLevel(Level level)
	{
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
				cellManager.occupyAndConfigureCell(i,content.gameObject,content.currentType,content.currentColor,true);
			}
			else if((cellType & 0x8) == 0x8)
			{	
				//Obstaculo
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.OBSTACLE);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,Piece.EType.LETTER_OBSTACLE,Piece.EColor.NONE,true);
				//obstaclesCount++;

				gridCharacters.Add(letter);
			}
			else if((cellType & 0x20) == 0x20)
			{	
				//De tutorial
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.TUTORIAL);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE,true);
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
			cellManager.occupyAndConfigureCell (cells [i], piece.squares [i], piece.currentType,piece.currentColor);

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
			onUsersAction(piece.squares.Length);
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

					cellManager.occupyAndConfigureCell (cells [i] [j], letter.gameObject, Piece.EType.LETTER,Piece.EColor.NONE);
					letter.gameObject.transform.DOMove (cellPosition, 0);
					gridCharacters.Add(letter);
				}
			}
		}
	}

	//TODO: checar el nombre de la funcion
	protected void onUsersAction(int earnedPoints,int movementsUsed = 1)
	{
		addPoints (earnedPoints);
		substractMoves(movementsUsed);
		actualizeHUDInfo ();

		checkIfLoose ();
	}

	protected void addPoints(int amount)
	{
		pointsCount += amount;
		goalManager.submitPoints (amount);
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
		goalManager.submitWord(wordManager.letters);

		//Los puntos se leen antes de limpiar porque sin letras no hay puntos
		onUsersAction (wordManager.wordPoints);
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

	protected void initHudValues()
	{
		hudManager.actualizePoints(0);
		hudManager.showPieces (pieceManager.getShowingPieces ());
		hudManager.actualizeGems(UserDataManager.instance.playerGems);
		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		//TODO: Hay que quitar este arreglo, todo en la hud y en todos lados se usan 3 estrellas (y este arreglo consume memoria)
		float[] scoreToStar = new float[3];
		scoreToStar [0] = currentLevel.scoreToStar1;
		scoreToStar [1] = currentLevel.scoreToStar2;
		scoreToStar [2] = currentLevel.scoreToStar3;
		hudManager.setStarsData (scoreToStar);

		//Se muestra el objetivo al inicio del nivel
		hudManager.showGoalAsLetters((goalManager.currentCondition == GoalManager.LETTERS));
		hudManager.setWinCondition (goalManager.currentCondition, goalManager.getGoalConditionParameters());

		hudManager.setGoalPopUp(goalManager.currentCondition,goalManager.getGoalConditionParameters());
		activatePopUp ("goalPopUp");
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		if(!wordManager.checkIfAWordIsPossible(gridCharacters) || remainingMoves <= 0)
		{
			Debug.Log ("Perdio de verdad");
			audioManager.PlayLoseAudio();
		}
	}

	protected void checkIfLoose()
	{
		if(!cellManager.checkIfOnePieceCanFit(pieceManager.getShowingPieces()) || remainingMoves == 0 && !gameOver)
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
		if (!gameOver) 
		{
			Debug.Log ("Gano de verdad.");
			gameOver = true;
			unlockPowerUp ();
			Invoke ("winBonification", piecePositionedDelay * 2);
		}
	}

	protected void winBonification()
	{
		audioManager.PlayWonAudio();

		allowGameInput (false);

		//Se limpian las letras 
		wordManager.removeAllLetters();

		winBonificationActions();
	}

	protected void winBonificationActions()
	{
		if(cellManager.getAllEmptyCells().Length > 0 &&remainingMoves > 0 )
		{
			add1x1Block ();
		}
		else
		{
			if (remainingMoves > 0) 
			{
				addMovementPoint ();
			}
			else
			{
				if(cellManager.existType(Piece.EType.PIECE))
				{
					cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.PIECE));
				}
				StartCoroutine (addWinLetterAfterActions ());
				actualizeHUDInfo ();
				return;
			}
		}
		actualizeHUDInfo ();
		StartCoroutine (continueWinBonificationActions ());
	}

	protected void addMovementPoint()
	{
		addPoints (1);
		substractMoves (1);
	}

	protected void add1x1Block()
	{
		Cell[] emptyCells = cellManager.getAllEmptyCells();
		Cell cell;

		cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];


		GameObject go = GameObject.Instantiate (bonificationPiecePrefab) as GameObject;

		go.GetComponent<Piece> ().currentColor = cellManager.colorRandom ();

		cellManager.occupyAndConfigureCell(cell,go,Piece.EType.PIECE,Piece.EColor.AQUA,true);

		showScoreTextOnHud (cell.transform.position, 1);
		substractMoves (1);
		addPoints(1);

	}
	IEnumerator continueWinBonificationActions()
	{
		yield return new WaitForSeconds (.2f);
		winBonificationActions ();
	}

	IEnumerator addWinLetterAfterActions()
	{
		int random = Random.Range (0, cellToLetter.Count);

		if (cellToLetter.Count > 0) 
		{
			Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL);
			cellManager.occupyAndConfigureCell (cellToLetter [random],letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE,true);
			cellToLetter.RemoveAt (random);

			yield return new WaitForSeconds (.2f);
			StartCoroutine (addWinLetterAfterActions ());
		}
		else
		{
			useBombs();
		}
	}

	protected void useBombs()
	{
		if(cellManager.existType(Piece.EType.PIECE))
		{
			cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.PIECE));
			StartCoroutine (addWinLetterAfterActions ());
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
		if(cell.content.GetComponent<Letter> ().abcChar.isVocal())
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
			hudManager.actualizeGems(UserDataManager.instance.playerGems);

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
		UserDataManager.instance.giveLifeToPlayer (-1);
	}

	public void Retry()
	{
		
	}

	public void RefillLifes()
	{
		UserDataManager.instance.giveLifeToPlayer (UserDataManager.instance.maximumLifes);
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
		//hudManager.hideGoalPopUp ();
		allowGameInput ();
	}

	public void popUpCompleted (string action ="")
	{
		switch (action) {
		case "endGame":
			break;
		default:
			allowGameInput (true);	
			break;
		}
	}

	public void activatePopUp(string popUpName)
	{
		allowGameInput (false);
		hudManager.activatePopUp (popUpName);
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
		activatePopUp ("exitGame");
	}

	//TODO: esta funcion no actualiza info de la HUD es mas bien el status del jeugo (hay que ver un mejor nombre)
	//TODO: Esta funcion deberia ser mas granular, al llamarla ya mandenle los puntos y la condicion de victoria
	protected void actualizeHUDInfo()
	{
		hudManager.actualizeMovements (remainingMoves);
		hudManager.actualizePoints (pointsCount);

		actualizeWinCondition ();
	}

	//TODO: Esto no actualiza la condicion de victoria, actualiza la hud y el status en el que va la condicion de victoria
	//TODO: Esta funcion deberia ser mas granular, al llamarla ya mandenle los puntos y la condicion de victoria
	protected void actualizeWinCondition()
	{
		switch (goalManager.currentCondition) {
		case GoalManager.POINTS:
			hudManager.actualizePointsOnWinCondition (goalManager.pointsCount.ToString(),goalManager.goalPoints.ToString());
			break;
		case GoalManager.WORDS_COUNT:
			hudManager.actualizeWordsMadeOnWinCondition (goalManager.wordsCount.ToString(),goalManager.goalWords.ToString());
			break;
		}
	}

	//TODO: Esto no actualiza los puntos de una palabra, actualiza los puntos que muestra la hud por una palabra
	//TODO: Hagan mas granular a esta funcion y que reciba los puntos y no los lea
	protected void actualizeWordPoints()
	{
		hudManager.setLettersPoints (wordManager.wordPoints);
	}

	//TODO: showFloatingPointsAt
	protected void showScoreTextOnHud(Vector3 pos,int amount)
	{
		hudManager.showScoreTextAt(pos,amount);
	}
}