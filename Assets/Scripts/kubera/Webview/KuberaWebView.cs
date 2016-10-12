using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using utils.gems;
using Kubera.Data;

public class KuberaWebView : MonoBehaviour 
{
	public delegate void DwebViewNotifications();

	public GameObject videoModal;

	protected const string WEBVIEW_SCHEME = "shopika";

	protected const string WEBVIEW_LOGIN 	= "login";
	protected const string WEBVIEW_GEMS 	= "gemsUpdated";
	protected const string WEBVIEW_FINISH	= "purchaseFinished";

	public Camera webViewRectCanvasCamera;
	public RectTransform webViewSize;
	public GameObject customToolBar;

	public DwebViewNotifications OnLoggedIn;

	void Start()
	{
		videoModal.SetActive(false);
	}

	public void showShopikaAndRegisterForEvents()
	{
		videoModal.SetActive(true);
		if (GemsManager.GetCastedInstance<GemsManager> ().currentUserId == GemsManager.GetCastedInstance<GemsManager> ().ANONYMOUS_USER) 
		{
			if (((DataManagerKubera)DataManagerKubera.GetInstance ()).currentUser.firstTimeShopping) 
			{
				((DataManagerKubera)DataManagerKubera.GetInstance ()).currentUser.firstTimeShopping = false;
			} 
			showShopikaLogin ();
		} 
		else 
		{
			loginToShopika (GemsManager.GetCastedInstance<GemsManager> ().currentUserId,
				GemsManager.GetCastedInstance<GemsManager> ().currentUser.accesToken);
		}

		WebViewManager.GetInstance ().OnFinishLoading += showToolBar;
		registerForMessages ();
	}

	protected void showShopikaLogin()
	{
		videoModal.SetActive(true);
		WebViewManager.GetInstance ().createWebView ("http://shopika-store.cuatromedios.net/standalone-login",webViewSize,webViewRectCanvasCamera,false);
		//WebViewManager.GetInstance ().displayWebView.CanBounce = true;
	}

	protected void registerForMessages()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView (WEBVIEW_SCHEME, messageCallBack);
	}

	protected void messageCallBack(WebView webview,WebViewMessage message)
	{
		switch (message.Host) 
		{
		case(WEBVIEW_LOGIN):
			saveUserInfo (message.Arguments ["userId"], message.Arguments ["tokenId"],message.Arguments["displayName"]);
			loginToShopika (GemsManager.GetCastedInstance<GemsManager> ().currentUserId,
				GemsManager.GetCastedInstance<GemsManager> ().currentUser.accesToken);

			if (OnLoggedIn != null) 
			{
				OnLoggedIn ();
			}

			break;
		case(WEBVIEW_GEMS):
			
			if (!((DataManagerKubera)DataManagerKubera.GetInstance ()).alreadyPurchaseGems ()) 
			{
				KuberaAnalytics.GetInstance ().registerGemsFirstPurchase (PersistentData.GetInstance().lastLevelReachedName);
				((DataManagerKubera)DataManagerKubera.GetInstance ()).markGemsAsPurchased ();
			}

			GemsManager.GetCastedInstance<GemsManager>().OnGemsRemotleyChanged();
			break;
		case(WEBVIEW_FINISH):
			closeWebView ();
			GemsManager.GetCastedInstance<GemsManager>().OnGemsRemotleyChanged();
			break;
		}
	}

	protected void saveUserInfo(string userID,string tokenID,string displayName)
	{
		//TODO: Guardar datos de login en lugar seguro
		GemsManager.GetCastedInstance<GemsManager>().OnUserLoggedIn(userID,tokenID,displayName);
	}

	protected void loginToShopika(string userID,string tokenID)
	{
		string tempHtml = "<script type=\"text/javascript\">\n\tvar tokenId = \"{{TokenID}}\";\n\tvar userId = \"{{UserID}}\";\n</script>\n<form method=\"post\" action=\"http://shopika-store.cuatromedios.net/token-login\" id=\"tokenLoginForm\">\n\t<input type=\"hidden\" name=\"action\" value=\"token-login\">\n\t<input type=\"hidden\" name=\"tokenId\">\n\t<input type=\"hidden\" name=\"userId\">\n</form>\n<script type=\"text/javascript\">\n\tvar form = document.getElementById(\"tokenLoginForm\");\n\tform.tokenId.value = tokenId;\n\tform.userId.value = userId;\n\tform.submit();\n</script> ";

		tempHtml = tempHtml.Replace ("{{TokenID}}",tokenID);
		tempHtml = tempHtml.Replace ("{{UserID}}",userID);

		videoModal.SetActive(true);

		WebViewManager.GetInstance ().createWebView (tempHtml,webViewSize,webViewRectCanvasCamera,true);
		//WebViewManager.GetInstance ().displayWebView.CanBounce = true;
	}

	public void closeWebView()
	{
		if(videoModal.activeInHierarchy)
		{
			videoModal.SetActive(false);	
		}
		WebViewManager.GetInstance().OnFinishLoading -= showToolBar;
		WebViewManager.GetInstance ().stopLoading();
		WebViewManager.GetInstance ().hideWebView();
		customToolBar.SetActive (false);
	}

	public void showToolBar(WebView webview)
	{
		//Debug.Log (customToolBar + "******************");
		WebViewManager.GetInstance().showWebView();
		customToolBar.SetActive (true);
	}
}