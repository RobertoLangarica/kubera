﻿using UnityEngine;
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
		setStateMusic ();
		setStateSounds ();
	}

	public void activateMusic()
	{
		if (AudioManager.GetInstance ()) 
		{
			//AudioManager.GetInstance ().Play ("fxButton");
			setStateMusic ();

			if (AudioManager.GetInstance ().activateMusic) 
			{
				AudioManager.GetInstance ().activateMusic = false;
				UserDataManager.instance.isMusicActive = false;
			}
			else 
			{
				AudioManager.GetInstance ().activateMusic = true;
				UserDataManager.instance.isMusicActive = true;
			}
		}
	}

	public void activateSounds()
	{
		if(AudioManager.GetInstance())
		{
			//AudioManager.GetInstance().Play("fxButton");
			setStateSounds ();

			if(AudioManager.GetInstance().activateSounds)
			{
				AudioManager.GetInstance().activateSounds = false;
				UserDataManager.instance.isSoundEffectsActive = false;
			}
			else
			{
				AudioManager.GetInstance().activateSounds = true;
				UserDataManager.instance.isSoundEffectsActive = true;
			}
		}
	}

	public void setStateMusic()
	{
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
