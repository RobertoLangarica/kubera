using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;
using utils.gems;

public class KuberaShare : MonoBehaviour 
{
	private eShareOptions[]	m_excludedOptions	= new eShareOptions[0];

	public Action OnFinishedSharing;

	public void shareShopikaURL()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text 		= MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SHARE_DEFAULT_TEXT);
		_shareSheet.URL			= "http://shopika-store.cuatromedios.com/i/" + GemsManager.GetCastedInstance<GemsManager> ().currentUser.shareCode;
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		#if USES_SHARING
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
		#endif
	}

	private void FinishedSharing (eShareResult _result)
	{
		if (OnFinishedSharing != null) 
		{
			OnFinishedSharing ();
		}
	}
}
