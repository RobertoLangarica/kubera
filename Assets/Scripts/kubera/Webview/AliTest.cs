using UnityEngine;
using System.Collections;
using VoxelBusters.Utility.UnityGUI.MENU;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

public class AliTest : MonoBehaviour 
	{
		[SerializeField]
		private 	string 		m_url;
		[SerializeField, Multiline(6)]
		private 	string		m_HTMLString;
		[SerializeField]
		private 	string		m_javaScript;
		[SerializeField]
		private 	string		m_evalString;
		[SerializeField]
		private 	string[]	m_schemeList	= new string[] {
			"unity",
			"mailto",
			"tel"
		};

		private 	WebView		m_webview;

	private URL _url;

		void Start ()
		{
			// Cache instances
		/*m_webview	= FindObjectOfType<WebView>();

		// Set frame
		if (m_webview != null)
			SetFrame();

		// Registering callbacks
		WebView.DidShowEvent						+= DidShowEvent;
		WebView.DidHideEvent						+= DidHideEvent;
		WebView.DidDestroyEvent						+= DidDestroyEvent;
		WebView.DidStartLoadEvent					+= DidStartLoadEvent;
		WebView.DidFinishLoadEvent					+= DidFinishLoadEvent;
		WebView.DidFailLoadWithErrorEvent			+= DidFailLoadWithErrorEvent;
		WebView.DidFinishEvaluatingJavaScriptEvent	+= DidFinishEvaluatingJavaScriptEvent;
		WebView.DidReceiveMessageEvent				+= DidReceiveMessageEvent;

		LoadHTMLString ();

		AddNewURLSchemeName ();*/

		_url = new URL ("https://s3-us-west-1.amazonaws.com/4mstatic/home_video.mp4");

		Debug.Log ("Empeza");
		//media.PlayVideoFromURL (_url,(r)=>{Debug.Log("Completo");});
		NPBinding.MediaLibrary.PlayVideoFromURL(_url, PlayVideoFinished);

		}

		#region API Methods

		private void LoadRequest ()
		{
			m_webview.LoadRequest(m_url);
		}

		private void LoadHTMLString ()
		{
			m_webview.LoadHTMLString(m_HTMLString);
		}

		private void LoadHTMLStringWithJavaScript ()
		{
			m_webview.LoadHTMLStringWithJavaScript(m_HTMLString, m_javaScript);						
		}

		private void LoadFile ()
		{
		Debug.Log ("LoadFile");
			//m_webview.LoadFile(Demo.Utility.GetScreenshotPath(), "image/png", null, null);
		}

		private void EvaluateJavaScriptFromString ()
		{
			m_webview.EvaluateJavaScriptFromString(m_evalString);
		}

		private void ShowWebView ()
		{
			m_webview.Show();
		}

		private void HideWebView ()
		{
			m_webview.Hide();
		}

		private void DestroyWebView ()
		{
			m_webview.Destroy();
		}

		private void AddNewURLSchemeName ()
		{
		Debug.Log("Registered schemes for receiving web view messages.");

			for (int _iter = 0; _iter < m_schemeList.Length; _iter++)
				m_webview.AddNewURLSchemeName(m_schemeList[_iter]);
		}

		private void SetFrame ()
		{
			m_webview.Frame		= new Rect(0f, Screen.height * 0.3f, Screen.width, Screen.height * 0.5f);

		Debug.Log(string.Format("Setting new frame: {0} for web view.", m_webview.Frame));
		}

		private void SetFullScreenFrame ()
		{
			m_webview.SetFullScreenFrame();

		Debug.Log("Setting web view frame to full screen.");
		}

		private void ClearCache ()
		{
			m_webview.ClearCache();

		Debug.Log("Cleared web view cache.");
		}

		#endregion

		#region API Callback Methods

		private void DidShowEvent (WebView _webview)
		{
		Debug.Log("Displaying web view on screen.");
		}

		private void DidHideEvent (WebView _webview)
		{
		Debug.Log("Dismissed web view from the screen.");
		}

		private void DidDestroyEvent (WebView _webview)
		{
		Debug.Log("Released web view instance.");
		}

		private void DidStartLoadEvent (WebView _webview)
		{
		Debug.Log("Started loading webpage contents.");
		Debug.Log(string.Format("URL: {0}.", _webview.URL));
		}

		private void DidFinishLoadEvent (WebView _webview)
		{
		Debug.Log("Finished loading webpage contents.");
		}

		private void DidFailLoadWithErrorEvent (WebView _webview, string _error)
		{
		Debug.Log("Failed to load requested contents.");
		Debug.Log(string.Format("Error: {0}.", _error));
		}

		private void DidFinishEvaluatingJavaScriptEvent (WebView _webview, string _result)
		{
		Debug.Log("Finished evaluating JavaScript script.");
		Debug.Log(string.Format("Result: {0}.", _result));
		}

		private void DidReceiveMessageEvent (WebView _webview,  WebViewMessage _message)
		{
		Debug.Log("Received a new message from web view.");
		Debug.Log(string.Format("Host: {0}.", 		_message.Host));
		Debug.Log(string.Format("Scheme: {0}.", 		_message.Scheme));
		Debug.Log(string.Format("URL: {0}.", 		_message.URL));
		Debug.Log(string.Format("Arguments: {0}.", 	_message.Arguments.ToJSON()));
			Debug.Log (_message.Host);
			Debug.Log (_message.Scheme);
			Debug.Log (_message.URL);
			Debug.Log (_message.Arguments.ToJSON().ToString());
		}

		#endregion


	private void PlayVideoFinished (ePlayVideoFinishReason _reason)
	{
		Debug.Log("Request to play video finished. Reason for finish is " + _reason + ".");
	}
	}