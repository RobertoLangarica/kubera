using UnityEngine;
using System.Collections;

public class SettingsButton : MonoBehaviour 
{
	public void activateMusic()
	{
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);

		if(AudioManager.instance.musicActive)
		{
			AudioManager.instance.musicActive = false;
			UserDataManager.instance.isMusicActive = false;
		}
		else
		{
			AudioManager.instance.musicActive = true;
			UserDataManager.instance.isMusicActive = true;
		}
	}

	public void activateSounds()
	{
		AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);

		if(AudioManager.instance.soundEffectsActive)
		{
			AudioManager.instance.soundEffectsActive = false;
			UserDataManager.instance.isSoundEffectsActive = false;
		}
		else
		{
			AudioManager.instance.soundEffectsActive = true;
			UserDataManager.instance.isSoundEffectsActive = true;
		}
	}
}
