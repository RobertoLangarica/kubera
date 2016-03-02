using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour 
{
	public AudioClip lineCreatedAudio;
	public AudioClip piecePositionatedAudio;
	public AudioClip buttonAudio;
	public AudioClip wonAudio;
	public AudioClip loseAudio;

	public List<AudioClip> mainThemes;

	public bool soundEffects;

	protected bool _mainAudio;

	public bool mainAudio
	{
		get{
			return _mainAudio;
		}

		set{

			if(value)
			{
				PlayMainAudio();
			}
			else
			{
				PauseMainAudio();
			}

			_mainAudio = value;
		}
	}

	protected int currentMainThemeIndex;

	protected AudioSource audioSource;

	void Start()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
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
