using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalPopUp : PopUpBase {

	public Text goalText;
	public Text goalLettersText;
	public Text LevelNumber;
	public Text LevelText;
	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

	public GameObject[] stars;

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

	public void setGoalPopUpInfo(string text, int starsReached, List<string> letters = null, string levelName = "")
	{
		this.LevelNumber.text = levelName;
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
		
		showStars (starsReached);
	}

	protected void showStars(int starsReached)
	{		
		for(int i=0; i<starsReached; i++)
		{
			stars [i].SetActive (true);
		}
	}


	public void playGame()
	{
		OnComplete ("playGame");
	}

	public void exit ()
	{
		OnComplete ();
	}
		
}
