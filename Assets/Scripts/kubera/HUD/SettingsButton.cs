using DG.Tweening;
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

	public Button Music;
	public Button Exit;
	public Button Sounds;
	public GameObject settingsBackground;
	public GameObject PointerOnScene;

	public delegate void DOnActivateMusic(bool activate);
	public DOnActivateMusic OnActivateMusic;

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

				AudioManager.GetInstance ().StopAllAudiosInCategory ("MAIN MUSIC");

				if(OnActivateMusic != null)
				{
					OnActivateMusic (false);
				}
			}
			else 
			{
				AudioManager.GetInstance ().activateMusic = true;
				UserDataManager.instance.isMusicActive = true;

				if(OnActivateMusic != null)
				{
					OnActivateMusic (true);
				}
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

	public void activeSettings()
	{
		/*print (activate);
		print (Sounds.gameObject.activeSelf);*/
		if (!settingsBackground.activeSelf) 
		{
			DOTween.Kill(settingsBackground,true);

			Exit.enabled = false;
			Music.enabled = false;
			Sounds.enabled = false;
			Exit.transform.localScale = Vector2.zero;
			Music.transform.localScale = Vector2.zero;
			Sounds.transform.localScale = Vector2.zero;

			settingsBackground.transform.localScale = Vector3.one;
			settingsBackground.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -180));
			settingsBackground.SetActive(true);

			settingsBackground.GetComponent<RectTransform> ().pivot = new Vector2(0.5f,0.5f);

			//Activar los otros botones
			settingsBackground.transform.DOLocalRotate (Vector3.zero,0.3f).SetId(settingsBackground).OnComplete(()=>
				{


					Exit.enabled = true;
					Music.enabled = true;
					Sounds.enabled = true;

					Exit.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
					Music.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
					Sounds.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
				});
			PointerOnScene.SetActive(true);
		}
		else 
		{
			DOTween.Kill(settingsBackground,true);

			Exit.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack);
			Music.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack);
			Sounds.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack).OnComplete(()=>
				{
					settingsBackground.GetComponent<RectTransform> ().pivot = Vector2.one;

					settingsBackground.transform.DOScale (new Vector3 (0, 0,0),0.3f).SetId(settingsBackground).OnComplete(()=>
						{
							settingsBackground.SetActive(false);

						});
				});

			PointerOnScene.SetActive(false);
		}
	}
}
