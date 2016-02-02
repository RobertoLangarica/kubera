using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	public Text scoreText;

	public bool destroyByColor;

	public Text points;
	public GameObject MoneyGameObject;

	protected int pointsCount =0;

	public bool canRotate;

	protected PersistentData persistentData;
	protected WordManager wordManager;
	protected CellsManager cellManager;

	void Awake () 
	{
		persistentData = FindObjectOfType<PersistentData>();
		wordManager = FindObjectOfType<WordManager>();
		cellManager = FindObjectOfType<CellsManager>();

		addPoints (0);
	}

	void Update () 
	{
		/*if(Input.GetKeyDown(KeyCode.A))
		{
			gameManagerLose();
		}*/
	}

	/*
	 * Se incrementa el puntaje del jugador
	 * 
	 * @params point{int}: La cantidad de puntos que se le entregara al jugador
	 */
	public void addPoints(int point)
	{
		pointsCount += point;

		points.text = pointsCount.ToString();
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
				wordManager.chars[i].letterWasUsed();
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

			//FindObjectOfType<InputGameController>().checkToLoose();

			for(int i = 0;i < wordManager.chars.Count;i++)
			{
				ABCChar abcChar = wordManager.chars[i].gameObject.GetComponent<UIChar>().piece.GetComponent<ABCChar>();
				UIChar uiChar = wordManager.chars[i].gameObject.GetComponent<UIChar>().piece.GetComponent<UIChar>();

				if(uiChar != null && abcChar != null)
				{
					if(wordManager.words.completeWord)
					{
						cellManager.getCellOnVec(uiChar.gameObject.transform.position).clearCell();
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
	}
}
