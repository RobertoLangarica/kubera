using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kubera.Data.Sync;

public class FacebookConnectButton : MonoBehaviour {

	public Text fbText;

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
		//TODO Tomar string del archivo de lenguaje
		if(loggedIn)
		{
			fbText.text = "Desconectate";
		}
		else
		{
			fbText.text = "Conectate";
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
}
