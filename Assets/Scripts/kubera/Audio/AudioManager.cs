using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour 
{
	public AudioClip lineCreatedAudio;
	public AudioClip piecePositionedAudio;
	public AudioClip buttonAudio;
	public AudioClip wonAudio;
	public AudioClip loseAudio;

	public List<AudioClip> mainThemes;

	public AudioSource audioSource;

	[HideInInspector]public bool soundEffects;

	//TODO: Audio source no tiene contexto de mainAudio
	//TODO: La diferencia tecnica que veo es Play vs PlayOneShot, puede usarse Audio y FX o Music y Audio...
	protected bool _mainAudio;
	protected int currentMainThemeIndex;

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


	void Start()
	{
		soundEffects = UserDataManager.instance.soundEffectsSetting;
		mainAudio = UserDataManager.instance.musicSetting;

		OnLevelWasLoaded();
	}

	//TODO: Hay que optar por la simplesa en los nombres OnLevelLoaded hace la misma chamba
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

	//TODO: Nombre??
	public bool PlayLeLineCreatedAudio()
	{
		if(lineCreatedAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(lineCreatedAudio);
		}
		//Sound was not assigned
		return false;
	}

	public bool PlayPiecePositionedAudio()
	{
		if(piecePositionedAudio != null && soundEffects)
		{
			audioSource.PlayOneShot(piecePositionedAudio);
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

	//TODO un nombre que no denote la existencia de mas de uno
	public bool PauseMainAudio()
	{
		if(mainThemes[currentMainThemeIndex] != null)
		{
			audioSource.Stop();
		}
		return false;
	}

	//TODO un nombre que no denote la existencia de mas de uno
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
