using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{

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
	protected List<string> words = new List<string>();
	protected int obstaclesQuantity = 0;
	protected int obstaclesUsed = 0;

	protected string[] myWinCondition;

	protected int totalMoves;
	protected int remainingMoves;

	public int secondChanceMovements = 5;
	public int secondChanceBombs = 2;
	protected int secondChanceTimes = 0;
	protected int bombsUsed = 0;

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
	protected List<GameObject> XMLPoolPiecesList = new List<GameObject>();
	protected List<ScriptableABCChar> XMLPoolLetersList = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> XMLPoolBlackLetersList = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> XMLPoolTutorialLetersList = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> randomizedPoolLeters = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> randomizedBlackPoolLeters = new List<ScriptableABCChar>();
	protected List<ScriptableABCChar> randomizedTutorialPoolLeters = new List<ScriptableABCChar>();
	protected List<ABCChar> listChar = new List<ABCChar>();

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
		fillPiecesPool ();

		pieceManager.initializeShowingPieces ();
		hud.showPieces (pieceManager.getShowingPieceList ());

		cellToLetter = new List<Cell> ();

		myWinCondition = currentLevel.winCondition.Split ('-');
		remainingMoves = totalMoves = currentLevel.moves;

		hud.setMovments (remainingMoves);
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
		string[] lettersPool = currentLevel.lettersPool.Split(',');
		string[] piecesInfo;

		/*Aqui diseccionar el XML****************/
		int amount = 0;
		ScriptableABCChar newLetter = null;

		if(XMLPoolLetersList.Count == 0)
		{
			/**
			 * CSV
			 * cantidad_letra_puntos/multiplo_tipo
			 * ej. 02_A_1_1
			 **/ 
			for(int i =0; i<lettersPool.Length; i++)
			{
				piecesInfo = lettersPool[i].Split('_');
				amount = int.Parse(piecesInfo[0]);

				for(int j = 0;j < amount;j++)
				{
					newLetter = new ScriptableABCChar();
					newLetter.character = piecesInfo[1];
					newLetter.pointsOrMultiple = piecesInfo[2];
					newLetter.type = int.Parse(piecesInfo[3]);

					XMLPoolLetersList.Add(newLetter);
				}
			}

			if(currentLevel.obstacleLettersPool.Length > 0)
			{
				lettersPool = currentLevel.obstacleLettersPool.Split(',');

				for(int i =0; i<lettersPool.Length; i++)
				{
					piecesInfo = lettersPool[i].Split('_');
					amount = int.Parse(piecesInfo[0]);
					for(int j = 0;j < amount;j++)
					{
						newLetter = new ScriptableABCChar();
						newLetter.character = piecesInfo[1];
						newLetter.pointsOrMultiple = piecesInfo[2];
						newLetter.type = int.Parse(piecesInfo[3]);
						XMLPoolBlackLetersList.Add(newLetter);
					}
				}
			}
			if(currentLevel.tutorialLettersPool.Length > 1)
			{
				Debug.Log(currentLevel.tutorialLettersPool.Length);
				string[] tutorialInfo = currentLevel.tutorialLettersPool.Split('-');
				lettersPool = tutorialInfo[0].Split(',');

				for(int i =0; i<lettersPool.Length; i++)
				{
					piecesInfo = lettersPool[i].Split('_');
					amount = int.Parse(piecesInfo[0]);
					for(int j = 0;j < amount;j++)
					{
						newLetter = new ScriptableABCChar();
						newLetter.character = piecesInfo[1];
						newLetter.pointsOrMultiple = piecesInfo[2];
						newLetter.type = int.Parse(piecesInfo[3]);
						XMLPoolTutorialLetersList.Add(newLetter);
					}
				}
			}
		}
		/*****/

		randomizedPoolLeters = randomizeList<ScriptableABCChar>(XMLPoolLetersList);
		randomizedBlackPoolLeters = randomizeList<ScriptableABCChar>(XMLPoolBlackLetersList);
	}

	protected List<T> randomizeList<T>(List<T> target)
	{
		List<T> result = new List<T>();
		List<T> temporal = new List<T>(target);
		int index;

		while(temporal.Count > 0)
		{
			index = Random.Range(0,temporal.Count);
			result.Add(temporal[index]);
			temporal.RemoveAt(index);
		}

		return result;
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


		for(int i=0; i< tempCell.Count; i++)
		{ 
			Vector3 cellPosition =  tempCell[i].transform.position + (new Vector3 (tempCell[i].GetComponent<SpriteRenderer> ().bounds.extents.x,
				-tempCell[i].GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

			cellManager.occupyAndConfigureCell (tempCell [i], piece.pieces [i], piece.currentType);

			tempCell [i].content.transform.DOMove (cellPosition, 0.5f);

			if(tempCell.Count == i+1)
			{
				StartCoroutine(allPiecesAreOnGrid(piece));
			}

		}

		for(int i = 0;i < piece.pieces.Length;i++)
		{
			piece.pieces[i].transform.SetParent(piece.transform.parent);
		}

		audioManager.PlayPiecePositionatedAudio();
	}

	IEnumerator allPiecesAreOnGrid(Piece piece)
	{
		yield return new WaitForSeconds (0.5f);
		if (!piece.powerUp) 
		{
			pieceManager.removeFromListPieceUsed (piece.gameObject);

			if (pieceManager.getShowingPieceList ().Count == 0) 
			{
				pieceManager.initializeShowingPieces ();
				hud.showPieces (pieceManager.getShowingPieceList ());
			}

			//damos puntos por las piezas en la pieza
			addPointsActions (piece.pieces.Length);
			hud.showScoreTextAt(piece.transform.position,piece.pieces.Length);
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
				obstaclesQuantity++;
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
		GameObject go = GameObject.Instantiate (singleSquarePiece) as GameObject;
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
		GameObject go = Instantiate (uiLetter)as GameObject;
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
			abcChar.initializeFromScriptableABCChar(giveLetterInfo());
			break;
		case("obstacle"):
			abcChar.initializeFromScriptableABCChar(giveBlackLetterInfo());
			break;
		case("tutorial"):
			abcChar.initializeFromScriptableABCChar(giveTutorialLetterInfo());
			break;
		}

		uiChar.type = abcChar.type;
		uiChar.changeColorAndSetValues(abcChar.character.ToLower ());

		listChar.Add(abcChar);

		return newLetter;
	}


	/*
	 * Se incrementa el puntaje del jugador
	 * 
	 * @params point{int}: La cantidad de puntos que se le entregara al jugador
	 */
	protected void addPoints(int point)
	{
		pointsCount += point;

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
					for (int j = 0; j < letters.Count; j++) 
					{
						if (letters [j].ToLower () == wordManager.chars [i].character.ToLower () && !letterFound) 
						{
							print (letters [j]);
							print ( wordManager.chars [i].character);
							hud.destroyLetterFound (letters [j]);
							letters.RemoveAt (j);
							letterFound = true;
						}
					}
				}
			}

			if (myWinCondition [0] == "word" || myWinCondition [0] == "ant"|| myWinCondition [0] == "sin") 
			{
				for (int j = 0; j < words.Count; j++) 
				{
					if(wordManager.getFullWord().ToLower() == words[j].ToLower())
					{
						wordFound = true;
					}
				}
			}

			amount *= multiplierHelper;

			wordsMade++;
			actualizeWordsCompletedWinCondition ();
			addPointsActions (amount);
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
			hud.setLettersCondition (letters);
		}
		else if(myWinCondition[0] == "obstacles")
		{
			quantity = obstaclesQuantity;
			hud.setObstaclesCondition (quantity);
		}
		else if(myWinCondition[0] == "word")
		{
			string[] text = myWinCondition [1].Split (',');

			for(int i=0; i<text.Length; i++)
			{
				words.Add (text[i].Split('_')[0]);
			}
			word = words [0];
			hud.setWordCondition (word);
		}
		else if(myWinCondition[0] == "sin")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				words.Add (text[i].Split('_')[0]);
			}
			word = words [0];
			hud.setSinCondition (word);
		}
		else if(myWinCondition[0] == "ant")
		{
			string[] text = myWinCondition [1].Split (',');
			for(int i=0; i<text.Length; i++)
			{
				words.Add (text[i].Split('_')[0]);
			}
			word = words [0];
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

		hud.showObjectivePopUp(myWinCondition [0],word,quantity,letters);
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
			if (letters.Count == 0) 
			{
				win = true;
			}
			break;
		case "obstacles":
			if (obstaclesQuantity == obstaclesUsed) 
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
			checkToLoose ();
		}
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		if(!wordManager.checkIfAWordisPossible(listChar))
		{
			audioManager.PlayLoseAudio();
		}
	}

	public void checkToLoose()
	{
		if(!cellManager.checkIfOneCanFit(pieceManager.getShowingPieceList()) || remainingMoves == 0)
		{
			if(remainingMoves == 0)
			{
				allowGameInput(false);
			}

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

			hud.setMovments (remainingMoves);

			GameObject go = GameObject.Instantiate (bonificationPiece) as GameObject;

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

				hud.setMovments (remainingMoves);
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

	protected void fillPiecesPool()
	{
		string[] piecesInfo;
		int amout = 0;

		string[] myPieces = currentLevel.pieces.Split(new char[1]{','});

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

	protected void fillTutorialLettersRandomPool()
	{
		List<ScriptableABCChar> tempList = new List<ScriptableABCChar> ();
		randomizedTutorialPoolLeters = new List<ScriptableABCChar>();

		tempList = new List<ScriptableABCChar>(XMLPoolTutorialLetersList);


		while(tempList.Count >0)
		{
			int val = Random.Range(0,tempList.Count);
			randomizedTutorialPoolLeters.Add(tempList[val]);
			tempList.RemoveAt(val);
		}
	}

	public ScriptableABCChar giveLetterInfo()
	{
		if(randomizedPoolLeters.Count==0)
		{
			randomizedPoolLeters = randomizeList<ScriptableABCChar>(XMLPoolLetersList);
		}

		ScriptableABCChar letter = randomizedPoolLeters[0];
		randomizedPoolLeters.RemoveAt (0);

		return letter;
	}

	public ScriptableABCChar giveBlackLetterInfo()
	{
		if(randomizedBlackPoolLeters.Count==0)
		{
			randomizedBlackPoolLeters = randomizeList<ScriptableABCChar>(XMLPoolBlackLetersList);
		}
		ScriptableABCChar letter = randomizedBlackPoolLeters[0];
		randomizedBlackPoolLeters.RemoveAt (0);

		return letter;
	}

	public ScriptableABCChar giveTutorialLetterInfo()
	{
		if(randomizedTutorialPoolLeters.Count==0)
		{
			randomizedTutorialPoolLeters = randomizeList<ScriptableABCChar>(XMLPoolTutorialLetersList);
		}
		ScriptableABCChar letter = randomizedTutorialPoolLeters[0];
		randomizedTutorialPoolLeters.RemoveAt (0);

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
