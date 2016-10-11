using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Kubera.Data;
using utils.gems;

public class GameManager : MonoBehaviour 
{
	public delegate void DGameManagerNotification();

	public DGameManagerNotification OnPiecePositionated;
	public DGameManagerNotification OnPointsEarned;
	public DGameManagerNotification OnMovementRemoved;

	public Text scoreText;

	public GameObject bonificationPiecePrefab;

	protected bool powerUpUsed = false;

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
	public float checkIfLoseSeconds = 5;

	protected int sizeGridX = 8;
	protected int sizeGridY = 8;

	protected bool rotationActive;

	protected List<Cell> cellToLetter;

	protected int currentWildCardsActivated;

	//TODO hacerlos publicos y ponerlos en el editor
	public WordManager		wordManager;
	public CellsManager		cellManager;
	public PowerUpManager	powerupManager;
	public PieceManager 	pieceManager;
	public HUDManager 	 	hudManager;
	public InputPiece 		inputPiece;
	public InputWords 		inputWords;
	public GoalManager		goalManager;
	public SettingsButton settingsButton;
	public BombAnimation bombAnimation;
	public SecondChancePopUp secondChance;
	public SecondChanceFreeBombs SecondChanceFreeBombs;
	public LinesCreatedAnimation linesAnimation;
	public startGamePopUp startGameReference;
	public FloatingTextBase gemsExpendedFeedBack;

	private Level currentLevel;
	private List<Letter> gridCharacters = new List<Letter>();

	protected bool cancelBonify;

	//Para los analiticos
	protected Dictionary<string,int> powerUpsUsedCount = new Dictionary<string, int>();
	protected int expendedGems = 0;

	void Start()
	{
		isAnObjectMissing ();

		secondChance.OnSecondChanceAquired += secondChanceBought;
		secondChance.gameObject.SetActive (false);

		linesAnimation.OnCellFlipped += OnCellFlipped; 
		linesAnimation.OnAllCellsFlipped += OnAllCellsFlipped;

		wordManager.setMaxAllowedLetters(PersistentData.GetInstance().maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;

		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;
		powerupManager.OnPowerupCompletedNoGems = OnPowerupCompletedNoGems;

		inputPiece.OnDrop += OnPieceDropped;
		inputPiece.OnSelected += showShadowOnPiece;

		goalManager.OnGoalAchieved += OnLevelGoalAchieved;
		goalManager.OnLetterFound += hudManager.destroyLetterFound;
		goalManager.OnBlackLetter += hudManager.destroyLetterFound;

		hudManager.OnPopUpCompleted += popUpCompleted;
		hudManager.OnPiecesScaled += checkIfLose;

		if(GemsManager.GetCastedInstance<GemsManager>())
		{
			//actualizamos gemas
			GemsManager.GetCastedInstance<GemsManager>().OnGemsUpdated += hudManager.updateTextGems;
		}

		wordManager.onWordChange += refreshCurrentWordScoreOnHUD;
		settingsButton.OnActivateMusic += activateMusic;

		powerupManager.getPowerupByType (PowerupBase.EType.ROTATE).OnPowerupUsed += rotationDeactivated;

		inputRotate.OnRotateArrowsActivated += rotationActivated;
	
		if (PersistentData.GetInstance().abcDictionary.getAlfabet () == null) 
		{
			PersistentData.GetInstance ().onDictionaryFinished += startGame;
		} 
		else 
		{
			startGame ();
		}

		//TODO: hardcoding
		bonificationPiecePrefab.SetActive (true);
		//TODO: Control de flujo de juego con un init

		for (int i = 0; i < powerupManager.powerups.Count; i++) 
		{
			powerUpsUsedCount.Add (powerupManager.powerups [i].type.ToString (), 0);
		}
	}

	void OnDestroy()
	{
		if(GemsManager.GetCastedInstance<GemsManager>())
		{
			GemsManager.GetCastedInstance<GemsManager>().OnGemsUpdated -= hudManager.updateTextGems;
		}
	}

	protected void startGame()
	{
		if(!PersistentData.GetInstance().fromLevelsToGame && !PersistentData.GetInstance().fromLevelBuilder)
		{
			configureLevel(PersistentData.GetInstance().getLevelByIndex(37));
		}
		else
		{
			configureLevel(PersistentData.GetInstance().currentLevel);
		}

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance ().Stop ("menuMusic",false);
			AudioManager.GetInstance ().Play ("gamePlay");
		}

		hudManager.enablePowerUps ();
		unBlockPowerUpForThisLevel ();

		hudManager.setWorldBackground (currentLevel.world-1);
		populateGridFromLevel(currentLevel);
		cellManager.createFrame ();

		refreshCurrentWordScoreOnHUD (wordManager.wordPoints);

		StartCoroutine (finishLoadingFix ());
	}

	IEnumerator finishLoadingFix()
	{
		if(PersistentData.GetInstance().fromLevelsToGame)
		{
			yield return new WaitForEndOfFrame ();
			//ScreenManager.GetInstance().preLoadingScene ("Levels");
			yield return new WaitForEndOfFrame ();
			//yield return new WaitUntil (()=> ScreenManager.instance.preloadSceneAsync.isDone);
		}
		yield return new WaitForEndOfFrame ();
		if(ScreenManager.GetInstance() && !PersistentData.GetInstance().fromLevelBuilder)
		{
			ScreenManager.GetInstance().sceneFinishLoading ();
		}
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.R)) 
		{
			PersistentData.GetInstance().startLevel -= 1;
			ScreenManager.GetInstance().GoToScene ("Game");
		}
		if (Input.GetKeyUp (KeyCode.N)) 
		{
			ScreenManager.GetInstance().GoToScene ("Game");
		}
		if (Input.GetKeyUp (KeyCode.B) && PersistentData.GetInstance().startLevel > 1) 
		{
			PersistentData.GetInstance().startLevel -= 2;
			ScreenManager.GetInstance().GoToScene ("Game");
		}
		if (Input.GetKeyUp (KeyCode.Z)) 
		{
			onUsersAction (0, 5);
			//activatePopUp ("noOptionsPopUp");
			//onUsersAction (0);
		}

		if (Input.GetKeyUp (KeyCode.X)) 
		{
			onUsersAction (10, 0);
			//activatePopUp ("noOptionsPopUp");
			//onUsersAction (0);
		}

		if (Input.GetKeyUp (KeyCode.Y)) 
		{
			cancelBonify = true;
			goalManager.OnGoalAchieved ();
		}

		if (Input.GetKeyUp (KeyCode.Q)) 
		{
			Debug.Break ();
		}
	}

	protected void rotationActivated(GameObject go)
	{
		rotationActive = true;
	}

	protected void rotationDeactivated()
	{
		rotationActive = false;
	}

	private void configureLevel(Level level)
	{
		currentLevel = level;

		initLettersFromLevel(level);
		initPiecesFromLevel(level);

		initGoalsFromLevel (level);

		remainingMoves = totalMoves = currentLevel.moves;
	
		cellManager.setColorIndex (currentLevel.world-1);
		cellManager.resizeGrid(sizeGridX,sizeGridY);

		//LetterSize       
		//Depende del tamaño de las celdas que se aclcula en el resizeGrid
		Vector3 cellSizeReference = new Vector3(cellManager.cellSize,cellManager.cellSize,1);
		Vector3 lettersizeDelta = (Camera.main.WorldToScreenPoint(cellSizeReference) -Camera.main.WorldToScreenPoint(Vector3.zero)) * gridLettersSizeMultiplier;
		lettersizeDelta.x = Mathf.Abs(lettersizeDelta.x);
		lettersizeDelta.y = Mathf.Abs(lettersizeDelta.y);
		wordManager.gridLettersSizeDelta = new Vector2(lettersizeDelta.x , lettersizeDelta.y);

		initHudValues();
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
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
				content.squares [0].GetComponent<Collider2D> ().enabled = false;
				cellManager.occupyAndConfigureCell(i,content.gameObject.transform.GetChild (0).gameObject,content.currentType,content.currentColor,true);
				content.gameObject.transform.SetParent (hudManager.showingPiecesContainer);
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
		//TODO checar que el flujo funcione correctamente
		//allowGameInput (false);

		if(!dropOnGrid(piece))
		{
			inputPiece.returnSelectedToInitialState (0.1f);
			allowGameInput (true);
		}

		inputPiece.reset();
	}

	public bool canDropOnGrid(Piece piece)
	{
		List<Cell> cellsUnderPiece = cellManager.getFreeCellsUnderPiece(piece);

		if (cellsUnderPiece.Count == piece.squares.Length) 
		{
			return true;
		}
		return false;
	}

	public bool dropOnGrid(Piece piece)
	{
		if (canDropOnGrid(piece)) 
		{
			List<Cell> cellsUnderPiece = cellManager.getFreeCellsUnderPiece(piece);

			putPiecesOnGrid (piece, cellsUnderPiece);
			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("piecePositionated");
				AudioManager.GetInstance().Play("piecePositionated");
			}

			//Tomamos en cuenta los tiempos de todos los twens de posicionamiento
			StartCoroutine(afterPiecePositioned(piece));

			return true;
		}

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("badPositionated");
			AudioManager.GetInstance().Play("badPositionated");
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
		int pointsMade = 0;

		if(pieceManager.isAShowedPiece(piece))
		{
			pieceManager.removeFromShowedPieces (piece);

			if (pieceManager.getShowingPieces ().Count == 0) 
			{
				piecesWhereCreated = true;
				pieceManager.initializePiecesToShow ();
				hudManager.showPieces (pieceManager.getShowingPieces ());

				if(AudioManager.GetInstance())
				{
					AudioManager.GetInstance().Stop("pieceCreated");
					AudioManager.GetInstance().Play("pieceCreated");
				}
			}

			//Damos puntos por cada cuadro en la pieza
			onUsersAction(piece.squares.Length);

			pointsMade += piece.squares.Length;
		}


		List<List<Cell>> cells = cellManager.getCompletedVerticalAndHorizontalLines ();

		if (cells.Count != 0) 
		{
			//Puntos por las lineas creadas
			linesCreated (cells.Count);

			if(cells.Count > linesCreatedPoints.Count)
			{
				pointsMade += linesCreatedPoints[linesCreatedPoints.Count-1];
			}
			else
			{
				pointsMade += linesCreatedPoints[cells.Count];
			}

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

		showFloatingPointsAt (piece.transform.position, pointsMade);

		Destroy(piece.gameObject);

		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);

		if (OnPiecePositionated != null) 
		{
			OnPiecePositionated ();
		}
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
						cellManager.setCellContentColor (cells [i] [j], Piece.EColor.NONE);

						letters.Add(wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL));						
					}
				}
			}
		}

		linesAnimation.configurateAnimation(cellsToAnimate,letters);
	}

	public void OnCellFlipped(Cell cell, Letter letter)
	{
		cellManager.occupyAndConfigureCell (cell,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE);
		gridCharacters.Add(letter);

		/*checkIfLose ();
		allowGameInput (true);*/
	}

	public void OnAllCellsFlipped()
	{
		allowGameInput (true);
		Invoke("checkIfLose",0);
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

		if (OnMovementRemoved != null) 
		{
			OnMovementRemoved ();
		}
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
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("destroyWord");
			AudioManager.GetInstance().Play("destroyWord");
		}

		wordManager.removeAllLetters(false);
	}

	public void OnRetrieveWord()
	{
		if (!PersistentData.GetInstance ().fromLevelBuilder) 
		{
			KuberaAnalytics.GetInstance ().registerCreatedWord (currentLevel.name, wordManager.getCurrentWordOnList (), wordManager.letters.Count);
		}

		//Contamos obstaculos y si la meta es usar letras entonces vemos si se usan
		goalManager.submitWord(wordManager.letters);

		showFloatingPointsAt (wordManager.letterContainer.transform.position, wordManager.wordPoints);

		//Los puntos se leen antes de limpiar porque sin letras no hay puntos
		onUsersAction (wordManager.wordPoints);
		removeLettersFromGrid(wordManager.letters, true);

		//wordManager.removeAllLetters(true);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("retrieveWord");
			AudioManager.GetInstance().Play("retrieveWord");
		}

		checkIfLose ();
		useHintWord (false);
	}

	/**
	 * @param letters : Listado de letras
	 * @param useReferenceInstead : Elimina las letras que se tengan de referencia y no las de la lista 
	 * */
	private void removeLettersFromGrid(List<Letter> letters, bool useReferenceInstead)
	{
		Letter letter;

		float fulltime = 1.5f;
		float wait = 0.25f;

		if(useReferenceInstead)
		{
			wordManager.activateWordBtn (false, false);
			wordManager.activatePointsGO (false);
			StartCoroutine(wordManager.afterAllLettersRemoved(fulltime));
		}

		hudManager.showVacum (fulltime);

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

				StartCoroutine(wordManager.animateWordRetrieved (letter.letterReference,wait,fulltime));
				GameObject.DestroyImmediate(letter.gameObject);
			}
		}

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("vacuum");
			AudioManager.GetInstance().Play("vacuum");
		}
	}

	public void linesCreated(int totalLines)
	{
		if(AudioManager.GetInstance () && totalLines >0)
		{
			if(totalLines == 1)
			{
				AudioManager.GetInstance().Stop("lines1");
				AudioManager.GetInstance().Play("lines1");
			}
			else if( totalLines == 2)
			{
				AudioManager.GetInstance().Stop("lines2");
				AudioManager.GetInstance().Play("lines2");
			}
			else
			{
				AudioManager.GetInstance().Stop("lines3orMore");
				AudioManager.GetInstance().Play("lines3orMore");
			}
		}
		
		if(totalLines > linesCreatedPoints.Count-1)
		{
			totalLines = linesCreatedPoints.Count-1;
		}
		//print ("totalLines" +totalLines);
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
		if(GemsManager.GetCastedInstance<GemsManager>())
		{
			hudManager.updateTextGems(GemsManager.GetCastedInstance<GemsManager>().currentGems);
		}

		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		//Se muestra el objetivo al inicio del nivel
		hudManager.showGoalAsLetters((goalManager.currentCondition == GoalManager.LETTERS));
		hudManager.setWinCondition (goalManager.currentCondition, goalManager.getGoalConditionParameters());

		activatePopUp ("startGamePopUp");
		startGameReference.initText (goalManager.currentCondition);

	}

	protected void checkIfLose()
	{
		bool canFit = false;

		//HACK: al inicio del nivel que sirve en los tutoriales
		if (linesAnimation.isOnAnimation || (remainingMoves == currentLevel.moves && !powerUpUsed)) 
		{
			updatePiecesLightAndUpdateLetterState ();
			return;
		}
		canFit = checkIfIsPosiblePutPieces ();

		if((!canFit || remainingMoves == 0) && !gameOver)
		{
			updatePiecesLightAndUpdateLetterState ();
			if(remainingMoves == 0)
			{
				allowGameInput(false);
			}

			StartCoroutine(checkIfReallyLost());
		}
		else if(!gameOver)
		{
			updatePiecesLightAndUpdateLetterState ();
		}
	}

	IEnumerator checkIfReallyLost()
	{
		yield return new WaitForEndOfFrame();

		if(!gameOver && (remainingMoves <= 0 || !wordManager.checkIfAWordIsPossible(gridCharacters)))
		{
			allowGameInput (false,true);
			Debug.Log ("Perdio de verdad");
			
			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("loose");
				AudioManager.GetInstance().Play("losse");
			}

			if(remainingMoves <=0)
			{
				activatePopUp ("noMovementsPopUp");
			}
			else
			{
				activatePopUp ("noOptionsPopUp");
			}

		}
	}

	public bool checkIfIsPosiblePutPieces()
	{
		bool canFit;

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
				if (tempList[i].toRotateObject != null) 
				{
					tempPiece = tempList [i].toRotateObject.GetComponent<Piece> ();
					for (int j = 0; j < 3; j++) 
					{
						if (tempPiece != null) 
						{
							tempList.Add (tempPiece);
							tempPiece.transform.localScale = tempList [0].transform.localScale;
							tempPiece = tempPiece.toRotateObject.GetComponent<Piece> ();
						}
					}
				}
			}
			canFit = cellManager.checkIfOnePieceCanFit (tempList);
		}

		return canFit;
	}

	public void updatePiecesLightAndUpdateLetterState()
	{
		updatePiecesLight (checkIfIsPosiblePutPieces ());
		updateLettersState ();
	}

	protected void updatePiecesLight(bool canFit)
	{
		List<Piece> temp = pieceManager.getShowingPieces ();

		for (int i = 0; i < temp.Count; i++) 
		{
			temp [i].switchGreyPiece (!canFit);
		}
	}

	/**
	 *Checar si puede hacer palabras
	 **/
	protected void updateLettersState()
	{
		if(wordManager.checkIfAWordIsPossible(gridCharacters))
		{
			wordManager.updateGridLettersState (gridCharacters,WordManager.EWordState.WORDS_AVAILABLE);
		}
		else if(gridCharacters.Count > 0)
		{				
			wordManager.updateGridLettersState (gridCharacters, WordManager.EWordState.NO_WORDS_AVAILABLE);
		}
	}

	public List<Letter> getGridCharacters()
	{
		return gridCharacters;
	}

	protected void secondChanceBought()
	{
		remainingMoves += secondChance.movements;
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
		expendedGems += secondChance.getCurrentPrice ();
		secondChanceFreeBombs ();
		allowGameInput (true);
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
			allowGameInput (false,true);
			useHintWord (false);
			//Debug.Log ("Gano de verdad.");
			gameOver = true;
			unlockPowerUp ();
			activatePopUp ("winGamePopUp");

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("won");
				AudioManager.GetInstance().Play("won");
			}
		}
	}

	protected void winBonification()
	{
		if (!PersistentData.GetInstance ().fromLevelBuilder) 
		{
			KuberaAnalytics.GetInstance ().registerForBeforeBonification (currentLevel.name, pointsCount, remainingMoves);
		}

		HighLightManager.GetInstance ().turnOffAllHighLights ();
		wordManager.updateGridLettersState (gridCharacters,WordManager.EWordState.WORDS_AVAILABLE);
		updatePiecesLight (true);


		bombAnimation.OnAllAnimationsCompleted += destroyAndCountAllLetters;

		allowGameInput (false,true);

		cellToLetter = new List<Cell> ();

		//Se limpian las letras 
		wordManager.removeAllLetters();

		expendMovement();
	}

	protected void expendMovement()
	{
		if(cellManager.getAllEmptyCells().Length > 0 && remainingMoves > 0 )
		{
			add1x1Block ();
			if (cellManager.getAllEmptyCells ().Length > 0) 
			{
				add1x1Block ();
			}
			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("addBlock");
				AudioManager.GetInstance().Play("addBlock");
			}
		}
		else
		{
			if (remainingMoves > 0) 
			{
				addMovementPoint ();
				addMovementPoint ();
				if(AudioManager.GetInstance())
				{
					AudioManager.GetInstance().Stop("letterPoint");
					AudioManager.GetInstance().Play("letterPoint");
				}
			}
			else
			{
				if(cellManager.existType(Piece.EType.PIECE))
				{
					cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.PIECE));
				}
				Invoke ("addWinLetterAfterActions", 0);
				updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
				return;
			}
		}
		updateHudGameInfo(remainingMoves,pointsCount,goalManager.currentCondition);
		Invoke ("expendMovement", 0.1f);
	}

	protected void addMovementPoint()
	{
		if (remainingMoves == 0) 
		{
			return;
		}

		addPoints (1);
		substractMoves (1);
	}

	protected void add1x1Block()
	{
		if (remainingMoves == 0) 
		{
			return;
		}

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

	protected void addWinLetterAfterActions()
	{
		int random = Random.Range (0, cellToLetter.Count);

		if (cellToLetter.Count > 0) 
		{
			StartCoroutine (bombAnimation.startSinglePieceAnimation (cellToLetter [random]));
			cellToLetter.RemoveAt (random);

			if (cellToLetter.Count > 0) 
			{
				random = Random.Range (0, cellToLetter.Count);
				StartCoroutine (bombAnimation.startSinglePieceAnimation (cellToLetter [random]));
				cellToLetter.RemoveAt (random);
			}

			Invoke ("addWinLetterAfterActions", 0.1f);
		}
		else
		{
			Invoke ("destroyAndCountAllLetters", 1);
		}
	}

	protected void destroyAndCountAllLetters()
	{
		cellToLetter = new List<Cell>();
		cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.LETTER));
		cellToLetter.AddRange (cellManager.getCellsOfSameType (Piece.EType.LETTER_OBSTACLE));
		Invoke ("destroyLetter", 0);
	}

	protected void destroyLetter()
	{
		int random = Random.Range (0, cellToLetter.Count);

		if (cellToLetter.Count > 0) 
		{
			showDestroyedLetterScore(cellToLetter[random]);
			cellToLetter [random].destroyCell ();
			cellToLetter.RemoveAt (random);

			if (cellToLetter.Count > 0) 
			{
				random = Random.Range (0, cellToLetter.Count);
				showDestroyedLetterScore (cellToLetter [random]);
				cellToLetter [random].destroyCell ();
				cellToLetter.RemoveAt (random);
			}

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("letterPoint");
				AudioManager.GetInstance().Play("letterPoint");
			}

			Invoke ("destroyLetter", 0.1f);
		} 
		else 
		{
			afterBonification ();
		}
	}

	protected void afterBonification()
	{
		#if UNITY_EDITOR
		if(!DataManagerKubera.GetInstance())
		{

		}
		else
		#endif	
		{
			if(!PersistentData.GetInstance ().fromLevelsToGame)
			{
				return;
			}
			#if UNITY_EDITOR
			if(cancelBonify)
			{
				//Se guarda en sus datos que ha pasado el nivel
				(DataManagerKubera.GetInstance() as DataManagerKubera).savePassedLevel(PersistentData.GetInstance().currentLevel.name,
					3,pointsCount);

				PersistentData.GetInstance ().fromGameToLevels = true;
				PersistentData.GetInstance ().fromLoose = false;
				PersistentData.GetInstance ().lastPlayedLevelStars = 3;
				PersistentData.GetInstance ().lastPlayedLevelPoints = pointsCount;
				LifesManager.GetInstance ().giveALife(1,true);

				Invoke ("toLevels", 0.75f);
			}
			else
			#endif	
			{
				//Se guarda en sus datos que ha pasado el nivel
				(DataManagerKubera.GetInstance() as DataManagerKubera).savePassedLevel(PersistentData.GetInstance().currentLevel.name,
					hudManager.getEarnedStars(),pointsCount);

				PersistentData.GetInstance ().fromGameToLevels = true;
				PersistentData.GetInstance ().fromLoose = false;
				PersistentData.GetInstance ().lastPlayedLevelStars = hudManager.getEarnedStars();
				PersistentData.GetInstance ().lastPlayedLevelPoints = pointsCount;

				LifesManager.GetInstance ().giveALife(1,true);

				Invoke ("toLevels", 0.75f);

				//Gano y ya se termino win bonification
				/*PersistentData.GetInstance().fromLevelBuilder = true;
				SceneManager.LoadScene ("Game");*/
			}
		}
	}

	protected void toLevels()
	{
		//activateMusic (false);
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance ().Stop ("gamePlay",false);
			AudioManager.GetInstance ().Play ("menuMusic");
		}

		if(ScreenManager.GetInstance())
		{
			//ScreenManager.GetInstance().preLoadingContinue();
		}


		if (!PersistentData.GetInstance ().fromLevelBuilder) 
		{
			KuberaAnalytics.GetInstance ().registerPowerUpsUse (currentLevel.name, powerUpsUsedCount,
				(DataManagerKubera.GetInstance () as DataManagerKubera).getLevelAttempts (currentLevel.name));
		}

		if (expendedGems != 0 && !PersistentData.GetInstance ().fromLevelBuilder) 
		{

			KuberaAnalytics.GetInstance ().registerGemsUsedOnLevel (currentLevel.name, expendedGems);

			if (!((DataManagerKubera)DataManagerKubera.GetInstance ()).alreadyUseGems ()) 
			{
				KuberaAnalytics.GetInstance ().registerGemsUsedForFirstTime (currentLevel.name);
				((DataManagerKubera)DataManagerKubera.GetInstance ()).markGemsAsUsed ();
			}

			if (((DataManagerKubera)DataManagerKubera.GetInstance ()).alreadyPurchaseGems () &&
			   !((DataManagerKubera)DataManagerKubera.GetInstance ()).alreadyUseGemsAfterPurchase ()) 
			{
				KuberaAnalytics.GetInstance ().registerGemsUsedAfterFirstPurchase (currentLevel.name);
				((DataManagerKubera)DataManagerKubera.GetInstance ()).markGemsAsUsedAfterPurchased ();
			}
		}

		ScreenManager.GetInstance().GoToScene ("Levels");
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

	public void allowGameInput(bool allowInput = true,bool deactivateAll = false)
	{
		if(deactivateAll)
		{
			hudManager.activatePowerUpButtont (false);
			inputPiece.allowInput = false;
			inputRotate.allowInput = false;
			hudManager.activatePowerUpButtont (false);
		}

		if (!rotationActive) 
		{
			inputPiece.allowInput = allowInput;
			inputWords.allowInput = allowInput;
		}


	}
		
	protected void unlockPowerUp()
	{
		if(currentLevel.unblockWordHint)
		{
			UserDataManager.instance.isWordHintPowerUpUnlocked = true;
		}
		if(currentLevel.unblockBomb)
		{
			UserDataManager.instance.isDestroyNeighborsPowerUpUnlocked = true;
		}
		if(currentLevel.unblockBlock)
		{
			UserDataManager.instance.isOnePiecePowerUpUnlocked = true;
		}
		if(currentLevel.unblockRotate)
		{
			UserDataManager.instance.isRotatePowerUpUnlocked = true;
		}
		if(currentLevel.unblockWildcard)
		{
			UserDataManager.instance.isWildCardPowerUpUnlocked = true;
		}
		if(currentLevel.unblockDestroy)
		{
			UserDataManager.instance.isDestroyPowerUpUnlocked = true;
		}
	}

	protected void unBlockPowerUpForThisLevel()
	{
		if(currentLevel.unblockWordHint)
		{
			hudManager.powerUps [0].gameObject.SetActive(true);
		}
		if(currentLevel.unblockBomb)
		{
			hudManager.powerUps [1].gameObject.SetActive(true);
		}
		if(currentLevel.unblockBlock)
		{
			hudManager.powerUps [2].gameObject.SetActive(true);
		}
		if(currentLevel.unblockRotate)
		{
			hudManager.powerUps [3].gameObject.SetActive(true);
		}
		if(currentLevel.unblockWildcard)
		{
			hudManager.powerUps [4].gameObject.SetActive(true);
		}
		if(currentLevel.unblockDestroy)
		{
			hudManager.powerUps [5].gameObject.SetActive(true);
		}
	}

	public void tryToActivatePowerup(int powerupTypeIndex)
	{
		//TODO: Chequeo con transaction manager para ver que onda con las gemas
		//TODO: Checar lo del precio de los powerUps

		if(!inputPiece.allowInput && !inputRotate.allowInput)
		{
			return;
		}

		allowGameInput(false);

		powerupManager.activatePowerUp((PowerupBase.EType) powerupTypeIndex,canActivatePowerUp((PowerupBase.EType) powerupTypeIndex));

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}

		if (!powerupManager.getPowerupByType ((PowerupBase.EType)powerupTypeIndex).isFree) 
		{
			gemsExpendedFeedBack.myText.text = "-" + powerupManager.getPowerUpPrice ((PowerupBase.EType)powerupTypeIndex);
			gemsExpendedFeedBack.gameObject.SetActive (true);
			gemsExpendedFeedBack.transform.GetChild (0).gameObject.SetActive (true);
		} 
		else 
		{
			gemsExpendedFeedBack.myText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FREE_POWERUP_PRICE);
			gemsExpendedFeedBack.gameObject.SetActive (true);
			gemsExpendedFeedBack.transform.GetChild (0).gameObject.SetActive (false);
		}
	}

	protected bool canActivatePowerUp(PowerupBase.EType type)
	{
		#if UNITY_EDITOR
		if(!GemsManager.GetCastedInstance<GemsManager>())
		{
			return true;
		}
		#endif

		//Checa si tiene dinero para usar el poder
		return powerupManager.getPowerupByType(type).isFree || GemsManager.GetCastedInstance<GemsManager>().isPossibleToConsumeGems(powerupManager.getPowerUpPrice(type));
	}

	private void OnPowerupCanceled(PowerupBase.EType type)
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("powerupCanceled");
			AudioManager.GetInstance().Play("powerupCanceled");
		}

		gemsExpendedFeedBack.gameObject.SetActive (false);
		
		allowGameInput(true);
	}

	private void OnPowerupCompleted(PowerupBase.EType type)
	{
		powerUpUsed = true;

		if(isBombAndSecondChance(type))
		{
			useFreeBomb ();
			powerUpsUsedCount[type.ToString()]++;
		}
		else
		{
			if(!powerupManager.getPowerupByType(type).isFree)
			{
				GemsManager.GetCastedInstance<GemsManager>().tryToConsumeGems(powerupManager.getPowerUpPrice(type));
				expendedGems += powerupManager.getPowerUpPrice (type);
				powerUpsUsedCount[type.ToString()]++;

				gemsExpendedFeedBack.myText.text = "-" + powerupManager.getPowerUpPrice (type);
			}

			Vector3 tempV3 = gemsExpendedFeedBack.transform.position;
			tempV3.y += cellManager.cellSize;
			gemsExpendedFeedBack.startAnim (gemsExpendedFeedBack.transform.position,tempV3);
		}

		allowGameInput(true);
	}

	private void OnPowerupCompletedNoGems(PowerupBase.EType type)
	{
		//TODO: abrimos el popUp de no Gems
		print ("noGems");
		activatePopUp ("NoGemsPopUp");

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

	public void useHintWord(bool use = false)
	{
		List<Letter> hintLetters = new List<Letter> ();
		hintLetters =  wordManager.findLetters (gridCharacters);

		if(use)
		{
			wordManager.cancelHint = false;
			wordManager.updateGridLettersState (hintLetters, WordManager.EWordState.HINTED_WORDS);
		}
		else
		{
			if(!wordManager.cancelHint)
			{				
				wordManager.cancelHinting();
			}
		}
	}

	public void activateSettings(bool activate)
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}
		hudManager.activateSettings (activate);
	}

	public void closeObjectivePopUp()
	{
		//AudioManagerOld.instance.PlaySoundEffect(AudioManagerOld.ESOUND_EFFECTS.BUTTON);
		//hudManager.hideGoalPopUp ();
		allowGameInput ();
	}

	public void popUpCompleted (string action ="")
	{
		switch (action) {
		case "startGame":
			allowGameInput ();
			updatePiecesLightAndUpdateLetterState ();
			hudManager.animateLvlGo ();
			TutorialManager.GetInstance ().init ();

			if(PersistentData.GetInstance().fromLevelsToGame)
			{
				LifesManager.GetInstance ().takeALife ();
			}
			break;
		case "endGame":
			toLevels ();
			break;
		case "winPopUpEnd":
			if(cancelBonify)
			{
				Invoke ("afterBonification", piecePositionedDelay * 2);
			}
			else
			{
				Invoke ("winBonification", piecePositionedDelay * 2);
			}
			break;
		case "looseNoMovements":
			activatePopUp ("SecondChance");
			break;
		case "loose":
			PersistentData.GetInstance ().fromLoose = true;
			PersistentData.GetInstance ().fromGameToLevels = true;
			toLevels ();
			break;
		case "NoGemsPopUp":
			activatePopUp ("NoGemsPopUp");
			break;
		case "checkInSeconds":
			Invoke ("checkIfLose", checkIfLoseSeconds);
			allowGameInput ();
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.ALL_POPUPS);

			break;
		default:		
			//print ("quien lo llama?");
				Invoke ("allowInputFromInvoke", 0);
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
		PersistentData.GetInstance().startLevel -= 1;
		PersistentData.GetInstance ().fromLoose = true;
		PersistentData.GetInstance ().fromGameToLevels = true;		
		activatePopUp ("exitGame");
		//toLevels ();
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
		case GoalManager.OBSTACLES:
			hudManager.actualizePointsOnObstacleLetters (goalManager.obstaclesCount.ToString(),goalManager.goalObstacles.ToString());
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

		//Se movio el chequeo para aca ya que aqui se suman lineas y puntos
		if (OnPointsEarned != null) 
		{
			OnPointsEarned ();
		}
	}

	protected void activateMusic(bool activate)
	{
		if(AudioManager.GetInstance())
		{
			if(activate)
			{
				AudioManager.GetInstance ().Play ("gamePlay");
			}
			else
			{
				AudioManager.GetInstance ().Stop ("gamePlay");
			}

		}
	}

	protected void isAnObjectMissing()
	{
		if(wordManager == null)
		{
			Debug.LogError ("misingObject: wordManager");
		}
		if(cellManager == null)
		{
			Debug.LogError ("misingObject: cellManager");
		}
		if(powerupManager == null)
		{
			Debug.LogError ("misingObject: powerupManager");
		}
		if(hudManager == null)
		{
			Debug.LogError ("misingObject: hudManager");
		}
		if(pieceManager == null)
		{
			Debug.LogError ("misingObject: pieceManager");
		}
		if(wordManager == null)
		{
			Debug.LogError ("misingObject: wordManager");
		}
		if(inputPiece == null)
		{
			Debug.LogError ("misingObject: inputPiece");
		}
		if(inputWords == null)
		{
			Debug.LogError ("misingObject: inputWords");
		}
		if(goalManager == null)
		{
			Debug.LogError ("misingObject: goalManager");
		}
		if(bombAnimation == null)
		{
			Debug.LogError ("misingObject: bombAnimation");
		}
		if(settingsButton == null)
		{
			Debug.LogError ("misingObject: settingsButton");
		}
		if(SecondChanceFreeBombs == null)
		{
			Debug.LogError ("misingObject: SecondChanceFreeBombs");
		}
		if(linesAnimation == null)
		{
			Debug.LogError ("misingObject: linesAnimation");
		}
		if(startGameReference == null)
		{
			Debug.LogError ("misingObject: startGameReference");
		}
	}
}