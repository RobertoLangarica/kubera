﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	public Text scoreText;

	public GameObject bonificationPiecePrefab;
	public GameObject retryPopUpPrefab;
	public GameObject notEnoughLifesPopUpPrefab;
	public GameObject singleSquarePiecePrefab;
	public GameObject gridLetterPrefab;

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
	public float piecePositionedDelay = 0.5f;

	public bool canRotate;
	public bool destroyByColor;

	public Transform canvasOfLetters;

	protected List<Cell> cellToLetter;

	protected int currentWildCardsActivated;

	protected WordManager wordManager;
	protected CellsManager cellManager;
	protected PowerUpManager powerupManager;
	protected PieceManager pieceManager;
	protected HUD hud;
	protected AudioManager audioManager;

	protected InputPiece inputPiece;
	protected InputWords inputWords;

	public float totalLetterPercentToFillCell = 0.9f;

	protected Level currentLevel;
	protected RandomPool<ABCCharinfo> lettersPool;
	protected RandomPool<ABCCharinfo> obstaclesLettersPool;
	protected RandomPool<ABCCharinfo> tutorialLettersPool;
	protected List<ABCChar> charactersOnGrid = new List<ABCChar>();

	protected bool playerWon;

	void Awake () 
	{
		hud = FindObjectOfType<HUD> ();
		inputWords = FindObjectOfType<InputWords>();

		inputPiece = FindObjectOfType<InputPiece>();
		inputPiece.OnDrop += OnPieceDropped;

		cellManager = FindObjectOfType<CellsManager>();

		wordManager = FindObjectOfType<WordManager>();
		wordManager.OnSendVector3 += sendVectorToCellManager;
		wordManager.OnLettersActualized += showPointsOfLettersSelected;

		powerupManager = FindObjectOfType<PowerUpManager>();
		powerupManager.OnPowerupCanceled = OnPowerupCanceled;
		powerupManager.OnPowerupCompleted = OnPowerupCompleted;

		pieceManager = FindObjectOfType<PieceManager>();

		audioManager = FindObjectOfType<AudioManager>();
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

		fillLettersPool();
		pieceManager.setPieces(getPiecesFromLevel(level));

		pieceManager.initializePiecesToShow ();
		hud.showPieces (pieceManager.getShowingPieces ());

		cellToLetter = new List<Cell> ();

		myWinCondition = currentLevel.winCondition.Split ('-');
		remainingMoves = totalMoves = currentLevel.moves;

		hud.setMovements (remainingMoves);
		hud.setGems(UserDataManager.instance.playerGems);
		hud.setLevelName (currentLevel.name);
		hud.setSecondChanceLock (false);

		allowGameInput(false);

		float[] scoreToStar = new float[3];
		scoreToStar [0] = currentLevel.scoreToStar1;
		scoreToStar [1] = currentLevel.scoreToStar2;
		scoreToStar [2] = currentLevel.scoreToStar3;
		hud.setMeterData (scoreToStar);
	
		cellManager.resizeGrid(10,10);
		parseTheCellsOnGrid();

		getWinCondition ();
	}

	protected void fillLettersPool()
	{
		string[] letters = currentLevel.lettersPool.Split(',');
		string[] piecesInfo;

		/*Aqui diseccionar el XML****************/
		int amount = 0;
		ABCCharinfo newLetter = null;
		List<ABCCharinfo> result = new List<ABCCharinfo>();

		/**
		 * CSV
		 * cantidad_letra_puntos/multiplo_tipo
		 * ej. 02_A_1_1
		 **/ 
		for(int i =0; i<letters.Length; i++)
		{
			piecesInfo = letters[i].Split('_');
			amount = int.Parse(piecesInfo[0]);

			for(int j = 0;j < amount;j++)
			{
				newLetter = new ABCCharinfo();
				newLetter.character = piecesInfo[1];
				newLetter.pointsOrMultiple = piecesInfo[2];
				newLetter.type = int.Parse(piecesInfo[3]);

				result.Add(newLetter);
			}
		}
		lettersPool = new RandomPool<ABCCharinfo>(result);


		if(currentLevel.obstacleLettersPool.Length > 0)
		{
			result.Clear();
			letters = currentLevel.obstacleLettersPool.Split(',');

			for(int i =0; i<letters.Length; i++)
			{
				piecesInfo = letters[i].Split('_');
				amount = int.Parse(piecesInfo[0]);
				for(int j = 0;j < amount;j++)
				{
					newLetter = new ABCCharinfo();
					newLetter.character = piecesInfo[1];
					newLetter.pointsOrMultiple = piecesInfo[2];
					newLetter.type = int.Parse(piecesInfo[3]);
					result.Add(newLetter);
				}
			}

			obstaclesLettersPool = new RandomPool<ABCCharinfo>(result);
		}
		if(currentLevel.tutorialLettersPool.Length > 1)
		{
			result.Clear();
			string[] tutorialInfo = currentLevel.tutorialLettersPool.Split('-');
			letters = tutorialInfo[0].Split(',');

			for(int i =0; i<letters.Length; i++)
			{
				piecesInfo = letters[i].Split('_');
				amount = int.Parse(piecesInfo[0]);
				for(int j = 0;j < amount;j++)
				{
					newLetter = new ABCCharinfo();
					newLetter.character = piecesInfo[1];
					newLetter.pointsOrMultiple = piecesInfo[2];
					newLetter.type = int.Parse(piecesInfo[3]);
					result.Add(newLetter);
				}
			}

			tutorialLettersPool = new RandomPool<ABCCharinfo>(result);
		}
	}

	protected List<Piece> getPiecesFromLevel(Level level)
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
			}
		}

		return pieces;
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
		if (cellManager.canPositionateAll (piece.squares)) 
		{
			putPiecesOnGrid (piece);
			audioManager.PlayPiecePositionedAudio();
			checkForCompletedLines();
			StartCoroutine(afterPiecePositioned(piece));
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

		//TODO: Hay que poner un comentario aqui que nos diga que pasa con esta reenparentada
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
				hud.showPieces (pieceManager.getShowingPieces ());
			}

			//Damos puntos por cada cuadro en la pieza
			addPoints (piece.squares.Length);
			substractMoves(1);
			hud.showScoreTextAt(piece.transform.position,piece.squares.Length);
		}

		Destroy(piece.gameObject);
	}

	private void checkForCompletedLines()
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
						GameObject cellContent = getAndRegisterNewLetter("normal");
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
		string[] levelGridData = currentLevel.grid.Split(',');
		int cellType = 0;

		List<GameObject> tutorialLetters = new List<GameObject>();

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
				cellManager.occupyAndConfigureCell(i,cellContent,cellContent.GetComponent<Piece> ().currentType,true);
			}
			if((cellType & 0x4) == 0x4)
			{
				cellManager.setCellType(i,EPieceType.NONE);
			}
			if((cellType & 0x8) == 0x8)
			{	
				cellContent = getAndRegisterNewLetter("obstacle");
				cellManager.occupyAndConfigureCell(i,cellContent,EPieceType.LETTER_OBSTACLE,true);
				obstaclesCount++;
			}
			if((cellType & 0x20) == 0x20)
			{	
				cellContent = getAndRegisterNewLetter("tutorial");
				cellManager.occupyAndConfigureCell(i,cellContent,EPieceType.LETTER,true);
				tutorialLetters.Add(cellContent);
			}
		}

		if(tutorialLetters.Count > 0)
		{
			selectLettersForTutorial(tutorialLetters);
		}
	}

	protected void selectLettersForTutorial(List<GameObject> tutorialLetters)
	{
		string[] selectedLetters = currentLevel.tutorialLettersPool.Split('-')[1].Split(',');
		ABCChar abcChar;

		for(int i = 0;i < selectedLetters.Length;i++)
		{
			for(int j = 0;j < tutorialLetters.Count;j++)
			{
				abcChar = tutorialLetters[j].GetComponent<ABCChar>();

				if(abcChar.character == selectedLetters[i])
				{
					wordManager.sendLetterToWord(tutorialLetters[j]);
					tutorialLetters.RemoveAt(j);
					break;
				}
			}
		}
	}

	protected GameObject createCellBlockContent(int contentColor)
	{
		GameObject go = GameObject.Instantiate (singleSquarePiecePrefab) as GameObject;
		int tempType = contentColor >> 6;

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

	protected GameObject createLetter()
	{
		GameObject go = Instantiate (gridLetterPrefab)as GameObject;
		SpriteRenderer sprite = cellManager.cellPrefab.GetComponent<SpriteRenderer>();

		go.transform.SetParent (canvasOfLetters,false);

		go.GetComponent<BoxCollider2D>().enabled = true;

		Vector3 letterSize = (Camera.main.WorldToScreenPoint(sprite.bounds.size) -Camera.main.WorldToScreenPoint(Vector3.zero)) * totalLetterPercentToFillCell;
		go.GetComponent<RectTransform> ().sizeDelta = new Vector2(Mathf.Abs(letterSize.x),Mathf.Abs(letterSize.y));

		go.GetComponent<BoxCollider2D>().size =  go.GetComponent<RectTransform> ().rect.size;

		return go;
	}

	public GameObject getAndRegisterNewLetter(string letterType)
	{
		GameObject newLetter = createLetter();

		ABCChar abcChar = newLetter.GetComponent<ABCChar> ();
		UIChar uiChar = newLetter.GetComponent<UIChar> ();

		switch(letterType)
		{
		case("normal"):
			abcChar.initializeFromInfo(lettersPool.getNextRandomized());
			break;
		case("obstacle"):
			abcChar.initializeFromInfo(obstaclesLettersPool.getNextRandomized());
			break;
		case("tutorial"):
			abcChar.initializeFromInfo(tutorialLettersPool.getNextRandomized());
			break;
		}

		uiChar.type = abcChar.type;
		uiChar.changeColorAndSetValues(abcChar.character.ToLower ());

		charactersOnGrid.Add(abcChar);

		return newLetter;
	}


	/*
	 * Se incrementa el puntaje del jugador
	 * 
	 * @params point{int}: La cantidad de puntos que se le entregara al jugador
	 */
	protected void addPoints(int amount)
	{
		pointsCount += amount;
		hud.setPoints (pointsCount);

		actualizePointsWinCondition ();

		if (!playerWon) 
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

	protected void substractMoves(int amount)
	{
		remainingMoves-=amount;
		hud.setMovements (remainingMoves);
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

	public void deleteWord()
	{
		wordManager.resetValidation(true);
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
					obstaclesUsed++;
				}
				
				if (myWinCondition [0] == "letters") 
				{
					for (int j = 0; j < goalLetters.Count; j++) 
					{
						if (goalLetters [j].ToLower () == wordManager.chars [i].character.ToLower () && !letterFound) 
						{
							print (goalLetters [j]);
							print ( wordManager.chars [i].character);
							hud.destroyLetterFound (goalLetters [j]);
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
					if(wordManager.getFullWord().ToLower() == goalWords[j].ToLower())
					{
						wordFound = true;
					}
				}
			}

			amount *= multiplierHelper;

			wordsMade++;
			actualizeWordsCompletedWinCondition ();

			substractMoves(1);
			addPoints(amount);

		}
		resetLettersSelected ();
	}

	protected void resetLettersSelected()
	{
		wordManager.resetValidation();

	}

	protected void sendVectorToCellManager(Vector3 vector3)
	{
		cellManager.getCellUnderPoint(vector3).clearCell();
	}

	protected void showPointsOfLettersSelected ()
	{
		int amount = 0;
		int multiplierHelper = 1;

		for (int i = 0; i < wordManager.chars.Count; i++) 
		{
			switch (wordManager.chars [i].pointsOrMultiple) 
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
					amount += int.Parse (wordManager.chars [i].pointsOrMultiple);}
				break;
			}
		}

		amount *= multiplierHelper;
		hud.setLettersPoints (amount);
	}

	public void linesCreated(int totalLines)
	{
		if(totalLines > 0)
		{
			Debug.Log(totalLines);
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
			hud.setLettersCondition (goalLetters);
		}
		else if(myWinCondition[0] == "obstacles")
		{
			quantity = obstaclesCount;
			hud.setObstaclesCondition (quantity);
		}
		else if(myWinCondition[0] == "word")
		{
			string[] text = myWinCondition [1].Split (',');

			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hud.setWordCondition (word);
		}
		else if(myWinCondition[0] == "sin")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hud.setSinCondition (word);
		}
		else if(myWinCondition[0] == "ant")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				goalWords.Add (text[i].Split('_')[0]);
			}
			word = goalWords [0];
			hud.setAntCondition (word);
		}
		else if(myWinCondition[0] == "points")
		{
			quantity = int.Parse (myWinCondition [1]);
			hud.setPointsCondition (quantity,pointsCount);
		}
		else if(myWinCondition[0] == "words")
		{
			quantity = int.Parse (myWinCondition [1]);
			hud.setWordsCondition (quantity,wordsMade);
		}

		//Se muestra el objetivo al inicio del nivel

		hud.showObjectivePopUp(myWinCondition[0],word,quantity,goalLetters);
	}

	protected void actualizePointsWinCondition ()
	{
		if(myWinCondition[0] == "points")
		{
			int quantity = 0;
			quantity = int.Parse (myWinCondition [1]);
			hud.setPointsCondition (quantity,pointsCount);
		}
	}

	protected void actualizeWordsCompletedWinCondition()
	{
		if(myWinCondition[0] == "words")
		{
			int quantity = 0;
			quantity = int.Parse (myWinCondition [1]);
			hud.setWordsCondition (quantity,wordsMade);
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
			if (goalLetters.Count == 0) 
			{
				win = true;
			}
			break;
		case "obstacles":
			if (obstaclesCount == obstaclesUsed) 
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
		case "ant":
			if (wordFound) 
			{
				win = true;
			}
			break;
		case "sin":
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
			playerWon = true;
			unlockPowerUp();
			winBonification ();
		}
		else
		{
			checkIfLoose ();
		}
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		if(!wordManager.checkIfAWordIsPossible(charactersOnGrid))
		{
			audioManager.PlayLoseAudio();
		}
	}

	public void checkIfLoose()
	{
		if(!cellManager.checkIfOneCanFit(pieceManager.getShowingPieces()) || remainingMoves == 0)
		{
			if(remainingMoves == 0)
			{
				allowGameInput(false);
			}

			Debug.Log ("No puede poner piezas");
			while(true)
			{
				bool pass = true;
				for(int i=0; i < charactersOnGrid.Count; i++)
				{
					if(!charactersOnGrid[i])
					{
						charactersOnGrid.RemoveAt(i);
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
		audioManager.PlayWonAudio();

		allowGameInput (false);

		//Se limpian las letras 
		resetLettersSelected ();

		add1x1Block();
	}

	protected void add1x1Block()
	{
		Cell[] emptyCells = cellManager.getAllEmptyCells();
		Cell cell;

		if (remainingMoves != 0 && emptyCells.Length > 0) 
		{
			cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];

			remainingMoves--;

			hud.setMovements (remainingMoves);

			GameObject go = GameObject.Instantiate (bonificationPiecePrefab) as GameObject;

			go.GetComponent<Piece> ().currentType = cellManager.colorRandom ();

			cellManager.occupyAndConfigureCell(cell,go,go.GetComponent<Piece> ().currentType,true);

			hud.showScoreTextAt(cell.transform.position,1);
			addPoints(1);

			StartCoroutine (add1x1BlockMore ());
		}
		else 
		{
			if(remainingMoves != 0)
			{
				addPoints (1);
				remainingMoves--;

				hud.setMovements (remainingMoves);
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
			cellManager.occupyAndConfigureCell (cellToLetter [random],getAndRegisterNewLetter("normal"),EPieceType.LETTER,true);
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
		int amount = 0;

		if(int.TryParse(cell.content.GetComponent<ABCChar>().pointsOrMultiple,out amount))
		{
			hud.showScoreTextAt(cell.transform.position,amount);
			addPoints(amount);
		}
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

		if(useGems(secondChancePrice))
		{
			secondChanceTimes++;

			remainingMoves += secondChanceMovements;
			hud.setMovements (remainingMoves);

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

		hud.activateSettings (activate);
	}

	public void closeObjectivePopUp()
	{
		audioManager.PlayButtonAudio();
		hud.hideObjectivePopUp ();
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

		hud.quitGamePopUp ();
	}
}
