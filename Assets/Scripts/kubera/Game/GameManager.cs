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
	public GameObject singleSquarePiecePrefab;

	protected int pointsCount = 0;
	protected int wordsMade = 0;
	protected bool wordFound;
	protected List<string> goalLetters;
	protected List<string> goalWords = new List<string>();
	protected int obstaclesCount = 0;
	protected int obstaclesUsed = 0;

	protected string[] myWinCondition;

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

	protected WordManager	wordManager;
	protected CellsManager	cellManager;
	protected PowerUpManager powerupManager;
	protected PieceManager 	 pieceManager;
	protected HUDManager 	 hudManager;
	protected AudioManager 	 audioManager;
	protected InputPiece inputPiece;
	protected InputWords inputWords;
	protected Level currentLevel;
	protected List<Letter> gridCharacters = new List<Letter>();


	void Start()
	{
		hudManager = FindObjectOfType<HUDManager> ();
		inputWords = FindObjectOfType<InputWords>();

		inputPiece = FindObjectOfType<InputPiece>();
		inputPiece.OnDrop += OnPieceDropped;

		cellManager = FindObjectOfType<CellsManager>();

		wordManager = FindObjectOfType<WordManager>();
		wordManager.setMaxAllowedLetters(PersistentData.instance.maxWordLength);
		wordManager.gridLettersParent = gridLettersContainer;

		powerupManager = FindObjectOfType<PowerUpManager>();
		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		pieceManager = FindObjectOfType<PieceManager>();

		audioManager = FindObjectOfType<AudioManager>();

		//LetterSize
		Vector3 cellSizeReference = cellManager.cellPrefab.GetComponent<SpriteRenderer>().bounds.size;
		Vector3 lettersizeDelta = (Camera.main.WorldToScreenPoint(cellSizeReference) -Camera.main.WorldToScreenPoint(Vector3.zero)) * gridLettersSizeMultiplier;
		lettersizeDelta.x = Mathf.Abs(lettersizeDelta.x);
		lettersizeDelta.y = Mathf.Abs(lettersizeDelta.y);

		wordManager.gridLettersSizeDelta = new Vector2(lettersizeDelta.x, lettersizeDelta.y);

		//TODO: Leer las gemas de algun lado
		UserDataManager.instance.playerGems = 300;

		//TODO: el release no manda un random
		#if UNITY_ANDROID
		setAndConfigureLevel(PersistentData.instance.getRandomLevel());
		#else
		setAndConfigureLevel(PersistentData.instance.currentLevel);
		#endif
	}

	private void setAndConfigureLevel(Level level)
	{
		currentLevel = level;

		readLettersFromLevel(level);
		pieceManager.setPieces(getPiecesPoolFromLevel(level));

		pieceManager.initializePiecesToShow ();
		hudManager.showPieces (pieceManager.getShowingPieces ());

		cellToLetter = new List<Cell> ();

		myWinCondition = currentLevel.goal.Split ('-');
		remainingMoves = totalMoves = currentLevel.moves;

		hudManager.setGems(UserDataManager.instance.playerGems);
		hudManager.setLevelName (currentLevel.name);
		hudManager.setSecondChanceLock (false);

		allowGameInput(false);

		float[] scoreToStar = new float[3];
		scoreToStar [0] = currentLevel.scoreToStar1;
		scoreToStar [1] = currentLevel.scoreToStar2;
		scoreToStar [2] = currentLevel.scoreToStar3;
		hudManager.setStarData (scoreToStar);
	
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

	protected List<Piece> getPiecesPoolFromLevel(Level level)
	{
		string[] info;
		int amount = 0;

		string[] piecesInfo = level.pieces.Split(',');
		List<Piece> pieces = new List<Piece>();

		for(int i =0; i<piecesInfo.Length; i++)
		{
			info = piecesInfo[i].Split('_');
			amount = int.Parse(info[0]);

			for(int j=0; j<amount; j++)
			{
				pieces.Add (((GameObject)(Resources.Load (info[1]))).GetComponent<Piece>());
				pieces[pieces.Count-1].setUniqueId();
			}
		}

		return pieces;
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
				Piece content = createSingleSquarePiece(singleSquarePiecePrefab, cellType>>6);
				cellManager.occupyAndConfigureCell(i,content.gameObject,content.currentType,true);
			}
			else if((cellType & 0x8) == 0x8)
			{	
				//Obstaculo
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.OBSTACLE);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,EPieceType.LETTER_OBSTACLE,true);
				obstaclesCount++;

				gridCharacters.Add(letter);
			}
			else if((cellType & 0x20) == 0x20)
			{	
				//De tutorial
				Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.TUTORIAL);
				cellManager.occupyAndConfigureCell(i,letter.gameObject,EPieceType.LETTER,true);
				tutorialLetters.Add(letter);

				gridCharacters.Add(letter);
			}
		}

		if(tutorialLetters.Count > 0)
		{
			selectLettersFromGrid(tutorialLetters, level.tutorialLettersPool.Split('-')[1].Split(','));
		}
	}

	protected Piece createSingleSquarePiece(GameObject sourcePrefab, int colorIndex)
	{
		GameObject go = GameObject.Instantiate (sourcePrefab) as GameObject;
		go.GetComponent<BoxCollider2D> ().enabled = false;
		Piece piece = go.GetComponent<Piece>();
		piece.currentType = (EPieceType)colorIndex;

		return piece;
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
		if (!piece) 
		{
			return;
		}

		if(!dropPieceOnGrid(piece))
		{
			inputPiece.returnSelectedToInitialState (0.1f);
		}

		inputPiece.reset ();
	}

	public bool dropPieceOnGrid(Piece piece)
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

			piecePosition =  cells[i].transform.position + (new Vector3 (cells[i].GetComponent<SpriteRenderer> ().bounds.extents.x,
				-cells[i].GetComponent<SpriteRenderer> ().bounds.extents.y, 0));
			
			piece.squares [i].transform.DOMove (piecePosition, piecePositionedDelay);

		}

		//DONE: Hay que poner un comentario aqui que nos diga que pasa con esta reenparentada

		//Se reasigna el padre de los squares, para poder destruir su contenedor tras averlo posicionado, para evitar tener GameObjects en desuso en la escena
		for(int i = 0;i < piece.squares.Length;i++)
		{
			piece.squares[i].transform.SetParent(piece.transform.parent);
		}
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
			actionFinished(piece.squares.Length);
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
				if (cells [i] [j].contentType != EPieceType.LETTER) 
				{
					Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL);

					Vector3 cellPosition =  cells [i] [j].transform.position + (new Vector3 (cells [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x,
						-cells [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

					cellManager.occupyAndConfigureCell (cells [i] [j], letter.gameObject, EPieceType.LETTER);
					letter.gameObject.transform.DOMove (cellPosition, 0);
					gridCharacters.Add(letter);
				}
			}
		}
	}

	//TODO: checar el nombre de la funcion
	protected void actionFinished(int points,int movements = 1)
	{
		addPoints (points);
		substractMoves(movements);
		actualizeHUDInfo ();
		if (checkGoal ()) 
		{
			won ();
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
		int amount = 0;
		int multiplierHelper = 1;
		bool letterFound;
		bool canUseAllWildCards;

		for(int i = 0;i < wordManager.letters.Count;i++)
		{
			letterFound = false;

			if (wordManager.letters[i].type == Letter.EType.OBSTACLE) 
			{
				obstaclesUsed++;
			}

			if (myWinCondition [0] == "letters") 
			{
				for (int j = 0; j < goalLetters.Count; j++) 
				{
					if (goalLetters [j] == wordManager.letters [i].abcChar.character && !letterFound) 
					{
						print (goalLetters [j]);
						print ( wordManager.letters [i].abcChar.character);

						hudManager.destroyLetterFound (goalLetters [j]);

						goalLetters.RemoveAt (j);
						letterFound = true;
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

		amount *= multiplierHelper;

		wordsMade++;

		wordManager.removeAllLetters();

		actionFinished (amount);
	}

	protected void sendVectorToCellManager(Vector3 vector3)
	{
		cellManager.getCellUnderPoint(vector3).clearCell();
	}

	public void linesCreated(int totalLines)
	{
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
		int quantity = 0;
		string word = "";
		bool isLetterCondition = false;
		if(myWinCondition[0] == "letters")
		{
			goalLetters = new List<string> ();
			int i;
			string[] s = myWinCondition [1].Split (',');
			string[] temp;

			for(i=0; i< s.Length; i++)
			{
				temp = s [i].Split ('_');
				int amount = int.Parse (temp [0]);

				for(int j=0; j<amount; j++)
				{
					//print (temp [1]);
					goalLetters.Add (temp [1]);
				}
			}
			isLetterCondition = true;
			hudManager.setLettersCondition (goalLetters);
		}
		else if(myWinCondition[0] == "obstacles")
		{
			quantity = obstaclesCount;
			hudManager.setObstaclesCondition (quantity);
		}
		else if(myWinCondition[0] == "word")
		{
			string[] text = myWinCondition [1].Split (',');

			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hudManager.setWordCondition (word);
		}
		else if(myWinCondition[0] == "sin")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hudManager.setSinCondition (word);
		}
		else if(myWinCondition[0] == "ant")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hudManager.setAntCondition (word);
		}
		else if(myWinCondition[0] == "points")
		{
			quantity = int.Parse (myWinCondition [1]);
			hudManager.setPointsCondition (quantity,pointsCount);
		}
		else if(myWinCondition[0] == "words")
		{
			quantity = int.Parse (myWinCondition [1]);
			hudManager.setWordsCondition (quantity,wordsMade);
		}

		//Se muestra el objetivo al inicio del nivel
		hudManager.setGoal(isLetterCondition);
		hudManager.showObjectivePopUp(myWinCondition[0],word,quantity,goalLetters);
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


	protected bool checkGoal ()
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

	protected void won()
	{
		print ("win");
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
				if(cellManager.colorOfMoreQuantity() != EPieceType.NONE)
				{
					cellToLetter.AddRange (cellManager.getCellsOfSameType (cellManager.colorOfMoreQuantity ()));
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
			cellManager.occupyAndConfigureCell (cellToLetter [random],letter.gameObject,EPieceType.LETTER,true);
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
		if(cellManager.colorOfMoreQuantity() != EPieceType.NONE)
		{
			cellToLetter = new List<Cell>(cellManager.getCellsOfSameType(cellManager.colorOfMoreQuantity()));
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
		cellToLetter.AddRange (cellManager.getCellsOfSameType (EPieceType.LETTER));
		cellToLetter.AddRange (cellManager.getCellsOfSameType (EPieceType.LETTER_OBSTACLE));
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
		hudManager.hideObjectivePopUp ();
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