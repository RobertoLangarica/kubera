﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems;

public class NoGemsPopUp : PopUpBase {

	public Text Title;
	public Text Info;
	public Text buttonText;

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
	}

	public void Exit()
	{
		OnComplete ();
	}

	public void goToShopika()
	{
		OnComplete ();
		//TODO: abrir webView shopika
		shopikaWebView.showShopikaAndRegisterForEvents();
	}
}
