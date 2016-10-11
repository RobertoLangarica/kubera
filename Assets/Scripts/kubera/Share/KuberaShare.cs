using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class KuberaShare : MonoBehaviour 
{
	private 	eShareOptions[]	m_excludedOptions	= new eShareOptions[0];

	public void shareShopikaURL()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text		= "Algo";
		_shareSheet.URL			= "https://www.youtube.com/watch?v=J1Rd7zrvW7k";
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void FinishedSharing (eShareResult _result)
	{
		Debug.Log("Finished sharing");
	}
}
