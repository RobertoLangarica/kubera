using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;
using Kubera.Data.Sync;
using utils.gems;
using utils.gems.sync;

public class HomeManager : MonoBehaviour 
{
	public bool DirectlyToPlayOnTheFirstTime = true;

	public SettingsButton settingButtons;

	protected float speed = .33f;

	public Text playText;
	public GameObject block;

	public GameObject modal;

	public PopUpManager popUpManager;

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

		if(ShopikaSyncManager.GetCastedInstance<ShopikaSyncManager>().isGettingData)
		{
			activatePopUp ("shopikaConnect");
		}
		if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().isGettingData)
		{
			activatePopUp ("facebookLoadingConnect");
		}

		popUpManager.OnPopUpCompleted += closePopUp;
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
		activatePopUp ("shopikaPopUp");
	}
		
	protected void activatePopUp(string name)
	{
		modal.SetActive (true);
		popUpManager.activatePopUp (name);
	}

	protected void closePopUp(string action ="")
	{
		if(popUpManager.openPopUps.Count == 0)
		{
			modal.SetActive (false);
		}
	}

	public void activateFacebook()
	{
		//print (KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().facebookProvider.isLoggedIn);
		if(!KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
		{
			activatePopUp ("facebookLoadingConnect");
		}
	}


	public void activateShopika()
	{
		if (ShopikaManager.GetCastedInstance<ShopikaManager> ().currentUserId == ShopikaManager.GetCastedInstance<ShopikaManager> ().ANONYMOUS_USER) 
		{
			activatePopUp ("shopikaConnect");
		}
	}
}
