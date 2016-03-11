using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour 
{
	public AudioClip lineCreatedAudio;
	public AudioClip piecePositionatedAudio;
	public AudioClip buttonAudio;
	public AudioClip wonAudio;
	public AudioClip loseAudio;

	public List<AudioClip> mainThemes;

	public AudioSource audioSource;

	[HideInInspector]
	public bool soundEffects;

	protected bool _mainAudio;

	[HideInInspector]
	public bool mainAudio
	{
		get{
			return _mainAudio;
		}

		set{
			_mainAudio = value;

			if(_mainAudio == true)
			{
				PlayMainAudio();
			}
			else
			{
				PauseMainAudio();
			}
		}
	}

	protected int currentMainThemeIndex;

	void Start()
	{
		soundEffects = UserDataManager.instance.soundEffectsSetting;
		mainAudio = UserDataManager.instance.musicSetting;

		OnLevelWasLoaded();
	}

	void OnLevelWasLoaded()
	{
		switch(SceneManager.GetActiveScene().name)
		{
		case("Game"):
			PauseMainAudio();
			currentMainThemeIndex = 0;
			PlayMainAudio();
			break;
		}
	}

	public bool PlayLeLineCreatedAudio()
	{
		if(lineCreatedAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(lineCreatedAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PlayPiecePositionatedAudio()
	{
		if(piecePositionatedAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(piecePositionatedAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PlayLoseAudio()
	{
		if(loseAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(loseAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PlayWonAudio()
	{
		if(wonAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(wonAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PlayButtonAudio()
	{
		if(buttonAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(buttonAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PauseMainAudio()
	{
		if(mainThemes[currentMainThemeIndex] != null)
		{
			audioSource.Stop();
		}
		return false;
	}

	public bool PlayMainAudio()
	{
		if(mainThemes[currentMainThemeIndex] != null && mainAudio)
		{
			audioSource.clip = mainThemes[currentMainThemeIndex];
			audioSource.Play();
		}
		return false;
	}
}
