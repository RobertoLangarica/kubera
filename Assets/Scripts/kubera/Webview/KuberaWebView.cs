﻿using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using utils.gems;

public class KuberaWebView : MonoBehaviour 
{
	protected const string WEBVIEW_LOGIN 	= "login";
	protected const string WEBVIEW_GEMS 	= "gems";
	protected const string WEBVIEW_FINISH	= "finish";

	public Camera webViewRectCanvasCamera;
	public RectTransform webViewSize;

	public void showShopikaAndRegisterForEvents()
	{
		registerForMessages ();

		if (GemsManager.GetCastedInstance<GemsManager> ().currentUserId == GemsManager.GetCastedInstance<GemsManager> ().ANONYMOUS_USER) 
		{
			showShopikaLogin ();
		} 
		else 
		{
			loginToShopika (GemsManager.GetCastedInstance<GemsManager> ().currentUserId,
				GemsManager.GetCastedInstance<GemsManager> ().currentUser.accesToken);
		}
	}

	protected void showShopikaLogin()
	{
		WebViewManager.GetInstance ().createWebView ("http://shopika-store.cuatromedios.net/",webViewSize,webViewRectCanvasCamera,false);
	}

	protected void registerForMessages()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView (WEBVIEW_LOGIN, messageCallBack);

		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView (WEBVIEW_GEMS, messageCallBack);

		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView (WEBVIEW_FINISH, messageCallBack);
	}

	protected void messageCallBack(WebView webview,WebViewMessage message)
	{
		switch (message.Scheme) 
		{
		case(WEBVIEW_LOGIN):
			saveUserInfo (message.Arguments ["userId"],message.Arguments ["tokenId"]);
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
		string tempHtml = "<script type=\"text/javascript\">\n\tvar tokenId = \"{{TokenID}}\";\n\tvar userId = \"{{UserID}}\";\n</script>\n<form method=\"post\" action=\"http://shopika-store.cuatromedios.net/\" id=\"tokenLoginForm\">\n\t<input type=\"hidden\" name=\"action\" value=\"token-login\">\n\t<input type=\"hidden\" name=\"tokenId\">\n\t<input type=\"hidden\" name=\"userId\">\n</form>\n<script type=\"text/javascript\">\n\tvar form = document.getElementById(\"tokenLoginForm\");\n\tform.tokenId.value = tokenId;\n\tform.userId.value = userId;\n\tform.submit();\n</script> ";

		tempHtml = tempHtml.Replace ("{{TokenID}}",tokenID);
		tempHtml = tempHtml.Replace ("{{UserID}}",userID);

		Debug.Log (tempHtml);

		WebViewManager.GetInstance ().createWebView (tempHtml, true);
	}

	protected void closeWebView()
	{
		WebViewManager.GetInstance ().destroyWebView ();
	}
}