using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

public class KuberaWebView : MonoBehaviour 
{
	protected const string WEBVIEW_LOGIN = "login";

	public Camera webViewRectCanvasCamera;
	public RectTransform webViewSize;

	public void showShopikaAndRegisterForEvents()
	{
		showShopikaLogin ();
		registerForMessages ();
	}

	public void showShopikaLogin()
	{
		WebViewManager.GetInstance ().createWebView ("http://45.55.222.58:3003/standalone-login",webViewSize,webViewRectCanvasCamera,false);
	}

	protected void registerForMessages()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView (WEBVIEW_LOGIN, messageCallBack);
	}

	protected void messageCallBack(WebView webview,WebViewMessage message)
	{
		switch (message.Scheme) 
		{
		case(WEBVIEW_LOGIN):
			saveUserInfo (message.Arguments ["userId"],message.Arguments ["tokenId"]);
			break;
		}
	}

	protected void saveUserInfo(string userID,string tokenID)
	{
		//TODO: Guardar datos de login en lugar seguro
	}

	public void loginToShopika(string userID,string tokenID)
	{
		string tempHtml = "<script type=\"text/javascript\">\n\tvar tokenId = \"{{TokenID}}\";\n\tvar userId = \"{{UserID}}\";\n</script>\n<form method=\"post\" action=\"http://45.55.222.58:3003/token-login\" id=\"tokenLoginForm\">\n\t<input type=\"hidden\" name=\"action\" value=\"token-login\">\n\t<input type=\"hidden\" name=\"tokenId\">\n\t<input type=\"hidden\" name=\"userId\">\n</form>\n<script type=\"text/javascript\">\n\tvar form = document.getElementById(\"tokenLoginForm\");\n\tform.tokenId.value = tokenId;\n\tform.userId.value = userId;\n\tform.submit();\n</script> ";

		tempHtml = tempHtml.Replace ("{{TokenID}}",tokenID);
		tempHtml = tempHtml.Replace ("{{UserID}}",userID);

		Debug.Log (tempHtml);

		WebViewManager.GetInstance ().createWebView (tempHtml, true);
	}

	public void closeWebView()
	{
		WebViewManager.GetInstance ().destroyWebView ();
	}
}