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


	}

	public override void activate()
	{
		popUp.SetActive (true);
		exitBTN.gameObject.SetActive (false);

		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved  += popUpCompleted;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrievedFailure += failure;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnLoginFailure += failure;

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FACEBOOK_INFO);
	}

	protected void failure()
	{
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved  -= popUpCompleted;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrievedFailure -= failure;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnLoginFailure -= failure;

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
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrieved  -= popUpCompleted;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnDataRetrievedFailure -= failure;
		KuberaSyncManger.GetCastedInstance<KuberaSyncManger> ().OnLoginFailure -= failure;

		popUp.SetActive (false);
		OnComplete ("");
	}
}
