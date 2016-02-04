using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;

public class GameManager : MonoBehaviour 
{
	//Texto del PoUp
	public Text scoreText;

	public Text points;
	public Text movementsText;

	public GameObject MoneyGameObject;
	public GameObject bonificationPiece;

	protected int pointsCount = 0;
	protected int wordsMade = 0;
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
		myWinCondition = persistentData.currentLevel.winCondition.Split (new char[1]{ '_' });
		print (myWinCondition[0] +" " +myWinCondition[1]);

		currentMoves = totalMoves = persistentData.currentLevel.moves;
		movementsText.text = currentMoves.ToString();
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

		if(wordManager.words.completeWord)
		{
			for(int i = 0;i < wordManager.chars.Count;i++)
			{
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

	public void createOneSquareBlock()
	{
		deactivateCurrentPowerUp();

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.BLOCK_POWERUP).oneTilePower());
		activatedPowerUp = EPOWERUPS.BLOCK_POWERUP;
	}

	public void activateDestroyAColorPowerUp()
	{
		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbours = false;

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_ALL_COLOR_POWERUP).activateDestroyMode());
		inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = EPOWERUPS.DESTROY_ALL_COLOR_POWERUP;
	}

	public void activateDestroyNeighborsOfSameColor()
	{
		deactivateCurrentPowerUp();

		destroyByColor = true;
		cellManager.selectNeighbours = true;

		inputGameController.activePowerUp (powerUpManager.getPowerUp(EPOWERUPS.DESTROY_NEIGHBORS_POWERUP).activateDestroyMode());
		inputGameController.setDestroyByColor (destroyByColor);
		activatedPowerUp = EPOWERUPS.DESTROY_NEIGHBORS_POWERUP;
	}

	protected void deactivateCurrentPowerUp()
	{
		if (canRotate) 
		{
			canRotate = false;
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

	protected void winBonification()
	{
		//Se limpian las letras
		verifyWord();

		add1x1Block();
	}

	protected void add1x1Block()
	{
		Cell[] emptyCells = cellManager.allEmptyCells();
		Cell cell;


		if(currentMoves != 0)
		{
			cell = emptyCells[Random.Range(0,emptyCells.Length-1)];
			currentMoves--;

			GameObject go = GameObject.Instantiate(bonificationPiece) as GameObject;
			SpriteRenderer sprite = go.GetComponent<Piece>().pieces[0].GetComponent<SpriteRenderer>();

			Vector3 nVec = new Vector3(sprite.bounds.size.x*0.5f,
				-sprite.bounds.size.x*0.5f,0) + cell.transform.position;

			go.transform.position = nVec;

			go.transform.position = cellManager.Positionate(go.GetComponent<Piece>());

			Debug.Log(cellManager.colorOfMoreQuantity());
			go.GetComponent<Piece>().typeOfPiece = cellManager.colorOfMoreQuantity();

			add1x1Block();
		}
	}
}
