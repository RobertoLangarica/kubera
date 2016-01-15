using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	public PopUp objectivePopUp;
	public PopUp winPopUp;
	public Text scoreText;

	[HideInInspector]
	public Level currentLevel;

	public bool destroyByColor;

	public Text points;
	protected int pointsCount =0;
	public bool canRotate;

	protected PersistentData persistentData;

	void Awake () 
	{
		persistentData = FindObjectOfType<PersistentData>();
		
		currentLevel = persistentData.data.levels[persistentData.currentLevel];

		addPoints (0);
	}

	void Start()
	{
		//Se asignan delegates a los botones de los PopUps
		objectivePopUp.redBDelegate += closePopUp;
		objectivePopUp.greenBDelegate += closePopUp;

		winPopUp.redBDelegate += goToIntro;
		winPopUp.greenBDelegate += goToPopScene;
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

	/*
	 * Se activa el PopUp de cuando ha perdido
	 * 
	 */
	public void gameManagerLose()
	{
		scoreText.text = points.text;
		winPopUp.showUp();
	}

	public void showObjective()
	{
		objectivePopUp.showUp();
	}

	public void closePopUp()
	{
		objectivePopUp.closePopUp();
	}
	
	public void goToIntro()
	{
		//ScreenManager.instance.GoToScene("Intro");
		winPopUp.closePopUp();
		ScreenManager.instance.GoToSceneAsync("Intro",0.5f);
	}
	
	public void goToPopScene()
	{
		//ScreenManager.instance.GoToScene("ObjectiveScene");
		winPopUp.closePopUp();
		ScreenManager.instance.GoToSceneAsync("ObjectiveScene",0.5f);
	}
}
