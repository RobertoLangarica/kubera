using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using utils.gems;
using Kubera.Data;

public class KuberaWebView : MonoBehaviour 
{
	protected const string WEBVIEW_SCHEME = "shopika";

	protected const string WEBVIEW_LOGIN 	= "login";
	protected const string WEBVIEW_GEMS 	= "gemsUpdated";
	protected const string WEBVIEW_FINISH	= "purchaseFinished";

	protected const string VIDEO_URL = "https://s3-us-west-1.amazonaws.com/4mstatic/home_video.mp4";

	public Camera webViewRectCanvasCamera;
	public RectTransform webViewSize;

	public void showShopikaAndRegisterForEvents()
	{
		if (GemsManager.GetCastedInstance<GemsManager> ().currentUserId == GemsManager.GetCastedInstance<GemsManager> ().ANONYMOUS_USER) 
		{
			if (((DataManagerKubera)DataManagerKubera.GetInstance ()).currentUser.firstTimeShopping) 
			{
				((DataManagerKubera)DataManagerKubera.GetInstance ()).currentUser.firstTimeShopping = false;

				URLVideoManager videoM = gameObject.AddComponent<URLVideoManager> ();

				videoM.OnVideoFinished += showShopikaFromVideo;
				videoM.playVideoFromURL (VIDEO_URL);
			} 
			else 
			{
				showShopikaLogin ();
			}
		} 
		else 
		{
			loginToShopika (GemsManager.GetCastedInstance<GemsManager> ().currentUserId,
				GemsManager.GetCastedInstance<GemsManager> ().currentUser.accesToken);
		}

		registerForMessages ();
	}

	protected void showShopikaLogin()
	{
		WebViewManager.GetInstance ().createWebView ("http://shopika-store.cuatromedios.net/standalone-login",webViewSize,webViewRectCanvasCamera,false);
	}

	protected void showShopikaFromVideo(ePlayVideoFinishReason reason)
	{
		showShopikaLogin ();
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
			saveUserInfo (message.Arguments ["userId"],message.Arguments ["tokenId"]);
			loginToShopika (GemsManager.GetCastedInstance<GemsManager> ().currentUserId,
				GemsManager.GetCastedInstance<GemsManager> ().currentUser.accesToken);
			break;
		case(WEBVIEW_GEMS):
			GemsManager.GetCastedInstance<GemsManager>().OnGemsRemotleyChanged();
			break;
		case(WEBVIEW_FINISH):
			closeWebView ();
			break;
		}
	}

	protected void saveUserInfo(string userID,string tokenID)
	{
		//TODO: Guardar datos de login en lugar seguro
		GemsManager.GetCastedInstance<GemsManager>().OnUserLoggedIn(userID,tokenID);
	}

	protected void loginToShopika(string userID,string tokenID)
	{
		string tempHtml = "<script type=\"text/javascript\">\n\tvar tokenId = \"{{TokenID}}\";\n\tvar userId = \"{{UserID}}\";\n</script>\n<form method=\"post\" action=\"http://shopika-store.cuatromedios.net/token-login\" id=\"tokenLoginForm\">\n\t<input type=\"hidden\" name=\"action\" value=\"token-login\">\n\t<input type=\"hidden\" name=\"tokenId\">\n\t<input type=\"hidden\" name=\"userId\">\n</form>\n<script type=\"text/javascript\">\n\tvar form = document.getElementById(\"tokenLoginForm\");\n\tform.tokenId.value = tokenId;\n\tform.userId.value = userId;\n\tform.submit();\n</script> ";

		tempHtml = tempHtml.Replace ("{{TokenID}}",tokenID);
		tempHtml = tempHtml.Replace ("{{UserID}}",userID);

		WebViewManager.GetInstance ().createWebView (tempHtml, true);
	}

	protected void closeWebView()
	{
		WebViewManager.GetInstance ().destroyWebView ();
	}
}