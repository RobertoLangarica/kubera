using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kubera.Data.Sync;

public class FacebookConnectButton : MonoBehaviour 
{
	public delegate void DfacebookConnectNotifications();

	public Text fbText;
	public DfacebookConnectNotifications OnLoggedIn;
	public DfacebookConnectNotifications OnLoggedOut;

	private KuberaSyncManger syncManager;

	public void Awake()
	{
		syncManager = KuberaSyncManger.GetCastedInstance<KuberaSyncManger>();
		changeText (syncManager.facebookProvider.isLoggedIn);

		syncManager.facebookProvider.OnLoginSuccessfull  += OnLoginChange;
		syncManager.facebookProvider.OnLogoutSuccessfull += OnLoginChange;
	}

	public void conectFacebook()
	{
		if(syncManager.facebookProvider.isLoggedIn)
		{
			logOutFacebook ();
			return;
		}

		//TODO: Quitar hardcoding de boton
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}
		
		syncManager.facebookLogin();
	}

	public void OnLoginChange(string message)
	{
		changeText (syncManager.facebookProvider.isLoggedIn);
	}

	public void changeText(bool loggedIn)
	{
		if(fbText)
		{
			//TODO Tomar string del archivo de lenguaje
			if(loggedIn)
			{
				fbText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FB_LOG_OUT_TEXT);

				if (OnLoggedIn != null) 
				{
					OnLoggedIn ();
				}
			}
			else
			{
				fbText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FB_LOG_IN_TEXT);

				if (OnLoggedOut != null) 
				{
					OnLoggedOut ();
				}
			}
		}
	}

	public void logOutFacebook()
	{
		//TODO: Quitar hardcoding de boton
		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("fxButton");
		}

		syncManager.facebookLogout();
	}

	void OnDestroy()
	{
		syncManager.facebookProvider.OnLoginSuccessfull  -= OnLoginChange;
		syncManager.facebookProvider.OnLogoutSuccessfull -= OnLoginChange;
	}
}
