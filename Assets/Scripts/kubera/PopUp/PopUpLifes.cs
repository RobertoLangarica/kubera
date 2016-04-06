using UnityEngine;
using System.Collections;

public class PopUpLifes : PopUpBase {


	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void popUpCompleted()
	{
		popUp.SetActive (false);

		OnComplete ();
	}

	public void exit ()
	{
		popUpCompleted ();
	}

	public void askForLifes()
	{
		popUpCompleted ();
	}

	public void chargeLifes()
	{
		popUpCompleted ();
	}
}
