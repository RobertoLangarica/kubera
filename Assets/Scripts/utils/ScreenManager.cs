using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ScreenManager : Manager<ScreenManager> {

	public string firstScreenName;//Primer pantalla que se muestra en el juego
	public string firstEditorScreen;//Primer pantalla que se muestra en el editor
	public string screenBeforeClose;//Pantalla que no tiene back (cierra la app)

	[HideInInspector]public bool blocked = false;
	[HideInInspector]public bool backAllowed = true;

	public LoadingScene loading;
	public bool autoHideLoading = true;

	protected string waitScreen;
	protected Dictionary<string,string> backScreens;

	protected float timeBeforeNextScreen;
	protected AsyncOperation waitingScreen = null;
	protected int framesBeforeSwitch;

	public delegate void DOnFinishLoadScene();
	public DOnFinishLoadScene OnFinish;
	protected AsyncOperation async;

	public AsyncOperation preloadSceneAsync;


	void Start()
	{
		backScreens = new Dictionary<string, string>();
		#if UNITY_EDITOR
		GoToScene(firstEditorScreen);
		#else
		GoToScene(firstScreenName);
		#endif
	}
		
	void Update () 
	{
		if(waitingScreen != null)
		{
			timeBeforeNextScreen -= Time.deltaTime;

			if(timeBeforeNextScreen <= 0)
			{
				//AsyncOperation nos reporta 100% o isDone == true hasta que se permite activar
				if(waitingScreen.progress >= 0.9f && --framesBeforeSwitch < 0)
				{
					/*if(SceneFadeInOut.instance != null)
					{
						//SceneFadeInOut.instance.Fade();
					}*/
					
					waitingScreen.allowSceneActivation = true;
					waitingScreen = null;

					//No se pueden encimar estas 2 acciones
					if(blocked)
					{
						//Cambia de pantalla con delay
						StartCoroutine("waitForScreen");
					}
					else if(autoHideLoading && loading != null)
					{
						StartCoroutine("hideLoadingDelayed",2);
					}
				}
			}
		}

		#if UNITY_ANDROID
		//Back nativo de android
		if (Input.GetKey(KeyCode.Escape))
		{
			//showPrevScene();

			//HARDCODING
			if(blocked || !backAllowed){return;}


			if(SceneManager.GetActiveScene().name == screenBeforeClose)
			{
				Application.Quit();
			}
		}
		#endif

		/*if (Input.GetKeyUp(KeyCode.Escape))
		{
			showPrevScene();
		}*/

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

	public void GoToScene(string newScene,bool allowSameScreen = false)
	{
		if(blocked || waitingScreen != null || (!allowSameScreen && newScene == SceneManager.GetActiveScene().name))
		{
			return;
		}

		if(SceneFadeInOut.instance != null)
			SceneFadeInOut.instance.Fade();

		if(!backScreens.ContainsKey(newScene))
		{
			backScreens.Add(newScene,SceneManager.GetActiveScene().name);
		}

		SceneManager.LoadScene (newScene);
	}

	/*public void preLoadingScene(string level)
	{
		preloadSceneAsync = SceneManager.LoadSceneAsync(level);
		preloadSceneAsync.allowSceneActivation = false;
	}

	public void preLoadingContinue()
	{
		print (preloadSceneAsync);

		loading.showLoading(0.1f,()=>{preloadSceneAsync.allowSceneActivation = true;});
	}

	public IEnumerator loadScene(string level)
	{
		async = SceneManager.LoadSceneAsync(level,LoadSceneMode.Single);

		async.allowSceneActivation = false;

		yield return async.isDone;

		async.allowSceneActivation = true;
	}

	public void sceneFinishLoading(float speed = 0.3f)
	{
		if (loading != null) 
		{
			loading.hideLoading (speed);
		}
	}*/

	public void GoToSceneAsync(string newScene, bool allowSameScreen = false, float waitTime = -1, int waitFrames = 10)
	{	
		if(blocked || waitingScreen != null || (!allowSameScreen && newScene == SceneManager.GetActiveScene().name))
		{
			return;
		}

		if(!backScreens.ContainsKey(newScene))
		{
			backScreens.Add(newScene,SceneManager.GetActiveScene().name);
		}

		timeBeforeNextScreen = waitTime;
		framesBeforeSwitch = waitFrames;

		showLoading();

		waitingScreen = SceneManager.LoadSceneAsync(newScene);
		waitingScreen.allowSceneActivation = false;
	}

	public void GoToSceneDelayed(string newScene, float delay = 5)
	{
		blocked = true;
		waitScreen = newScene;

		showLoading();

		//Hay un nivel asincrono cargando?
		//No se pueden encimar las acciones
		if(waitingScreen == null)
		{
			StartCoroutine("waitForScreen",delay);
		}
	}

	IEnumerator waitForScreen(float delay = 1.5f)
	{
		yield return new WaitForSeconds(delay);
		blocked = false;
		GoToScene(waitScreen);

		if(autoHideLoading && loading != null)
		{
			StartCoroutine("hideLoadingDelayed",2);
		}
	}

	IEnumerator hideLoadingDelayed(int framesDelay = 1)
	{
		for(int i = 0; i < framesDelay; i++)
		{yield return new WaitForEndOfFrame();}

		loading.hideLoading(0);
	}

	IEnumerator hideLoadingDelayedByTime(float timeDelay = 1.0f)
	{
		yield return new WaitForSeconds(timeDelay);

		loading.hideLoading(0);
	}

	public void showLoading()
	{
		if(loading != null)
		{			
			loading.showLoading(0);
		}
	}

	/**
	 * Llamado externo para cuando no se usa el autoHideLoading
	 **/ 
	public void hideLoading(int framesDelay = 0)
	{
		if(loading != null)
		{
			if(framesDelay > 0)
			{
				StartCoroutine("hideLoadingDelayed",framesDelay);
			}
			else
			{
				loading.hideLoading (0);
			}
		}
	}

	public void hideLoading(float timeDelay = 0)
	{
		if(loading != null)
		{
			if(timeDelay > 0)
			{
				StartCoroutine("hideLoadingDelayedByTime",timeDelay);
			}
			else
			{
				loading.hideLoading (0);
			}
		}
	}
}
