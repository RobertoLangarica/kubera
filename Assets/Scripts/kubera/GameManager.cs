using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	public GameObject toBuilderButton;

	//Texto del PoUp
	public Text scoreText;

	public GameObject bonificationPiece;

	public GameObject retryPopUp;
	public GameObject notEnoughLifesPopUp;

	public GameObject singleSquarePiece;
	public GameObject uiLetter;

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

	protected int currentWildCardsActivated;

	protected WordManager wordManager;
	protected CellsManager cellManager;
	protected PowerUpManager powerupManager;
	protected PieceManager pieceManager;
	protected HUD hud;

	protected InputPiece inputPiece;
	protected InputWords inputWords;

	protected Level currentLevel;
	protected List<GameObject> XMLPoolPiecesList = new List<GameObject>();
	protected List<ScriptableABCChar> XMLPoolLetersList = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> XMLPoolBlackLetersList = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> randomizedPoolLeters = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> randomizedBlackPoolLeters = new List<ScriptableABCChar>();
	protected List<ABCChar> listChar = new List<ABCChar>();

	void Awake () 
	{
		hud = FindObjectOfType<HUD> ();
		inputWords = FindObjectOfType<InputWords>();

		inputPiece = FindObjectOfType<InputPiece>();
		inputPiece.OnDrop += OnPieceDropped;

		cellManager = FindObjectOfType<CellsManager>();
		cellManager.OnLetterCreated += registerNewLetterCreated;

		wordManager = FindObjectOfType<WordManager>();
		wordManager.OnSendVector3 += sendVectorToCellManager;

		powerupManager = FindObjectOfType<PowerUpManager>();
		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		pieceManager = FindObjectOfType<PieceManager>();
	}

	void Start()
	{
		//TODO: Leer las gemas de algun lado
		UserDataManager.instance.playerGems = 300;

		setAndConfigureLevel(PersistentData.instance.currentLevel);
	}

	private void setAndConfigureLevel(Level level)
	{
		currentLevel = level;

		fillLettersPoolList ();
		fillPiecesPoolList ();

		pieceManager.initializeShowingPieces ();
		hud.showPieces (pieceManager.getShowingPieceList ());

		cellToLetter = new List<Cell> ();

		myWinCondition = currentLevel.winCondition.Split ('-');
		remainingMoves = totalMoves = currentLevel.moves;

		hud.setMovments (remainingMoves);



		hud.setGems(UserDataManager.instance.playerGems);
		hud.setLevelName (currentLevel.name);
		hud.setSecondChanceLock (false);

		//Se muestra el objetivo al inicio del nivel
		hud.showObjectivePopUp(myWinCondition[0],myWinCondition[1]);
		hud.OnObjectivePopUpClose += allowGameInput;
		allowGameInput(false);

		checkIfNeedToUnlockPowerUp();

		float[] scoreToStar = new float[3];
		scoreToStar [0] = PersistentData.instance.currentLevel.scoreToStar1;
		scoreToStar [1] = PersistentData.instance.currentLevel.scoreToStar2;
		scoreToStar [2] = PersistentData.instance.currentLevel.scoreToStar3;
		hud.setMeterData (scoreToStar);

		getWinCondition ();

		toBuilderButton.SetActive(false);
		if(PersistentData.instance.fromLevelBuilder)
		{
			PersistentData.instance.fromLevelBuilder = false;
			toBuilderButton.SetActive(true);
		}

		cellManager.resizeGrid(10,10);
		parseTheCellsOnGrid();
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.A))
		{
			parseTheCellsOnGrid ();
		}
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
		if (!cellManager.canPositionateAll (piece.pieces)) 
		{
			return false;
		}

		putPiecesOnGrid (piece);
		checkAndCompleteLines ();

		return true;
	}

	private void putPiecesOnGrid(Piece piece)
	{
		List<Cell> tempCell = new List<Cell> ();
		tempCell = cellManager.getCellsUnderPiece (piece);
		//damos puntos por las piezas en la pieza

		if (!piece.powerUp) 
		{
			pieceManager.removeFromListPieceUsed (piece.gameObject);
			addPointsActions (piece.pieces.Length);
			hud.showScoreTextAt(piece.transform.position,piece.pieces.Length);
		}

		for(int i=0; i< tempCell.Count; i++)
		{ 
			Vector3 cellPosition =  tempCell[i].transform.position + (new Vector3 (tempCell[i].GetComponent<SpriteRenderer> ().bounds.extents.x,
				-tempCell[i].GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

			cellManager.occupyAndConfigureCell (tempCell [i], piece.pieces [i], piece.currentType);

			tempCell[i].content.transform.DOMove (cellPosition, 0.5f);
		}

		for(int i = 0;i < piece.pieces.Length;i++)
		{
			piece.pieces[i].transform.SetParent(piece.transform.parent);
		}


		if (pieceManager.getShowingPieceList ().Count == 0) 
		{
			pieceManager.initializeShowingPieces ();
			hud.showPieces (pieceManager.getShowingPieceList ());
		}

		Destroy(piece.gameObject);
	}

	private void checkAndCompleteLines()
	{
		List<List<Cell>> cellList = new List<List<Cell>> ();
		cellList = cellManager.getCompletedVerticalAndHorizontalLines ();

		//damos puntos por las lineas creadas
		linesCreated (cellList.Count);

		if (cellList.Count > 0) 
		{			
			for (int i = 0; i < cellList.Count; i++) 
			{
				for(int j=0; j<cellList[i].Count; j++)
				{
					if (cellList [i] [j].pieceType != EPieceType.LETTER) 
					{
						GameObject cellContent = createLetterContent();
						Vector3 cellPosition =  cellList [i] [j].transform.position + (new Vector3 (cellList [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x,
							-cellList [i] [j].GetComponent<SpriteRenderer> ().bounds.extents.x, 0));
						
						cellManager.occupyAndConfigureCell (cellList [i] [j], cellContent, EPieceType.LETTER);
						cellContent.transform.DOMove (cellPosition, 0);

					}
				}
			}
		}
	}

	protected void parseTheCellsOnGrid()
	{
		GameObject cellContent = null;
		string[] levelGridData = PersistentData.instance.currentLevel.grid.Split(',');
		int cellType = 0;

		for(int i = 0;i < levelGridData.Length;i++)
		{
			cellType = int.Parse(levelGridData[i]);

			cellManager.setCellToType (i, cellType);
			if((cellType & 0x1) == 0x1)
			{
				cellManager.setCellType(i,EPieceType.NONE);
			}
			if((cellType & 0x2) == 0x2)
			{
				cellContent = createCellBlockContent(cellType);
				cellManager.setCellType(i,cellContent.GetComponent<Piece> ().currentType);
				cellManager.setCellContent(i,cellContent);
			}
			if((cellType & 0x4) == 0x4)
			{
				cellManager.setCellType(i,EPieceType.NONE);
			}
			if((cellType & 0x8) == 0x8)
			{	
				cellContent = createCellObstacleContent();
				cellManager.setCellType(i,EPieceType.LETTER_OBSTACLE);
				cellManager.setCellContent(i,cellContent);
			}
		}
	}

	protected GameObject createCellBlockContent(int contentColor)
	{
		GameObject go = GameObject.Instantiate (singleSquarePiece) as GameObject;
		int tempType = contentColor >> 4;

		go.GetComponent<BoxCollider2D> ().enabled = false;

		switch(tempType)
		{
		case(1):
			go.GetComponent<Piece> ().currentType = EPieceType.AQUA;
			//Debug.Log(typeOfPiece);
			break;
		case(2):
			go.GetComponent<Piece> ().currentType = EPieceType.BLUE;
			//Debug.Log(typeOfPiece);
			break;
		case(3):
			go.GetComponent<Piece> ().currentType = EPieceType.GREEN;
			//Debug.Log(typeOfPiece);
			break;
		case(4):
			go.GetComponent<Piece> ().currentType = EPieceType.MAGENTA;
			//Debug.Log(typeOfPiece);
			break;
		case(5):
			go.GetComponent<Piece> ().currentType = EPieceType.RED;
			//Debug.Log(typeOfPiece);
			break;
		case(6):
			go.GetComponent<Piece> ().currentType = EPieceType.YELLOW;
			//Debug.Log(typeOfPiece);
			break;
		case(7):
			go.GetComponent<Piece> ().currentType = EPieceType.GREY;
			//Debug.Log(typeOfPiece);
			break;
		}

		return go;
	}

	protected GameObject createCellObstacleContent()
	{
		GameObject go = Instantiate (uiLetter)as GameObject;

		go.transform.SetParent (GameObject.Find("CanvasOfLetters").transform,false);

		go.GetComponent<BoxCollider2D>().enabled = true;

		go.GetComponent<BoxCollider2D>().size =  go.GetComponent<RectTransform> ().rect.size;

		registerNewLetterCreated (go.GetComponent<ABCChar> (), go.GetComponent<UIChar> (), true);

		return go;
	}

	protected GameObject createLetterContent()
	{
		GameObject go = Instantiate (uiLetter)as GameObject;

		go.transform.SetParent (GameObject.Find("CanvasOfLetters").transform,false);

		go.GetComponent<BoxCollider2D>().enabled = true;

		go.GetComponent<BoxCollider2D>().size =  go.GetComponent<RectTransform> ().rect.size;

		registerNewLetterCreated (go.GetComponent<ABCChar> (), go.GetComponent<UIChar> (), false);

		return go;
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
		if(checkIfExistEnoughGems(gemsPrice))
		{
			UserDataManager.instance.playerGems -= gemsPrice;

			hud.setGems(UserDataManager.instance.playerGems);

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

	protected void addPointsActions(int length)
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

		/*if(UserDataManager.instance.playerGems >= (powerUpManager2.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated) && activatedPowerUp)
		{
			UserDataManager.instance.playerGems -= (powerUpManager2.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated);

			if(activatedPowerUp.typeOfPowerUp == EPOWERUPS.WILDCARD_POWERUP)
			{
				deactivateCurrentPowerUp();
			}
			currentWildCardsActivated = 0;
			return true;
		}*/
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

		if(wordManager.wordsValidator.completeWord && canUseAllWildCards)
		{
			//useGems(powerUpManager.getPowerUp(EPOWERUPS.WILDCARD_POWERUP).gemsPrice * currentWildCardsActivated);
			
			for(int i = 0;i < wordManager.chars.Count;i++)
			{
				letterFound = false;
				switch(wordManager.chars[i].pointsOrMultiple)
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
					{amount += int.Parse(wordManager.chars[i].pointsOrMultiple);}
					break;
				}
				if (wordManager.chars [i].type == ABCChar.EType.OBSTACLE) 
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
		}

		wordManager.resetValidation();
		addPointsActions (amount);
	}

	protected void sendVectorToCellManager(Vector3 vector3)
	{
		cellManager.getCellUnderPoint(vector3).clearCell();
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
			abcChar.initializeFromScriptableABCChar(giveBlackLetterInfo());
		}
		else
		{
			abcChar.initializeFromScriptableABCChar(giveLetterInfo());
		}

		uiChar.type = abcChar.type;
		uiChar.changeColorAndSetValues(abcChar.character.ToLower ());

		listChar.Add(abcChar);
	}

	protected void getWinCondition()
	{
		if(myWinCondition[0] == "letters")
		{
			letters = new List<string> ();
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
					letters.Add (temp [1]);
				}
			}
		}
		if(myWinCondition[0] == "word")
		{
			words = myWinCondition [1].Split ('_');
		}
		hud.setWinConditionOnHud (myWinCondition [0],words,int.Parse(myWinCondition [1]),letters);
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
		FindObjectOfType<WordManager>().checkIfAWordisPossible(listChar);
	}

	public void checkToLoose()
	{
		if(!cellManager.checkIfOneCanFit(pieceManager.getShowingPieceList()) || remainingMoves == 0)
		{
			Debug.Log ("Perdio");
			while(true)
			{
				bool pass = true;
				for(int i=0; i < listChar.Count; i++)
				{
					if(!listChar[i])
					{
						listChar.RemoveAt(i);
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
		Cell[] emptyCells = cellManager.getAllEmptyCells();
		Cell cell;

		if (remainingMoves != 0 && emptyCells.Length > 0) 
		{
			cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];


			remainingMoves--;
			hud.setMovments (remainingMoves);

			GameObject go = GameObject.Instantiate (bonificationPiece) as GameObject;

			go.GetComponent<Piece> ().currentType = cellManager.colorOfMoreQuantity ();

			cellManager.setCellContent(cell,go);
			cellManager.setCellType(cell,go.GetComponent<Piece> ().currentType);

			StartCoroutine (add1x1BlockMore ());
		}
		else 
		{
			cellToLetter.AddRange (cellManager.getCellsOfSameType (cellManager.colorOfMoreQuantity ()));
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
			cellToLetter = new List<Cell>(cellManager.getCellsOfSameType(cellManager.colorOfMoreQuantity()));
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
		cellToLetter.AddRange (cellManager.getCellsOfSameType (EPieceType.LETTER));
		cellToLetter.AddRange (cellManager.getCellsOfSameType (EPieceType.LETTER_OBSTACLE));
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
			switch (cellToLetter[i].content.GetComponent<ABCChar>().pointsOrMultiple) 
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
					amount += int.Parse (cellToLetter[i].content.GetComponent<ABCChar>().pointsOrMultiple);}
				break;
			}
		}

		amount *= multiplierHelper;
		addPoints(amount,false);
	}

	protected void allowGameInput(bool allowInput = true)
	{
		inputPiece.allowInput = allowInput;
		inputWords.allowInput = allowInput;
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
		else if(remainingMoves >= 10)
		{
			winBombs = 5;
		}
	}

	protected void checkIfNeedToUnlockPowerUp()
	{
		if(currentLevel.unblockBlock)
		{
			//powerUpManager2.activatePower(EPOWERUPS.BLOCK_POWERUP);
		}
		if(currentLevel.unblockBomb)
		{
			//powerUpManager2.activatePower(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP);
		}
		if(currentLevel.unblockDestroy)
		{
			//powerUpManager2.activatePower(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP);
		}
		if(currentLevel.unblockRotate)
		{
			//powerUpManager2.activatePower(EPOWERUPS.ROTATE_POWERUP);
		}
		if(currentLevel.unblockWildcard)
		{
			//powerUpManager2.activatePower(EPOWERUPS.WILDCARD_POWERUP);
		}
	}

	protected void UnlockPowerUp()
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

	protected void fillPiecesPoolList()
	{
		string[] piecesInfo;
		int amout = 0;

		string[] myPieces = PersistentData.instance.currentLevel.pieces.Split(new char[1]{','});

		for(int i =0; i<myPieces.Length; i++)
		{
			piecesInfo = myPieces[i].Split(new char[1]{'_'});
			amout = int.Parse(piecesInfo[0]);

			for(int j=0; j<amout; j++)
			{
				XMLPoolPiecesList.Add ((GameObject)(Resources.Load (piecesInfo[1])));
			}
		}

		pieceManager.setPiecesInList (XMLPoolPiecesList);
	}

	protected void fillLettersPoolList()
	{
		string[] lettersPool = PersistentData.instance.currentLevel.lettersPool.Split(new char[1]{','});
		string[] piecesInfo;

		/*Aqui diseccionar el XML****************/
		int amout = 0;
		ScriptableABCChar newLetter = null;

		if(XMLPoolLetersList.Count == 0)
		{
			for(int i =0; i<lettersPool.Length; i++)
			{
				piecesInfo = lettersPool[i].Split(new char[1]{'_'});
				amout = int.Parse(piecesInfo[0]);
				for(int j = 0;j < amout;j++)
				{
					newLetter = new ScriptableABCChar();
					newLetter.character = piecesInfo[1];
					newLetter.pointsOrMultiple = piecesInfo[2];
					newLetter.type = int.Parse(piecesInfo[3]);

					XMLPoolLetersList.Add(newLetter);
				}
			}
			if(PersistentData.instance.currentLevel.obstacleLettersPool.Length > 0)
			{
				lettersPool = PersistentData.instance.currentLevel.obstacleLettersPool.Split(new char[1]{','});

				for(int i =0; i<lettersPool.Length; i++)
				{
					piecesInfo = lettersPool[i].Split(new char[1]{'_'});
					amout = int.Parse(piecesInfo[0]);
					for(int j = 0;j < amout;j++)
					{
						newLetter = new ScriptableABCChar();
						newLetter.character = piecesInfo[1];
						newLetter.pointsOrMultiple = piecesInfo[2];
						newLetter.type = int.Parse(piecesInfo[3]);
						XMLPoolBlackLetersList.Add(newLetter);
					}
				}
			}
		}
		/*****/
		fillLetterPoolRandomList ();
		fillBlackLetterPoolRandomList ();
	}

	protected void fillLetterPoolRandomList()
	{
		List<ScriptableABCChar> tempList = new List<ScriptableABCChar> ();
		randomizedPoolLeters = new List<ScriptableABCChar>(XMLPoolLetersList);

		tempList = new List<ScriptableABCChar>(XMLPoolLetersList);


		while(tempList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,tempList.Count);
			randomizedBlackPoolLeters.Add(tempList[val]);
			tempList.RemoveAt(val);
		}
	}

	protected void fillBlackLetterPoolRandomList()
	{
		List<ScriptableABCChar> tempList = new List<ScriptableABCChar> ();
		randomizedBlackPoolLeters = new List<ScriptableABCChar>(XMLPoolBlackLetersList);

		tempList = new List<ScriptableABCChar>(XMLPoolBlackLetersList);


		while(tempList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,tempList.Count);
			randomizedBlackPoolLeters.Add(tempList[val]);
			tempList.RemoveAt(val);
		}
	}

	public ScriptableABCChar giveLetterInfo()
	{
		if(randomizedPoolLeters.Count==0)
		{
			fillLetterPoolRandomList();
		}

		ScriptableABCChar letter = randomizedPoolLeters[0];
		randomizedPoolLeters.RemoveAt (0);

		return letter;
	}

	public ScriptableABCChar giveBlackLetterInfo()
	{
		if(randomizedBlackPoolLeters.Count==0)
		{
			fillBlackLetterPoolRandomList();
		}
		ScriptableABCChar letter = randomizedBlackPoolLeters[0];
		randomizedBlackPoolLeters.RemoveAt (0);

		return letter;
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

	public void activateSettings()
	{
		hud.activateSettings ();
	}

	//TODO
	public void activateMusic()
	{
	}

	public void activateSounds()
	{
	}

	public void exit()
	{
		
	}
}
