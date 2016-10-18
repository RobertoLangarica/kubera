using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems;
using utils.gems.sync;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

public class ShopikaConnectPopUp : PopUpBase {

	public Text title;
	public Text Info;
	public Button exitBTN;

	void Start()
	{
		exitBTN.gameObject.SetActive (false);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_INFO);

		utils.gems.sync.ShopikaSyncManager.GetCastedInstance<utils.gems.sync.ShopikaSyncManager> ().OnDataRetrieved  += popUpCompleted;
		utils.gems.sync.ShopikaSyncManager.GetCastedInstance<utils.gems.sync.ShopikaSyncManager> ().OnDataRetrievedFailure += failure;

		WebViewManager.GetInstance ().OnFinishLoading += webClosed;
	}

	public override void activate()
	{
		popUp.SetActive (true);
		exitBTN.gameObject.SetActive (false);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_INFO);
	}

	protected void failure()
	{
		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_SHOPIKA_INFO);
		exitBTN.gameObject.SetActive (true);
	}

	public void exit()
	{
		popUpCompleted ();
	}

	protected void popUpCompleted()
	{
		popUp.SetActive (false);
		OnComplete ("");
	}

	protected void webClosed(WebView webview)
	{
		if (ShopikaManager.GetCastedInstance<ShopikaManager> ().currentUserId == ShopikaManager.GetCastedInstance<ShopikaManager> ().ANONYMOUS_USER) 
		{
			failure ();
		}
	}
}
