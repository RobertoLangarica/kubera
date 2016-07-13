using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalPopUp : PopUpBase {

	public Text goalText;
	public Text goalLettersText;
	public Text LevelName;
	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

	public GameObject[] stars;
	public float speedShowStars = 0.5f;

	void Start()
	{
		int maxSize = 5;
		if(((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-gridLayoutGroup.padding.left) < goalLettersContainer.GetComponent<RectTransform> ().rect.height *.8f)
		{
			gridLayoutGroup.cellSize = new Vector2((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-5
				,(goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-5);
		}
		else
		{
			gridLayoutGroup.cellSize = new Vector2(goalLettersContainer.GetComponent<RectTransform>().rect.height*.8f
				,goalLettersContainer.GetComponent<RectTransform>().rect.height*.8f);
		}
	}
	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void setGoalPopUpInfo(string text, int starsReached, List<string> letters = null, string levelName = "", bool animation = false)
	{
		this.LevelName.text = levelName;
		if(letters.Count != 0)
		{
			goalText.enabled = false;
			lettersObjective.SetActive (true);
			goalLettersText.text = text;
			foreach (object val in letters) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = val.ToString();
				letter.GetComponentInChildren<Text> ().text = val.ToString();
				letter.transform.SetParent (goalLettersContainer.transform,false);
			}
		}
		else
		{
			goalText.text = text;
		}
		print (starsReached);
		showStars (starsReached, animation);
	}

	protected void showStars(int starsReached, bool animation)
	{
		if(animation)
		{
			for(int i=0; i<stars.Length; i++)
			{
				stars [i].SetActive (false);
				stars [i].transform.localScale = new Vector3 (0, 0, 0);
			}
			showStarsByAnimation (starsReached);
		}
		else
		{
			for(int i=0; i<stars.Length; i++)
			{
				stars [i].SetActive (false);
			}
			for(int i=0; i<starsReached; i++)
			{
				stars [i].SetActive (true);
			}
		}
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
		OnComplete ("playGame");
	}

	public void exit ()
	{
		OnComplete ("closeObjective");
	}
		
}
