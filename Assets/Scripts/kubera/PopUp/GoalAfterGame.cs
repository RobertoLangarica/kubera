using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalAfterGame : PopUpBase {

	public Text LevelName;

	public Text PointsText;
	public Text Points;

	public GameObject[] stars;
	public float speedShowStars = 0.5f;

	void Start()
	{

	}

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void setGoalPopUpInfo(int starsReached, string levelName, string points)
	{
		this.LevelName.text = levelName;
		this.Points.text = points;

		showStars (starsReached);
	}

	protected void showStars(int starsReached)
	{
		for(int i=0; i<stars.Length; i++)
		{
			stars [i].SetActive (false);
			stars [i].transform.localScale = new Vector3 (0, 0, 0);
		}
		showStarsByAnimation (starsReached);
	}

	protected void showStarsByAnimation(int starsReached)
	{
		if(starsReached >=1)
		{
			stars [0].SetActive (true);
			stars [0].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
				if (starsReached >= 2) {
					stars [1].SetActive (true);
					stars [1].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
						if (starsReached == 3) {
							stars [2].SetActive (true);
							stars [2].transform.DOScale (new Vector2(1,1),speedShowStars).OnComplete (() => {
								print ("Termino de mostrar estrellas");
							});
						}
					});
				}
			});
		}
	}

	public void playGame()
	{
		OnComplete ("continue");
	}

	public void exit ()
	{
		OnComplete ("closeObjective");
	}
}
