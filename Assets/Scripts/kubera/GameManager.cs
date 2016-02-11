﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	//Texto del PoUp
	public Text scoreText;

	public Text points;
	public Text movementsText;

	public GameObject MoneyGameObject;
	public GameObject bonificationPiece;

	protected int pointsCount =0;
	protected int wordsMade =0;
	protected List<string> letters;
	protected int blackLettersUsed =0;

	protected string[] myWinCondition;

	protected int totalMoves;
	protected int currentMoves;

	protected int winBombs;

	public bool canRotate;
	public bool destroyByColor;

	protected List<Cell> cellToLetter;

	protected EPOWERUPS activatedPowerUp;

	protected PersistentData persistentData;
	protected WordManager wordManager;
	protected CellsManager cellManager;
	protected PowerUpManager powerUpManager;
	protected InputGameController inputGameController;
	protected PieceManager pieceManager;

	void Awake () 
	{
		persistentData = FindObjectOfType<PersistentData>();
		wordManager = FindObjectOfType<WordManager>();
		cellManager = FindObjectOfType<CellsManager>();
		powerUpManager = FindObjectOfType<PowerUpManager> ();
		inputGameController = FindObjectOfType<InputGameController> ();
		pieceManager = FindObjectOfType<PieceManager>();

		cellManager.OnlinesCounted += linesCreated;
		cellManager.OnLetterCreated += registerNewLetterCreated;

		inputGameController.deactivateRotateMode += setRotationOfPieces;
		inputGameController.deactivateDestroyMode += setDestroyByColor;
		inputGameController.pointsAtPieceSetCorrectly += piecePositionatedCorrectly;
	}

	void Start()
	{
		myWinCondition = persistentData.currentLevel.winCondition.Split (new char[1]{ '-' });
		cellToLetter = new List<Cell> ();

		currentMoves = totalMoves = persistentData.currentLevel.moves;
		movementsText.text = currentMoves.ToString();

		if(myWinCondition[0] == "letters")
		{
			letters = new List<string> ();
			int i;
			string[] s = myWinCondition [1].Split (new char[1]{ ',' });
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
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.A))
		{
			winBonification();
		}
	}

	/*
	 * Se incrementa el puntaje del jugador
	 * 
	 * @params point{int}: La cantidad de puntos que se le entregara al jugador
	 */
	protected void addPoints(int point)
	{
		pointsCount += point;

		points.text = pointsCount.ToString();

		checkWinCondition ();
	}

	protected void piecePositionatedCorrectly(int length)
	{
		currentMoves--;

		movementsText.text = currentMoves.ToString();

		addPoints(length);
	}

	public void activeMoney(bool show,int howMany=0)
	{
		if(show)
		{
			MoneyGameObject.SetActive (true);
			if(MoneyGameObject.transform.FindChild("Charge") != null)
			{
				MoneyGameObject.transform.FindChild ("Charge").GetComponentInChildren<Text> ().text = "-"+howMany.ToString ();
			}
		}
		else
		{
			MoneyGameObject.SetActive(false);	
		}
	}

	public void verifyWord()
	{
		//FindObjectOfType<ShowNext>().ShowingNext(false);

		int amount = 0;
		int multiplierHelper = 1;
		bool letterFound;
		if(wordManager.words.completeWord)
		{
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
				for (int j = 0; j < letters.Count; j++) 
				{
					if (letters [j].ToLower() == wordManager.chars [i].character.ToLower() && !letterFound) 
					{
						letters.RemoveAt (j);
						letterFound = true;
					}
				}
			}

			amount *= multiplierHelper;
			addPoints(amount);
			wordsMade++;
			//FindObjectOfType<InputGameController>().checkToLoose();

		}

		for(int i = 0;i < wordManager.chars.Count;i++)
		{
			ABCChar abcChar = wordManager.chars[i].gameObject.GetComponent<UIChar>().piece.GetComponent<ABCChar>();
			UIChar uiChar = wordManager.chars [i].gameObject.GetComponent<UIChar> ();

			if(uiChar != null && abcChar != null)
			{
				if(wordManager.words.completeWord)
				{
					cellManager.getCellOnVec(uiChar.piece.transform.position).clearCell();
					uiChar.DestroyPiece();
				}
				else
				{
					if(abcChar.wildcard)
					{
						//GameObject.Find("WildCard").GetComponent<PowerUpBase>().returnPower();
					}
					else
					{
						uiChar.piece.GetComponent<UIChar>().backToNormal();						
					}

				}
			}
		}
		wordManager.resetValidation();
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
			}
			break;
		case(4):
			{
				addPoints(50);
			}
			break;
		case(5):
			{
				addPoints(75);
			}
			break;
		case(6):
			{
				addPoints(105);
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
		deactivateCurrentPowerUp();

		canRotate = true;
		inputGameController.setCanRotate (canRotate);
		activatedPowerUp = EPOWERUPS.ROTATE_POWERUP;
	}

	public void addWildCardInCurrentWord()
	{
		deactivateCurrentPowerUp();

		wordManager.addCharacter(".",gameObject);
		FindObjectOfType<ShowNext>().ShowingNext(true);
		activatedPowerUp = EPOWERUPS.WILDCARD_POWERUP;
	}

	public void createOneSquareBlock(Transform myButtonPosition)
	{
		deactivateCurrentPowerUp();

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.BLOCK_POWERUP).oneTilePower(myButtonPosition));
		activatedPowerUp = EPOWERUPS.BLOCK_POWERUP;
	}

	public void activateDestroyAColorPowerUp(Transform myButtonPosition)
	{
		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbours = false;

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP).activateDestroyMode(myButtonPosition));
		inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = EPOWERUPS.DESTROY_ALL_COLOR_POWERUP;
	}

	public void activateDestroyNeighborsOfSameColor(Transform myButtonPosition)
	{
		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbours = true;

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP).activateDestroyMode(myButtonPosition));
		inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = EPOWERUPS.DESTROY_NEIGHBORS_POWERUP;
	}

	protected void deactivateCurrentPowerUp()
	{
		if (canRotate) 
		{
			canRotate = false;
			inputGameController.setCanRotate (canRotate);
		}
		if(destroyByColor)
		{
			destroyByColor = false;
		}
	}

	protected void setDestroyByColor(bool activate)
	{
		destroyByColor = activate;
	}

	protected void setRotationOfPieces(bool activate)
	{
		canRotate = activate;
	}

	protected void checkWinCondition ()
	{
		bool win = false;
		switch (myWinCondition[0]) {
		case "points":
			if(pointsCount >= int.Parse( myWinCondition[1]))
			{
				print ("win");
				win = true;
			}
			break;

		case "words":
			if (wordsMade >= int.Parse (myWinCondition [1])) 
			{
				print ("win");
				win = true;
			}
			break;
		case "letters":
			if (letters.Count == 0) 
			{
				print ("win");
				win = true;
			}
			break;
		case "blackLetters":
			if (blackLettersUsed >= int.Parse (myWinCondition [1])) 
			{
				print ("win");
				win = true;
			}
			break;
		default:
			break;
		}

		if (win) 
		{
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
		if(!cellManager.VerifyPosibility(pieceManager.piecesInBar) || currentMoves == 0)
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
		setInput (false);

		//Se limpian las letras
		verifyWord();

		bombsForWinBonification();

		if(cellManager.colorOfMoreQuantity() != ETYPEOFPIECE_ID.NONE)
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

		if (currentMoves != 0) {
			cell = emptyCells [Random.Range (0, emptyCells.Length - 1)];


			currentMoves--;
			movementsText.text = currentMoves.ToString();

			GameObject go = GameObject.Instantiate (bonificationPiece) as GameObject;
			SpriteRenderer sprite = go.GetComponent<Piece> ().pieces [0].GetComponent<SpriteRenderer> ();

			Vector3 nVec = new Vector3 (sprite.bounds.size.x * 0.5f,
				               -sprite.bounds.size.x * 0.5f, 0) + cell.transform.position;

			go.transform.position = nVec;

			//Debug.Log(cellManager.colorOfMoreQuantity());
			go.GetComponent<Piece> ().colorToSet = 0;
			go.GetComponent<Piece> ().typeOfPiece = cellManager.colorOfMoreQuantity ();

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
			cellToLetter [random].typeOfPiece = ETYPEOFPIECE_ID.LETTER;
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
		if(winBombs > 0 && cellManager.colorOfMoreQuantity() != ETYPEOFPIECE_ID.NONE)
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
		cellToLetter.AddRange (cellManager.searchCellsOfSameColor (ETYPEOFPIECE_ID.LETTER));
		cellToLetter.AddRange (cellManager.searchCellsOfSameColor (ETYPEOFPIECE_ID.LETTER_FROM_BEGINING));
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
		addPoints(amount);
	}

	protected void setInput(bool active)
	{
		if (active) 
		{
			GameObject.Find ("FingerGestures").SetActive (true);
			for (int i = 0; i < GameObject.Find ("PanelPowerUp").transform.childCount; i++) 
			{
				GameObject.Find ("PanelPowerUp").transform.GetChild(i).GetComponent<Button> ().interactable = true;
			}
		}
		else 
		{
			GameObject.Find ("FingerGestures").SetActive (false);
			for (int i = 0; i < GameObject.Find ("PanelPowerUp").transform.childCount; i++) 
			{
				GameObject.Find ("PanelPowerUp").transform.GetChild(i).GetComponent<Button> ().interactable = false;
			}
		}
	}

	protected void bombsForWinBonification()
	{
		if(currentMoves > 0 && currentMoves < 4)
		{
			winBombs = 1;
		}
		else if(currentMoves > 3 && currentMoves < 7)
		{
			winBombs = 2;
		}
		else if(currentMoves > 6 && currentMoves < 10)
		{
			winBombs = 3;
		}
		else if(currentMoves > 10)
		{
			winBombs = 5;
		}
	}
}
