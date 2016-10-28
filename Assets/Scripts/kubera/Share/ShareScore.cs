using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class ShareScore : MonoBehaviour
{
	private eShareOptions[]	m_excludedOptions	= new eShareOptions[0];
	public Action OnFinishedSharing;


	public void sharePassedLevel(String level)
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text 		= MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SHARE_DEFAULT_TEXT);
		_shareSheet.URL			= "";
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

