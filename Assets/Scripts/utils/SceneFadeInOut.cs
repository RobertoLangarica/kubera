using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour {

	public static SceneFadeInOut instance;
	public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
	
	
	private bool sceneFading;      // Whether or not the scene is still fading in.
	//private bool colorStart = true;
	
	private Transform color;
	private Canvas mCanvas;
	void Awake ()
	{
		instance = this;

		color = transform.FindChild("Panel");
		// Set the texture so that it is the the size of the screen and covers it.
		//guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
		mCanvas = GetComponent<Canvas>();

	}
	
	void Update ()
	{
		// If the scene is starting...
		if(sceneFading)
		{
			StartFading();
		}
	}

	public void Fade()
	{
		color.GetComponent<Image>().color = new Color(0,0,0,1);
		//colorStart = false;
		sceneFading = true;
		mCanvas.sortingOrder=100;
	}
	
	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		color.GetComponent<Image>().color = Color.Lerp(color.GetComponent<Image>().color,Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	void StartFading ()
	{
		// Fade the texture to clear.
		FadeToClear();
		//Debug.Log(color.GetComponent<Image>().color.a);
		// If the texture is almost clear...

		if(color.GetComponent<Image>().color.a <= 0.05f)
		{
			// ... set the colour to clear and disable the GUITexture.
			color.GetComponent<Image>().color = Color.clear;
			
			// The scene is no longer starting.
			sceneFading = false;
			//colorStart = true;
			//mCanvas.sortingOrder=-1;
		}
	}
}