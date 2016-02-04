using UnityEngine;
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

	protected int pointsCount =0;
	protected int wordsMade =0;
	protected List<string> letters;
	protected int blackLettersUsed =0;

	protected string[] myWinCondition;

	protected int totalMoves;
	protected int currentMoves;

	public bool canRotate;
	public bool destroyByColor;

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
						abcChar.backToNormal();						
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
		uiChar.changeSpriteRendererTexture(wordManager.changeTexture (abcChar.character.ToLower ()));

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
		switch (myWinCondition[0]) {

		case "points":
			if(pointsCount >= int.Parse( myWinCondition[1]))
			{
				print ("win");
			}
			break;

		case "words":
			if (wordsMade >= int.Parse (myWinCondition [1])) 
			{
				print ("win");
			}
			break;
		case "letters":
			if (letters.Count == 0) 
			{
				print ("win");
			}
			break;
		case "blackLetters":
			if (blackLettersUsed >= int.Parse (myWinCondition [1])) 
			{
				print ("win");
			}
			break;
		default:
			break;
		}

		checkToLoose();
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
}
