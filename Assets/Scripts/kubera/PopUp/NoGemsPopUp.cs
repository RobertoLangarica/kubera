using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems;

public class NoGemsPopUp : PopUpBase {

	public Text Title;
	public Text Info;
	public Text buttonText;
	public Text alternateButtonText;

	public KuberaWebView shopikaWebView;

	void Start()
	{
		Title.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NO_GEMS_POPUP_TITLE);
		if (ShopikaManager.GetCastedInstance<ShopikaManager> ().currentUserId == ShopikaManager.GetCastedInstance<ShopikaManager> ().ANONYMOUS_USER) 
		{
			Info.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NO_GEMS_POPUP_INFO_NO_LOGIN);
		} 
		else 
		{
			Info.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NO_GEMS_POPUP_INFO);
		}

		buttonText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NO_GEMS_POPUP_BUTTON);
		alternateButtonText.text = buttonText.text;
	}

	public void Exit()
	{
		OnComplete ();
	}

	public void goToShopika()
	{
		OnComplete ();


		//HARDCODING para ue este cambio este en android e iOS
		/*#if UNITY_IOS || UNITY_EDITOR
		if(ShopikaManager.GetCastedInstance<ShopikaManager>().canGiveLocalGems())
		{
			ShopikaManager.GetCastedInstance<ShopikaManager>().giveLocalAnonymousGems();
		}
		#endif*/
		if(ShopikaManager.GetCastedInstance<ShopikaManager>().canGiveLocalGems())
		{
			ShopikaManager.GetCastedInstance<ShopikaManager>().giveLocalAnonymousGems();
		}


		#if UNITY_EDITOR
		//Evitamos el webview en editor porque no funciona
		return;
		#endif

		//HARDCODING para ue este cambio este en android e iOS
		/*#if UNITY_IOS
		shopikaWebView.showShopikaLanding();
		#else
		shopikaWebView.showShopikaAndRegisterForEvents();
		#endif*/
		shopikaWebView.showShopikaLanding();
	}
}
