using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

public class AliTest : MonoBehaviour 
{
	public Camera webViewRectCanvasCamera;
	public RectTransform webViewSize;

	public void createWebView()
	{
		WebViewManager.GetInstance ().createWebView ("http://45.55.222.58:3003/standalone-login",webViewSize,webViewRectCanvasCamera,false);
	}

	public void registerForTelMessage()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView ("login", messageCallBack);
	}

	public void registerForMailToMessage()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView ("mailto", messageCallBack);
	}

	public void unregisterForTelMessage()
	{
		WebViewManager.GetInstance ().unRegisterFromMessage ("tel", messageCallBack);
	}

	public void unregisterForMailToMessage()
	{
		WebViewManager.GetInstance ().unRegisterFromMessage("mailto", messageCallBack);
	}

	public void messageCallBack(WebView webview,WebViewMessage message)
	{
		/*Debug.Log ("Soy el registrado");
		Debug.Log (message.Host);
		Debug.Log (message.Scheme);
		Debug.Log (message.URL);
		Debug.Log (message.Arguments.ToJSON().ToString());*/

		Debug.Log (message.Arguments ["tokenId"]);
		Debug.Log (message.Arguments ["userId"]);

		string tempHtml = "<script type=\"text/javascript\">\n\tvar tokenId = \"{{TokenID}}\";\n\tvar userId = \"{{UserID}}\";\n</script>\n<form method=\"post\" action=\"http://45.55.222.58:3003/token-login\" id=\"tokenLoginForm\">\n\t<input type=\"hidden\" name=\"action\" value=\"token-login\">\n\t<input type=\"hidden\" name=\"tokenId\">\n\t<input type=\"hidden\" name=\"userId\">\n</form>\n<script type=\"text/javascript\">\n\tvar form = document.getElementById(\"tokenLoginForm\");\n\tform.tokenId.value = tokenId;\n\tform.userId.value = userId;\n\tform.submit();\n</script> ";

		tempHtml = tempHtml.Replace ("{{TokenID}}",message.Arguments ["tokenId"]);
		tempHtml = tempHtml.Replace ("{{UserID}}",message.Arguments ["userId"]);

		Debug.Log (tempHtml);

		WebViewManager.GetInstance ().createWebView (tempHtml, true);
	}

	public void setWholeHUD()
	{
		WebViewManager.GetInstance ().changeViewControls (eWebviewControlType.TOOLBAR);
	}

	public void setCloseButton()
	{
		WebViewManager.GetInstance ().changeViewControls (eWebviewControlType.CLOSE_BUTTON);
	}

	public void showVideo()
	{
		URLVideoManager.GetInstance ().playVideoFromURL ("https://s3-us-west-1.amazonaws.com/4mstatic/home_video.mp4");
	}

	public void setVideoCAllBack()
	{
		URLVideoManager.GetInstance ().OnVideoFinished += PlayVideoFinished;
	}

	private void PlayVideoFinished (ePlayVideoFinishReason reason)
	{
		Debug.Log("Me registre al video");
	}
}