using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;

public class HomeManager : MonoBehaviour 
{
	public bool DirectlyToPlayOnTheFirstTime = true;

	public SettingsButton settingButtons;

	protected float speed = .33f;

	public Text playText;
	public GameObject block;
	public GameObject shopikaPopUp;
	public GameObject connectingFacebookPopUp;
	public GameObject connectingShopikaPopUp;

	void Start()
	{
		if(!DirectlyToPlayOnTheFirstTime)
		{
			Debug.Log("<color=red>Modo test: Se desactivo el poder ir directamente a jugar en el primer uso del juego.</color>");	
		}

		settingButtons.OnActivateMusic += activateMusic;

		playText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance ().Stop ("gamePlay",false);
			if(!AudioManager.GetInstance().IsPlaying("menuMusic"))
			{
				AudioManager.GetInstance ().Play ("menuMusic");
			}
		}

		Invoke ("startScene",0.3f);
	}

	void startScene()
	{
		ScreenManager.GetInstance().sceneFinishLoading ();
	}

	public void goToScene(string scene)
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}

		ScreenManager.GetInstance().GoToScene (scene);
	}

	public void goToPlay()
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}

		if(DataManagerKubera.GetCastedInstance<DataManagerKubera> ().currentUser.levels.Count != 0)
		{
			ScreenManager.GetInstance().GoToScene ("Levels");
		}
		else
		{
			PersistentData.GetInstance ().fromLevelsToGame = true;
			PersistentData.GetInstance ().currentLevel = PersistentData.GetInstance ().getFirstLevel ();

			if(!DirectlyToPlayOnTheFirstTime)
			{
				ScreenManager.GetInstance().GoToScene ("Levels");	
			}
			else
			{
				ScreenManager.GetInstance().GoToScene ("Game");	
			}


		}
	}
		
	public void ereaseData()
	{
		DataManagerKubera.GetInstance ().deleteData ();
		if(AudioManager.GetInstance())
		{
			
			AudioManager.GetInstance().Play("fxButton");
		}
	}

	public void activateSettings()
	{
		settingButtons.activeSettings ();
	}

	public void activateMusic(bool activate)
	{
		if(activate)
		{
			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance ().Stop ("gamePlay",false);
				if(!AudioManager.GetInstance().IsPlaying("menuMusic"))
				{
					AudioManager.GetInstance ().Play ("menuMusic");
				}
			}
		}
	}

	public void activateShopikaPopUp()
	{
		shopikaPopUp.SetActive (true);
	}
}
