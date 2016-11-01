using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kubera.Data.Sync;

public class FacebookLoadingConnectPopUp : PopUpBase {

	public Text title;
	public Text Info;
	public Button exitBTN;

	void Start()
	{
		exitBTN.gameObject.SetActive (false);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_INFO);

		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved  += popUpCompleted;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrievedFailure += failure;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnLoginFailure += failure;

	}

	public override void activate()
	{
		popUp.SetActive (true);
		exitBTN.gameObject.SetActive (false);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_INFO);
	}

	protected void failure()
	{
		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_FACEBOOK_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_FACEBOOK_INFO);
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

	void OnDestroy()
	{
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved  -= popUpCompleted;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrievedFailure -= failure;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnLoginFailure -= failure;
	}
}
