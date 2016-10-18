using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpeningShopika : PopUpBase 
{
	public Text Title;
	public Text Info;
	public Text buttonText;

	public KuberaWebView shopikaWebView;

	void Start()
	{
		Title.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.VISIT_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.VISIT_SHOPIKA_INFO);
		buttonText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.VISIT_SHOPIKA_BUTTON);
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