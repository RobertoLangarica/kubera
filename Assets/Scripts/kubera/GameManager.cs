﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	[HideInInspector]
	public Levels data;

	public PopUp objectivePopUp;
	public PopUp winPopUp;

	protected int currLevel;

	public bool destroyByColor;

	public Text points;
	protected int pointsCount =0;

	// Use this for initialization
	void Awake () 
	{
		TextAsset tempTxt = (TextAsset)Resources.Load ("levels");
		data = Levels.LoadFromText(tempTxt.text);
		currLevel = 0;
		//Debug.Log(data.levels[0].pool);
		addPoints (0);
	}

	void Start()
	{
		objectivePopUp.redBDelegate += closePopUp;
		objectivePopUp.greenBDelegate += closePopUp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addPoints(int point)
	{
		pointsCount += point;

		points.text = pointsCount.ToString();
	}

	public void gameManagerLose()
	{

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
		ScreenManager.instance.GoToScene("Intro");
	}
	
	public void goToPopScene()
	{
		ScreenManager.instance.GoToScene("ObjectiveScene");
	}
}
