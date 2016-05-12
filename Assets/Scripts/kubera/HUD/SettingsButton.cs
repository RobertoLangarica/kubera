using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsButton : MonoBehaviour 
{
	public void activateMusic()
	{
		if (PersistentData.instance.startLevel > 1) 
		{
			PersistentData.instance.startLevel -= 2;
			SceneManager.LoadScene ("Game");
		}
		/*AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);

		if(AudioManager.instance.musicActive)
		{
			AudioManager.instance.musicActive = false;
			UserDataManager.instance.isMusicActive = false;
		}
		else
		{
			AudioManager.instance.musicActive = true;
			UserDataManager.instance.isMusicActive = true;
		}*/
	}

	public void activateSounds()
	{
		SceneManager.LoadScene ("Game");
		/*AudioManager.instance.PlaySoundEffect(AudioManager.ESOUND_EFFECTS.BUTTON);

		if(AudioManager.instance.soundEffectsActive)
		{
			AudioManager.instance.soundEffectsActive = false;
			UserDataManager.instance.isSoundEffectsActive = false;
		}
		else
		{
			AudioManager.instance.soundEffectsActive = true;
			UserDataManager.instance.isSoundEffectsActive = true;
		}*/
	}
}
