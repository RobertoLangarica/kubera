using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kubera.Data.Sync;

public class FBConnectPopUp : PopUpBase {

	public Text title;
	public Text descriptionText;
	public Text conectButton;

	public bool askLifes;

	private KuberaSyncManger syncManager;

	void Start()
	{
		syncManager = KuberaSyncManger.GetCastedInstance<KuberaSyncManger>();
	}

	public override void activate()
	{
		popUp.SetActive (true);

		title.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FB_CONNECT_POPUP_TITLE);

		descriptionText.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FB_CONNECT_POPUP_INFO1);
		conectButton.text =  MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FB_CONNECT_POPUP_BUTTON);
	}

	public void closePressed()
	{
		//popUp.SetActive (false);
		OnComplete ();
	}

	public void conectFacebook()
	{
		syncManager.facebookLogin();

		syncManager.facebookProvider.OnLoginSuccessfull += OnComplete;
	}
}
