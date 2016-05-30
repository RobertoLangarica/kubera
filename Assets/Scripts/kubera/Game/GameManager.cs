using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;

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

	public InputPowerUpRotate inputRotate;

	protected int sizeGridX = 8;
	protected int sizeGridY = 8;

	protected bool rotationActive;

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
	private BombAnimation bombAnimation;
	private SecondChancePopUp secondChance;
	private SecondChanceFreeBombs SecondChanceFreeBombs;

	private Level currentLevel;
	private List<Letter> gridCharacters = new List<Letter>();

	void Start()
	{
		wordManager			= FindObjectOfType<WordManager>();
		cellManager			= FindObjectOfType<CellsManager>();
		powerupManager		= FindObjectOfType<PowerUpManager>();
		hudManager			= FindObjectOfType<HUDManager> ();
		pieceManager		= FindObjectOfType<PieceManager>();
		inputPiece			= FindObjectOfType<InputPiece>();
		inputWords			= FindObjectOfType<InputWords>();
		goalManager			= FindObjectOfType<GoalManager>();
		linesAnimation 		= FindObjectOfType<LinesCreatedAnimation> ();
		bombAnimation 		= FindObjectOfType<BombAnimation> ();
		secondChance 		= FindObjectOfType<SecondChancePopUp> ();
		SecondChanceFreeBombs 	= FindObjectOfType<SecondChanceFreeBombs> ();

		secondChance.OnSecondChanceAquired += secondChanceBought;
		secondChance.gameObject.SetActive (false);

		linesAnimation.OnCellFlipped += OnCellFlipped; 

		wordManager.setMaxAllowedLetters(PersistentData.instance.maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;

		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		inputPiece.OnDrop += OnPieceDropped;
		inputPiece.OnSelected += showShadowOnPiece;

		goalManager.OnGoalAchieved += OnLevelGoalAchieved;
		goalManager.OnLetterFound += hudManager.destroyLetterFound;

		hudManager.OnPopUpCompleted += popUpCompleted;
		hudManager.OnPiecesScaled += checkIfLose;

		wordManager.onWordChange += refreshCurrentWordScoreOnHUD;

		powerupManager.getPowerupByType (PowerupBase.EType.ROTATE).OnPowerupCompleted += rotationDeactivated;
		inputRotate.OnRotateArrowsActivated += rotationActivated;

		if(PersistentData.instance)
		{
			configureLevel(PersistentData.instance.currentLevel);
		}

		//TODO: Control de flujo de juego con un init
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.R)) 
		{
			PersistentData.instance.startLevel -= 1;
			SceneManager.LoadScene ("Game");
		}
		if (Input.GetKeyUp (KeyCode.N)) 
		{
			SceneManager.LoadScene ("Game");
		}
		if (Input.GetKeyUp (KeyCode.B) && PersistentData.instance.startLevel > 1) 
		{
			PersistentData.instance.startLevel -= 2;
			SceneManager.LoadScene ("Game");
		}
	}

	protected void rotationActivated(GameObject go)
	{
		rotationActive = true;
		Debug.Log (rotationActive);
	}

	protected void rotationDeactivated()
	{
		rotationActive = false;
		Debug.Log (rotationActive);
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
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
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
				content.squares [0].GetComponent<Collider2D> ().enabled = true;
				cellManager.occupyAndConfigureCell(i,content.gameObject.transform.GetChild (0).gameObject,content.currentType,content.currentColor,true);
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
		allowGameInput (false);

		if(!tryToDropOnGrid(piece))
		{
			inputPiece.returnSelectedToInitialState (0.1f);
			allowGameInput (true);
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
			target.DOScale(target.localScale* 0.8f, 0.1f).SetDelay(piecePositionedDelay).OnComplete(()=>{showShadowOnPiece (piece, false);});
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
			allowGameInput (true);
			checkIfLose ();
		}
		else
		{
			allowGameInput (true);
		}

		Destroy(piece.gameObject);

		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
	}

	private void convertLinesToLetters(List<List<Cell>> cells)
	{
		List<Cell>  cellsToAnimate = new List<Cell> ();
		List<Letter> letters = new List<Letter>();

		for (int i = 0; i < cells.Count; i++) 
		{
			for(int j=0; j<cells[i].Count; j++)
			{
				if (cells [i] [j].contentType != Piece.EType.LETTER) 
				{
					if(cellsToAnimate.IndexOf(cells[i][j]) == -1)
					{
						cellsToAnimate.Add(cells[i][j]);

						cellManager.setCellContentType (cells[i][j], Piece.EType.LETTER);

						letters.Add(wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL));						
					}
				}
			}
		}

		linesAnimation.configurateAnimation(cellsToAnimate,letters);
	}

	protected void OnCellFlipped(Cell cell, Letter letter)
	{
		cellManager.occupyAndConfigureCell (cell,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE);
		gridCharacters.Add(letter);

		checkIfLose ();
		allowGameInput (true);
	}

	public void showShadowOnPiece (GameObject obj, bool showing = true)
	{
		Piece piece = obj.GetComponent<Piece> ();
		showShadowOnPiece (piece, showing);
	}

	private void showShadowOnPiece (Piece piece, bool showing = true)
	{
		pieceManager.showingShadow (piece, showing);
	}

	//TODO: checar el nombre de la funcion
	protected void onUsersAction(int earnedPoints,int movementsUsed = 1)
	{
		addPoints (earnedPoints);
		substractMoves(movementsUsed);
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
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

		wordManager.removeAllLetters(true);

		checkIfLose ();
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

		hudManager.updateTextPoints(0);
		hudManager.showPieces (pieceManager.getShowingPieces ());
		hudManager.updateTextGems(UserDataManager.instance.playerGems);
		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		//Se muestra el objetivo al inicio del nivel
		hudManager.showGoalAsLetters((goalManager.currentCondition == GoalManager.LETTERS));
		hudManager.setWinCondition (goalManager.currentCondition, goalManager.getGoalConditionParameters());

		hudManager.setGoalPopUp(goalManager.currentCondition,goalManager.getGoalConditionParameters());
		activatePopUp ("goalPopUp");
	}

	protected void checkIfLose()
	{
		bool canFit = false;

		//HACK: al inicio del nivel que sirve en los tutoriales
		if (linesAnimation.isOnAnimation || remainingMoves == currentLevel.moves) 
		{
			return;
		}

		if (!rotationActive) 
		{
			canFit = cellManager.checkIfOnePieceCanFit (pieceManager.getShowingPieces ());
		} 
		else 
		{
			List<Piece> tempList = new List<Piece> (pieceManager.getShowingPieces ());
			Piece tempPiece= null;

			for (int i = 0; i < pieceManager.getShowingPieces ().Count; i++) 
			{
				tempPiece = pieceManager.getShowingPieces () [i].toRotateObject.GetComponent<Piece> ();
				for (int j = 0; j < 3; j++) 
				{
					tempList.Add (tempPiece);
					tempPiece = tempPiece.toRotateObject.GetComponent<Piece> ();
				}
			}
			canFit = cellManager.checkIfOnePieceCanFit (tempList);
		}

		if(!canFit || remainingMoves == 0 && !gameOver)
		{
			if(remainingMoves == 0)
			{
				allowGameInput(false);
			}

			Debug.Log ("No puede poner piezas");

			for(int i = gridCharacters.Count-1; i >= 0; i--)
			{
				if(!gridCharacters[i])
				{
					gridCharacters.RemoveAt(i);
				}
			}

			StartCoroutine(checkIfReallyLost());
		}
	}

	IEnumerator checkIfReallyLost()
	{
		yield return new WaitForEndOfFrame();

		if(!gameOver && (remainingMoves <= 0 || !wordManager.checkIfAWordIsPossible(gridCharacters)))
		{
			Debug.Log ("Perdio de verdad");
			AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.LOSE);

			activatePopUp ("SecondChance");
		}
	}

	protected void secondChanceBought()
	{
		remainingMoves += secondChance.movements;
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
		secondChanceFreeBombs ();
	}

	protected void secondChanceFreeBombs()
	{
		bombsUsed += secondChance.bombs;
		SecondChanceFreeBombs.actualizeFreeBombs (bombsUsed);
		SecondChanceFreeBombs.activateFreeBombs (true);
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

		bombAnimation.OnAllAnimationsCompleted += destroyAndCountAllLetters;

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
				updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
				return;
			}
		}
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
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

		cellManager.occupyAndConfigureCell(cell,go.transform.GetChild(0).gameObject,Piece.EType.PIECE,Piece.EColor.YELLOW,true);

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
			StartCoroutine (bombAnimation.startSinglePieceAnimation (cellToLetter [random]));
			cellToLetter.RemoveAt (random);

			yield return new WaitForSeconds (.2f);
			StartCoroutine (addWinLetterAfterActions ());
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
		else 
		{
			//Gano y a se termino win bonification
			PersistentData.instance.fromLevelBuilder = true;
			SceneManager.LoadScene ("Game");
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

		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
	}

	public void allowGameInput(bool allowInput = true)
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
		if(canActivatePowerUp((PowerupBase.EType) powerupTypeIndex))
		{
			powerupManager.activatePowerUp((PowerupBase.EType) powerupTypeIndex);
		}
	}

	protected bool canActivatePowerUp(PowerupBase.EType type)
	{
		//Checa si tiene dinero para usar el poder
		//transaction manager
		return true;
	}

	private void OnPowerupCanceled(PowerupBase.EType type)
	{
		allowGameInput(true);
	}

	private void OnPowerupCompleted(PowerupBase.EType type)
	{
		if(isBombAndSecondChance(type))
		{
			useFreeBomb ();
		}
		else
		{
			//TODO: consumimos gemas

		}
		

		allowGameInput(true);
	}

	protected void useFreeBomb()
	{
		bombsUsed--;

		SecondChanceFreeBombs.actualizeFreeBombs (bombsUsed);

		if(bombsUsed == 0)
		{
			SecondChanceFreeBombs.activateFreeBombs (false);
		}
	}

	protected bool isBombAndSecondChance(PowerupBase.EType type)
	{
		if(type == PowerupBase.EType.BOMB && bombsUsed > 0)
		{
			return true;
		}
		return false;
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
		
				Invoke ("allowInputFromInvoke", 0.5f);
			break;
		}
	}

	protected void allowInputFromInvoke()
	{
		allowGameInput();
	}

	public void activatePopUp(string popUpName)
	{
		allowGameInput (false);
		hudManager.activatePopUp (popUpName);
	}

	public void quitGame()
	{
		PersistentData.instance.startLevel -= 1;
		SceneManager.LoadScene ("Game");
		/*AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);
		activatePopUp ("exitGame");*/
	}

	//DONE:Le cambie el nombre de actualizeHudInfo a updateHudGameInfo 
	// esta funcion no actualiza info de la HUD es mas bien el status del jeugo (hay que ver un mejor nombre)
	protected void updateHudGameInfo (int remainingMoves,int pointsCount,string goalCondition)
	{
		hudManager.updateTextMovements (remainingMoves);
		hudManager.updateTextPoints (pointsCount);

		updateHudWinCondition (goalCondition);
	}

	//DONE: Esto no actualiza la condicion de victoria, actualiza la hud y el status en el que va la condicion de victoria
	//DONE: updateHudWinCondition 
	protected void updateHudWinCondition(string goalCondition)
	{
		switch (goalCondition) {
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