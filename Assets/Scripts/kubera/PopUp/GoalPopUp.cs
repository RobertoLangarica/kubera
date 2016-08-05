using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalPopUp : PopUpBase {

	public Text goalTextABA;
	public Text goalTextABB;
	public Text goalTextA;
	public Text goalTextLetters;

	public Text LevelNumber;
	public Text LevelText;
	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject ABObjective;
	public GameObject AObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

	public GameObject[] stars;

	protected int objective;
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

	public void setGoalPopUpInfo(string textA,string textB,int starsReached, List<string> letters = null, string levelName = "" , int aABLetterObjectives = 0)
	{
		this.LevelNumber.text = levelName;
		ABObjective.SetActive (false);
		AObjective.SetActive (false);
		lettersObjective.SetActive (false);
		resetStars ();


		objective = aABLetterObjectives;
		switch (objective) {
		case 0:
			goalTextABA.text = textA;
			goalTextABB.text = textB;
			ABObjective.SetActive (true);
			break;
		case 1:
			goalTextA.text = textA;
			AObjective.SetActive (true);
			break;
		case 2:
			goalTextLetters.text = textA;
			lettersObjective.SetActive (true);

			break;
		default:
			break;
		}
		if(letters.Count != 0)
		{
			destroyLetersOnContainer ();
			//goalText.enabled = false;
			lettersObjective.SetActive (true);
			//goalLettersText.text = text;
			foreach (object val in letters) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = val.ToString();
				letter.GetComponentInChildren<Text> ().text = val.ToString();
				letter.transform.SetParent (goalLettersContainer.transform,false);
			}
		}

		showStars (starsReached);
	}

	protected void destroyLetersOnContainer()
	{
		for(int i=0; i<goalLettersContainer.transform.childCount;)
		{
			DestroyImmediate (goalLettersContainer.transform.GetChild (i).gameObject);
		}
	}

	protected void showStars(int starsReached)
	{		
		for(int i=0; i<starsReached; i++)
		{
			stars [i].SetActive (true);
		}
	}

	protected void resetStars()
	{		
		for(int i=0; i<stars.Length; i++)
		{
			stars [i].SetActive (false);
		}
	}


	public void playGame()
	{
		OnComplete ("playGame");
	}

	public void exit ()
	{
		OnComplete ("closeRetry");
	}
		
}
