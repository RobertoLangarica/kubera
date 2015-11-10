using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour {

	public void onPlay()
	{
		Debug.Log ("Play");
		ScreenManager.instance.GoToScene("ObjectiveScene");
	}
}
