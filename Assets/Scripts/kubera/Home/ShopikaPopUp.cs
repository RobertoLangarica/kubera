using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopikaPopUp : MonoBehaviour {

	public KuberaWebView kuberaWebView;

	public GameObject popUp;
	public Text conectButtonText;
	public Text inviteFriendsButtonText;
	public KuberaShare share;

	void Start()
	{
		conectButtonText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECT_SHOPIKA_POPUP);
		inviteFriendsButtonText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.INVITE_FRIENDS_SHOPIKA_POPUP);

		share.OnFinishedSharing += exit;
	}

	public void exit()
	{
		popUp.SetActive (false);
	}

	public void conectShopika()
	{
		//TODO hacer logout
		kuberaWebView.showShopikaAndRegisterForEvents ();
	}

	public void inviteFriends()
	{
		share.shareShopikaURL ();
	}
}
