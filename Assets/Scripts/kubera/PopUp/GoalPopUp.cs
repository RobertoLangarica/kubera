using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GoalPopUp : PopUpBase {

	public Text goalText;
	public Text goalLettersText;
	public Transform goalLettersContainer;

	public GameObject lettersObjective;
	public GameObject uiLetter;
	public GridLayoutGroup gridLayoutGroup;

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

	public void setGoalPopUpInfo(string text, List<string> letters = null)
	{
		if(letters != null)
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
	}

	protected void popUpCompleted()
	{
		popUp.SetActive (false);

		OnComplete ();
	}

	public void exit ()
	{
		popUpCompleted ();
	}
		
}
