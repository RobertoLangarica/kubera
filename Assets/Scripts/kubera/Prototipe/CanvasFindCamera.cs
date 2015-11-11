using UnityEngine;
using System.Collections;

public class CanvasFindCamera : MonoBehaviour {

	void OnLevelWasLoaded() {
		if(gameObject.GetComponent<Canvas> ().renderMode != RenderMode.ScreenSpaceCamera)
		{
			gameObject.GetComponent<Canvas> ().renderMode = RenderMode.ScreenSpaceCamera;
		}
		gameObject.GetComponent<Canvas> ().worldCamera = Camera.main;
		
	}
}
