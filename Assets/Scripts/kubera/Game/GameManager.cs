using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	public Text scoreText;

	public GameObject bonificationPiecePrefab;

	protected bool gameOver = false;

	protected int pointsCount = 0;

	protected int totalMoves;
	protected int remainingMoves;

	protected int bombsUsed = 0;
	public float piecePositionedDelay = 0.1f;

	public int consonantPoints= 3;
	public int vocalPoints = 1; 

	public Transform gridLettersContainer;
	public float gridLettersSizeMultiplier = 0.9f;

	public List<int> linesCreatedPoints = new List<int> ();

	protected int sizeGridX = 8;
	protected int sizeGridY = 8;

	protected List<Cell> cellToLetter;

	protected int currentWildCardsActivated;

	private WordManager		wordManager;
	private CellsManager	cellManager;
	private PowerUpManager	powerupManager;
	private PieceManager 	pieceManager;
	private HUDManager 	 	hudManager;
	private InputPiece 		inputPiece;
	private InputWords 		inputWords;
	private GoalManager		goalManager;

	private LinesCreatedAnimation linesAnimation;

	private SecondChancePopUp secondChance;

	private Level currentLevel;
	private List<Letter> gridCharacters = new List<Letter>();


	void Start()
	{
		wordManager		= FindObjectOfType<WordManager>();
		cellManager		= FindObjectOfType<CellsManager>();
		powerupManager	= FindObjectOfType<PowerUpManager>();
		hudManager		= FindObjectOfType<HUDManager> ();
		pieceManager	= FindObjectOfType<PieceManager>();
		inputPiece		= FindObjectOfType<InputPiece>();
		inputWords		= FindObjectOfType<InputWords>();
		goalManager		= FindObjectOfType<GoalManager>();
		linesAnimation 	= FindObjectOfType<LinesCreatedAnimation> ();
		secondChance 	= FindObjectOfType<SecondChancePopUp> ();

		secondChance.OnSecondChanceAquired += secondChanceBought;
		secondChance.gameObject.SetActive (false);

		linesAnimation.OnAnimationFinish += OnLinesAnimationEnded;

		wordManager.setMaxAllowedLetters(PersistentData.instance.maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;

		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		inputPiece.OnDrop += OnPieceDropped;
		inputPiece.OnSelected += setShadow;

		goalManager.OnGoalAchieved += OnLevelGoalAchieved;
		goalManager.OnLetterFound += hudManager.destroyLetterFound;

		hudManager.OnPopUpCompleted += popUpCompleted;
		hudManager.OnPiecesScaled += checkIfLoose;

		wordManager.onWordChange += refreshCurrentWordScoreOnHUD;

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

		initGoalsFromLevel (level);

		remainingMoves = totalMoves = currentLevel.moves;
	
		cellManager.resizeGrid(sizeGridX,sizeGridY);

		//LetterSize       
		//Depende del tamaño de las celdas que se aclcula en el resizeGrid
		Vector3 cellSizeReference = new Vector3(cellManager.cellSize,cellManager.cellSize,1);
		Vector3 lettersizeDelta = (Camera.main.WorldToScreenPoint(cellSizeReference) -Camera.main.WorldToScreenPoint(Vector3.zero)) * gridLettersSizeMultiplier;
		lettersizeDelta.x = Mathf.Abs(lettersizeDelta.x);
		lettersizeDelta.y = Mathf.Abs(lettersizeDelta.y);
		wordManager.gridLettersSizeDelta = new Vector2(lettersizeDelta.x , lettersizeDelta.y);

		populateGridFromLevel(level);

		initHudValues();
		actualizeHUDInfo();
		refreshCurrentWordScoreOnHUD (wordManager.wordPoints);
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
		List<Cell> cellsUnderPiece = cellManager.getFreeCellsUnderPiece(piece);

		if (cellsUnderPiece.Count == piece.squares.Length) 
		{
			putPiecesOnGrid (piece, cellsUnderPiece);
			AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.PIECE_POSITIONATED);

			//Tomamos en cuenta los tiempos de todos los twens de posicionamiento
			StartCoroutine(afterPiecePositioned(piece));

			return true;
		}

		return false;
	}

	private void putPiecesOnGrid(Piece piece, List<Cell> cellsUnderPiece)
	{
		Vector3 piecePosition;

		for(int i=0; i< cellsUnderPiece.Count; i++)
		{ 
			cellManager.occupyAndConfigureCell (cellsUnderPiece [i], piece.squares [i], piece.currentType,piece.currentColor);

			//Cada cuadro reeparentado para dejar de usar su contenedor actual
			//y manipularlo individualmente
			piece.squares[i].transform.SetParent(piece.transform.parent);

			piecePosition =  cellsUnderPiece[i].transform.position + (new Vector3 (cellsUnderPiece[i].GetComponent<SpriteRenderer> ().bounds.extents.x,
				-cellsUnderPiece[i].GetComponent<SpriteRenderer> ().bounds.extents.y, 0));

			piece.squares [i].GetComponent<Collider2D> ().enabled = true;

			Transform target = piece.squares[i].transform;

			target.DOMove (piecePosition, piecePositionedDelay);
			target.DOScale(target.localScale* 0.8f, 0.1f).SetDelay(piecePositionedDelay).OnComplete(()=>{setShadow (piece, false);});
			target.DOScale(target.localScale, 0.1f).SetDelay(piecePositionedDelay+0.1f);
		}
	}

	IEnumerator afterPiecePositioned(Piece piece)
	{
		yield return new WaitForSeconds (piecePositionedDelay+0.25f);

		bool piecesWhereCreated = false;

		if(pieceManager.isAShowedPiece(piece))
		{
			pieceManager.removeFromShowedPieces (piece);

			if (pieceManager.getShowingPieces ().Count == 0) 
			{
				piecesWhereCreated = true;
				pieceManager.initializePiecesToShow ();
				hudManager.showPieces (pieceManager.getShowingPieces ());
			}

			//Damos puntos por cada cuadro en la pieza
			onUsersAction(piece.squares.Length);
			showFloatingPointsAt (piece.transform.position, piece.squares.Length);
		}


		List<List<Cell>> cells = cellManager.getCompletedVerticalAndHorizontalLines ();

		if (cells.Count != 0) 
		{
			//Puntos por las lineas creadas
			linesCreated (cells.Count);
			convertLinesToLetters (cells);
		} 
		else if(!piecesWhereCreated)
		{
			checkIfLoose ();
		}

		Destroy(piece.gameObject);

		actualizeHUDInfo ();
	}

	private void convertLinesToLetters(List<List<Cell>> cells)
	{
		List<Cell> cellsToAnimate = new List<Cell> ();

		for (int i = 0; i < cells.Count; i++) 
		{
			for(int j=0; j<cells[i].Count; j++)
			{
				if (cells [i] [j].contentType != Piece.EType.LETTER) 
				{
					cellsToAnimate.Add(cells[i][j]);
				}
			}
		}

		linesAnimation.configurateAnimation(cellsToAnimate,cellManager,wordManager);
	}

	protected void OnLinesAnimationEnded(Cell cell,Letter letter, Piece.EType type, Piece.EColor color)
	{
		cellManager.occupyAndConfigureCell (cell,letter.gameObject,type,color);
		gridCharacters.Add(letter);

		checkIfLoose ();
	}

	//TODO: checar nombre
	private void setShadow (GameObject obj, bool showing = true)
	{
		Piece piece = obj.GetComponent<Piece> ();
		setShadow (piece, showing);
	}

	private void setShadow (Piece piece, bool showing = true)
	{
		pieceManager.showingShadow (piece, showing);
	}

	//TODO: checar el nombre de la funcion
	protected void onUsersAction(int earnedPoints,int movementsUsed = 1)
	{
		addPoints (earnedPoints);
		substractMoves(movementsUsed);
		actualizeHUDInfo ();
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

		checkIfLoose ();
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
		if(totalLines > 0)
		{
			AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.LINE_CREATED);
		}

		addPoints(linesCreatedPoints[totalLines]);
	}

	protected void initHudValues()
	{
		//TODO: Hay que quitar este arreglo, todo en la hud y en todos lados se usan 3 estrellas (y este arreglo consume memoria)
		float[] scoreToStar = new float[3];
		scoreToStar [0] = currentLevel.scoreToStar1;
		scoreToStar [1] = currentLevel.scoreToStar2;
		scoreToStar [2] = currentLevel.scoreToStar3;
		hudManager.setStarsData (scoreToStar);

		hudManager.actualizePoints(0);
		hudManager.showPieces (pieceManager.getShowingPieces ());
		hudManager.actualizeGems(UserDataManager.instance.playerGems);
		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		//Se muestra el objetivo al inicio del nivel
		hudManager.showGoalAsLetters((goalManager.currentCondition == GoalManager.LETTERS));
		hudManager.setWinCondition (goalManager.currentCondition, goalManager.getGoalConditionParameters());

		hudManager.setGoalPopUp(goalManager.currentCondition,goalManager.getGoalConditionParameters());
		activatePopUp ("goalPopUp");
	}

	protected void checkIfLoose()
	{
		if (linesAnimation.isOnAnimation) 
		{
			return;
		}

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

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		if(!wordManager.checkIfAWordIsPossible(gridCharacters) || remainingMoves <= 0)
		{
			Debug.Log ("Perdio de verdad");
			AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.LOSE);

			activatePopUp ("SecondChance");
		}
	}

	protected void secondChanceBought()
	{
		remainingMoves += secondChance.secondChanceMovements;
		actualizeHUDInfo ();
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
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.WON);

		allowGameInput (false);

		cellToLetter = new List<Cell> ();

		//Se limpian las letras 
		wordManager.removeAllLetters();

		expendMovement();
	}

	protected void expendMovement()
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
		StartCoroutine (continueExpendingMovements ());
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

		showFloatingPointsAt (cell.transform.position, 1);
		substractMoves (1);
		addPoints(1);

	}

	IEnumerator continueExpendingMovements()
	{
		yield return new WaitForSeconds (.2f);
		expendMovement ();
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

		showFloatingPointsAt (cell.transform.position, amount);
		addPoints(amount);

		actualizeHUDInfo ();
	}

	protected void allowGameInput(bool allowInput = true)
	{
		inputPiece.allowInput = allowInput;
		inputWords.allowInput = allowInput;
	}
		
	protected void unlockPowerUp()
	{
		if(currentLevel.unblockBlock)
		{
			UserDataManager.instance.isOnePiecePowerUpUnlocked = true;
		}
		if(currentLevel.unblockBomb)
		{
			UserDataManager.instance.isDestroyNeighborsPowerUpUnlocked = true;
		}
		if(currentLevel.unblockDestroy)
		{
			UserDataManager.instance.isDestroyPowerUpUnlocked = true;
		}
		if(currentLevel.unblockRotate)
		{
			UserDataManager.instance.isRotatePowerUpUnlocked = true;
		}
		if(currentLevel.unblockWildcard)
		{
			UserDataManager.instance.isWildCardPowerUpUnlocked = true;
		}
	}

	public void tryToActivatePowerup(int powerupTypeIndex)
	{
		//TODO: Chequeo con transaction manager para ver que onda con las gemas
		//TODO: Checar lo del precio de los powerUps
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
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);

		hudManager.activateSettings (activate);
	}

	public void closeObjectivePopUp()
	{
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);
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

	public void quitGame()
	{
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);
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
			hudManager.actualizeWordsMadeOnWinCondition (goalManager.wordsCount.ToString(),goalManager.goalWordsCount.ToString());
			break;
		}
	}

	protected void refreshCurrentWordScoreOnHUD(int wordScore)
	{
		hudManager.setLettersPoints (wordScore);
	}

	protected void showFloatingPointsAt(Vector3 pos,int amount)
	{
		hudManager.showScoreTextAt(pos,amount);
	}
}