using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour {
	public static ScreenManager instance;

	public string firstScreenName;//Primer pantalla que se muestra en el juego
	public string firstEditorScreen;//Primer pantalla que se muestra en el editor
	public string screenBeforeClose;//Pantalla que no tiene back (cierra la app)
	public AudioSource music;

	[HideInInspector]
	public bool blocked = false;
	[HideInInspector]
	public bool backAllowed = true;

	protected float waitTime;
	protected string waitScreen;
	private bool destroyed = false;//Indica si el objeto ya se destruyo

	protected bool isAudioPlaying = true;
	protected Dictionary<string,string> backScreens;

	protected float timeBeforeNextScreen;
	protected AsyncOperation waitingScreen = null;
	protected int framesBeforeSwitch;

	void Awake()
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag ("screenManager");
		for(int i=1; i< go.Length; i++)
		{
			DestroyImmediate (go [i]);
		}
		//No se si al mandar destroyed en el awake llegue entrar a start pero no corremos riesgos
		if (go.Length > 1) 
		{
			destroyed = true;
			return;
		}

		DontDestroyOnLoad(this);
		instance = this;

		backScreens = new Dictionary<string, string>();
	
		transform.SetAsLastSibling(); 
	}

	void Start()
	{
		if(destroyed)
		{
			return;
		}

		//Cliente
		#if UNITY_EDITOR
		GoToScene(firstEditorScreen);
		#else
		GoToScene(firstScreenName);
		#endif
	}

	// Update is called once per frame
	void Update () {

		if(destroyed)
		{
			return;
		}

		#if UNITY_ANDROID
		//Back nativo de android
		if (Input.GetKey(KeyCode.Escape))
		{
			showPrevScene();
		}
		#endif

		if (Input.GetKeyUp(KeyCode.Escape))
		{
			showPrevScene();
		}


		if(waitingScreen != null)
		{
			timeBeforeNextScreen -= Time.deltaTime;

			if(timeBeforeNextScreen <= 0)
			{
				//AsyncOperation no reporta 100% o isDone == true hasta que se permite activar
				if(waitingScreen.progress >= 0.9f && --framesBeforeSwitch < 0)
				{
					if(SceneFadeInOut.instance != null)
					{
						//SceneFadeInOut.instance.Fade();
					}
					
					waitingScreen.allowSceneActivation = true;
					waitingScreen = null;

					//No se pueden encimar estas 2 acciones
					if(blocked)
					{
						//Cambia de pantalla con delay
						StartCoroutine("waitForScreen");
					}
				}
			}
		}
	}
	
	public void showPrevScene()	
	{
		if(blocked || !backAllowed){return;}


		if(SceneManager.GetActiveScene().name == screenBeforeClose)
		{
			Application.Quit();
		}
		else
		{
			//Mostramos la pantalla anterior
			if(backScreens.ContainsKey(SceneManager.GetActiveScene().name))
			{
				string name = SceneManager.GetActiveScene().name;
				GoToScene(backScreens[name]);
				backScreens.Remove(name);
			}
		}
	}

	public void GoToScene(string newScene)
	{
		if(blocked || waitingScreen != null || newScene == SceneManager.GetActiveScene().name)
		{
			return;
		}

		if (!isAudioPlaying && (newScene == firstEditorScreen || newScene == firstScreenName) ) 
		{
			isAudioPlaying = true;
			//music.Play();
		}

		if(SceneFadeInOut.instance != null)
			SceneFadeInOut.instance.Fade();

		if(!backScreens.ContainsKey(newScene))
		{
			backScreens.Add(newScene,SceneManager.GetActiveScene().name);
		}

		SceneManager.LoadScene (newScene);
	}


	public void GoToSceneAsync(string newScene,float waitTime = -1, int waitFrames = 10)
	{	
		if(blocked){return;}

		if(newScene == SceneManager.GetActiveScene().name)
		{
			return;
		}

		if (!isAudioPlaying && (newScene == firstEditorScreen || newScene == firstScreenName) ) 
		{
			isAudioPlaying = true;
		}

		if(!backScreens.ContainsKey(newScene))
		{
			backScreens.Add(newScene,SceneManager.GetActiveScene().name);
		}

		timeBeforeNextScreen = waitTime;
		framesBeforeSwitch = waitFrames;

		waitingScreen = SceneManager.LoadSceneAsync(newScene);
		waitingScreen.allowSceneActivation = false;
	}

	public void GoToSceneDelayed(string newScene, float delay = 5)
	{
		blocked = true;
		waitScreen = newScene;
		waitTime = delay;

		//Hay un nivel asincrono cargando?
		//No se pueden encimar las acciones
		if(waitingScreen == null)
		{
			StartCoroutine("waitForScreen");
		}
	}

	IEnumerator waitForScreen()
	{
		yield return new WaitForSeconds(1.5f);
		blocked = false;
		GoToScene(waitScreen);
	}
	
}
