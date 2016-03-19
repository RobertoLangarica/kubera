using UnityEngine;
using System.Collections;

public class IntroScreen : MonoBehaviour {

	protected IntroUIObject[] objects;

	void Start()
	{
		objects = GetComponentsInChildren<IntroUIObject>();

		for(int i = 2; i < objects.Length; i++)
		{
			objects[i].INDelay += Random.Range(0.6f,1.0f);
		}

		for(int i = 0; i < objects.Length; i++)
		{
			objects[i].In();
		}
	}

	public void onPlay()
	{
		for(int i = 0; i < objects.Length; i++)
		{
			objects[i].Out();
		}

		Debug.Log ("Play");

		//ScreenManager.instance.GoToSceneAsync("ObjectiveScene",(objects[0].OUTDuration+objects[0].OUTDelay+0.15f));
		ScreenManager.instance.GoToScene("Game");
	}
}
