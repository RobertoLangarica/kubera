using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems.sync;

public class ShopikaConnectPopUp : PopUpBase {

	public Text title;
	public Text Info;
	public Button exitBTN;

	void Start()
	{
		exitBTN.gameObject.SetActive (false);

		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_SHOPIKA_INFO);

		utils.gems.sync.GemsSyncManager.GetCastedInstance<utils.gems.sync.GemsSyncManager> ().OnDataRetrieved  += popUpCompleted;
		utils.gems.sync.GemsSyncManager.GetCastedInstance<utils.gems.sync.GemsSyncManager> ().OnDataRetrievedFailure += failure;
	}

	protected void failure()
	{
		title.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_SHOPIKA_TITLE);
		Info.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.CONNECTING_FAILURE_SHOPIKA_INFO);
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
}
