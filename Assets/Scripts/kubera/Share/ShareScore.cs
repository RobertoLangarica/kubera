using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class ShareScore : MonoBehaviour
{
	private eShareOptions[]	m_excludedOptions	= new eShareOptions[0];
	public Action OnFinishedSharing;


	public void sharePassedLevel(string points, int stars, String level)
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
	
		_shareSheet.Text 		= MultiLanguageTextManager.instance.multipleReplace(MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SHARE_LEVEL_PASSED_TEXT),
			new string[4]{ "{{pointsObtained}}" ,"{{starsObtained}}", "{{levelNumber}}" ,"/n" }, new string[4]{points, stars.ToString(), level,  Environment.NewLine });
		_shareSheet.URL			= "https://www.villavanilla.com/juegos/kubera";
		_shareSheet.AttachScreenShot();
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

