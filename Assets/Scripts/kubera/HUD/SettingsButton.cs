using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsButton : MonoBehaviour 
{
	public Sprite[] musicImages;
	public Sprite[] soundsImages;
	public Image music;
	public Image fx;

	void Start()
	{
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance ().Play ("gamePlay");
		}

		setStateMusic ();
		setStateSounds ();
	}

	public void activateMusic()
	{
		if (AudioManager.GetInstance ()) 
		{
			//AudioManager.GetInstance ().Play ("fxButton");

			if (AudioManager.GetInstance ().activateMusic) 
			{
				AudioManager.GetInstance ().activateMusic = false;
				UserDataManager.instance.isMusicActive = false;

				AudioManager.GetInstance ().Stop ("gamePlay");
			}
			else 
			{
				AudioManager.GetInstance ().activateMusic = true;
				UserDataManager.instance.isMusicActive = true;

				AudioManager.GetInstance ().Play ("gamePlay");
			}
			setStateMusic ();
		}
	}

	public void activateSounds()
	{
		if(AudioManager.GetInstance())
		{
			//AudioManager.GetInstance().Play("fxButton");
			if(AudioManager.GetInstance().activateSounds)
			{
				AudioManager.GetInstance().activateSounds = false;
				UserDataManager.instance.isSoundEffectsActive = false;

				AudioManager.GetInstance ().StopAllAudiosInCategory ("LOOP FX");
				AudioManager.GetInstance ().StopAllAudiosInCategory ("FX");
			}
			else
			{
				AudioManager.GetInstance().activateSounds = true;
				UserDataManager.instance.isSoundEffectsActive = true;
			}
			setStateSounds ();
		}
	}

	public void setStateMusic()
	{
		if (!AudioManager.GetInstance ()) {
			return;
		}
		if (AudioManager.GetInstance().activateMusic) 
		{
			music.sprite = musicImages [0];
		}
		else
		{
			music.sprite = musicImages [1];
		}
	}

	public void setStateSounds()
	{
		if (!AudioManager.GetInstance ()) {
			return;
		}
		if (AudioManager.GetInstance().activateSounds) 
		{
			fx.sprite = soundsImages [0];
		}
		else
		{
			fx.sprite = soundsImages [1];
		}
	}
}
