using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

[RequireComponent(typeof(WebView))]
public class WebViewManager : Manager<WebViewManager> 
{
	protected class ReceivedMessage
	{
		public string messageID;
		public DWebViewReceivedMessage action;

		public ReceivedMessage (string ID,DWebViewReceivedMessage nAction)
		{
			messageID = ID;
			action += nAction;
		}
	}

	public delegate void DWebViewReceivedMessage (WebView webview,WebViewMessage message);

	public Action<WebView> OnStartLoading;
	public Action<WebView> OnFinishLoading;
	public Action<WebView,string> OnFailLoading;
	public Action<WebView> OnShow;
	public Action<WebView> OnHide;
	public Action<WebView> OnDestroy;

	protected WebView displayWebView;
	protected Dictionary<string,ReceivedMessage> webViewMessagesSubscriptors;

	void Start()
	{
		displayWebView = GetComponent<WebView> ();

		registerAllListeners ();

		webViewMessagesSubscriptors = new Dictionary<string, ReceivedMessage> ();
	}

	protected void registerAllListeners()
	{
		WebView.DidShowEvent						+= showEvent;
		WebView.DidHideEvent						+= hideEvent;
		WebView.DidDestroyEvent						+= destroyEvent;
		WebView.DidStartLoadEvent					+= startLoadingEvent;
		WebView.DidFinishLoadEvent					+= finishLoadingEvent;
		WebView.DidFailLoadWithErrorEvent			+= failLoadingEvent;
		WebView.DidReceiveMessageEvent				+= receiveMessageEvent;
	}

	protected void unregisterAllListeners()
	{
		WebView.DidShowEvent						-= showEvent;
		WebView.DidHideEvent						-= hideEvent;
		WebView.DidDestroyEvent						-= destroyEvent;
		WebView.DidStartLoadEvent					-= startLoadingEvent;
		WebView.DidFinishLoadEvent					-= finishLoadingEvent;
		WebView.DidFailLoadWithErrorEvent			-= failLoadingEvent;
		WebView.DidReceiveMessageEvent				-= receiveMessageEvent;
	}

	public void createWebView(string nURL,bool fullScreen)
	{
		if (fullScreen) 
		{
			displayWebView.SetFullScreenFrame ();
		}

		createWebView (nURL);
	}

	public void createWebView(string nURL,RectTransform rectTransform,Camera canvasCamera)
	{
		Vector3[] temp = new Vector3[4];

		rectTransform.GetWorldCorners (temp);

		Vector3 screenPoint = canvasCamera.WorldToScreenPoint (temp[1]);

		displayWebView.Frame = new Rect(screenPoint.x,Screen.height - screenPoint.y,rectTransform.rect.width,rectTransform.rect.height);
		createWebView (nURL);
	}

	public void createWebView(string nURL,Rect viewRect)
	{
		displayWebView.Frame = viewRect;
		createWebView (nURL);
	}

	public void createWebView (string nURL)
	{
		displayWebView.LoadRequest(nURL);
		//HTML de prueba
		//displayWebView.LoadHTMLString("<!DOCTYPE html>\n<html>\n<body bgcolor=\"#91ACFF\">\n\t<font color=\"white\">\n\t\t<h1><center>WebView Demo</center></h1>\n\t\t<p>This code is used demo purpose. Can be used to test following features</p>\n\t\t<ul type=\"square\">\n\t\t\t<li>Load HTML string</li>\n\t\t\t<li>Load HTML string with JS</li>\n\t\t\t<li>Evaluate JS</li>\n\t\t\t<li>Send message to Unity3d</li>\n\t\t</ul>\n\t</font>\n\t<center><input type='button' value=\"Open scheme: unity\" onclick='window.location=\"unity://test/number?var1=12345&var2=67890\"'/></center>\n\n\t<center><input type='button' value=\"Open scheme: mailto\" onclick='window.location=\"mailto://info@airportpark.de\"'/></center>\n\n\t<center><input type='button' value=\"Open scheme: tel\" onclick='window.location=\"tel://+111111111111\"'/></center>\n\n\t<script>\n\tfunction Concat (str1, str2) \n\t{\n\t\treturn str1.concat(str2);\n\t}\n\t</script>\n</body>\n</html>");
	}

	public void changeViewControls(eWebviewControlType control)
	{
		displayWebView.ControlType = control;
	}

	public void showWebView ()
	{
		displayWebView.Show();
	}

	public void hideWebView ()
	{
		displayWebView.Hide();
	}

	public void destroyWebView ()
	{
		displayWebView.Destroy();
	}

	protected void clearCache()
	{
		displayWebView.ClearCache ();
	}

	public void registerToReceiveMessageFromWebView(string messageID,DWebViewReceivedMessage callBack)
	{
		if (webViewMessagesSubscriptors.ContainsKey (messageID)) 
		{
			webViewMessagesSubscriptors [messageID].action += callBack;
		} 
		else 
		{
			ReceivedMessage nMessage = new ReceivedMessage (messageID,callBack);

			webViewMessagesSubscriptors.Add (messageID, nMessage);

			displayWebView.AddNewURLSchemeName (messageID);
		}
	}

	public void unRegisterFromMessage(string messageID,DWebViewReceivedMessage callBack)
	{
		if(webViewMessagesSubscriptors.ContainsKey(messageID))
		{
			webViewMessagesSubscriptors [messageID].action -= callBack;

			if (webViewMessagesSubscriptors [messageID].action == null) 
			{
				webViewMessagesSubscriptors.Remove (messageID);
			}
		}
	}

	protected void startLoadingEvent(WebView webview)
	{
		Debug.Log("Started loading webpage contents.");
		Debug.Log(string.Format("URL: {0}.", webview.URL));

		if (OnStartLoading != null) 
		{
			OnStartLoading (webview);
		}
	}

	protected void finishLoadingEvent (WebView webview)
	{
		Debug.Log("Finished loading webpage contents.");

		if (OnFinishLoading != null) 
		{
			OnFinishLoading (webview);
		}
	}

	protected void failLoadingEvent (WebView webview, string error)
	{
		Debug.Log("Failed to load requested contents.");
		Debug.Log(string.Format("Error: {0}.", error));

		if (OnFailLoading != null) 
		{
			OnFailLoading (webview,error);
		}
	}

	protected void showEvent (WebView webview)
	{
		Debug.Log("Displaying web view on screen.");

		if (OnShow != null) 
		{
			OnShow (webview);
		}
	}

	protected void hideEvent (WebView webview)
	{
		Debug.Log("Dismissed web view from the screen.");

		if (OnHide != null) 
		{
			OnHide (webview);
		}
	}

	protected void destroyEvent (WebView webview)
	{
		Debug.Log("Released web view instance.");

		if (OnDestroy != null) 
		{
			OnDestroy (webview);
		}
	}

	protected void receiveMessageEvent (WebView webview,  WebViewMessage message)
	{
		Debug.Log("Received a new message from web view.");
		Debug.Log(string.Format("Host: {0}.", 		message.Host));
		Debug.Log(string.Format("Scheme: {0}.", 		message.Scheme));
		Debug.Log(string.Format("URL: {0}.", 		message.URL));
		Debug.Log(string.Format("Arguments: {0}.", 	message.Arguments.ToJSON()));

		if (webViewMessagesSubscriptors.ContainsKey (message.Scheme)) 
		{
			webViewMessagesSubscriptors [message.Scheme].action (webview, message);
		}
	}
}
