﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerOld : MonoBehaviour 
{
	public enum ESOUND_EFFECTS
	{
		LINE_CREATED,
		PIECE_POSITIONATED,
		BUTTON,
		WON,
		LOSE
	}

	public const string gameScene = "Game";

	public AudioClip lineCreatedEffect;
	public AudioClip piecePositionedEffect;
	public AudioClip buttonEffect;
	public AudioClip wonEffect;
	public AudioClip loseEffect;

	public List<AudioClip> musicList;

	public AudioSource audioSource;

	public static AudioManagerOld instance = null;

	[HideInInspector]public bool soundEffectsActive;

	protected bool _musicActive;
	protected int currentMainThemeIndex;

	[HideInInspector]
	public bool musicActive
	{
		get{
			return _musicActive;
		}

		set{
			_musicActive = value;

			if(_musicActive == true)
			{
				PlayCurrentMusic();
			}
			else
			{
				PauseCurrentMusic();
			}
		}
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		soundEffectsActive = UserDataManager.instance.isSoundEffectsActive;
		musicActive = UserDataManager.instance.isMusicActive;

		OnLevelWasLoaded();
	}

	void OnLevelWasLoaded()
	{
		PauseCurrentMusic();
		switch(SceneManager.GetActiveScene().name)
		{
		case(gameScene):
			currentMainThemeIndex = 0;
			break;
		}
		PlayCurrentMusic();
	}

	public bool PlaySoundEffect(ESOUND_EFFECTS effect)
	{
		AudioClip clip = null;

		switch (effect) 
		{
		case(ESOUND_EFFECTS.LINE_CREATED):
			clip = lineCreatedEffect;
			break;
		case(ESOUND_EFFECTS.PIECE_POSITIONATED):
			clip = piecePositionedEffect;
			break;
		case(ESOUND_EFFECTS.BUTTON):
			clip = buttonEffect;
			break;
		case(ESOUND_EFFECTS.WON):
			clip = wonEffect;
			break;
		case(ESOUND_EFFECTS.LOSE):
			clip = loseEffect;
			break;
		}

		if (clip != null) 
		{
			return PlayAudioClipOneShot (clip);
		}

		return false;
	}

	public bool PlayAudioClipOneShot(AudioClip clip)
	{
		if (soundEffectsActive) 
		{
			audioSource.PlayOneShot (clip);
			return true;
		}
		return false;
	}

	public bool PauseCurrentMusic()
	{
		if(musicList[currentMainThemeIndex] != null)
		{
			audioSource.Stop();
		}
		return false;
	}

	public bool PlayCurrentMusic()
	{
		if(musicList[currentMainThemeIndex] != null && musicActive)
		{
			audioSource.clip = musicList[currentMainThemeIndex];
			audioSource.Play();
		}
		return false;
	}
}
