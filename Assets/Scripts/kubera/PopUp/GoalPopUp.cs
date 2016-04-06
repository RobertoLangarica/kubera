using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoalPopUp : PopUpBase {

	public Text goalText;

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void setGoalPopUpInfo(string text)
	{
		goalText.text = text;
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
