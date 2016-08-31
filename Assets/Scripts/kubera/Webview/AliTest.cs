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
		WebViewManager.GetInstance ().createWebView ("http://45.55.222.58:3003/login",webViewSize,webViewRectCanvasCamera);
	}

	public void registerForTelMessage()
	{
		WebViewManager.GetInstance ().registerToReceiveMessageFromWebView ("tel", messageCallBack);
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
		Debug.Log ("Soy el registrado");
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