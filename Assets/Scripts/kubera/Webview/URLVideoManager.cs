using UnityEngine;
using System;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

public class URLVideoManager : Manager<URLVideoManager>
{
	public Action<ePlayVideoFinishReason> OnVideoFinished;

	public void playVideoFromURL(string nURL)
	{
		playVideoFromURL (URL.URLWithString (nURL));
	}

	public void playVideoFromURL(URL _url)
	{
		#if USES_MEDIA_LIBRARY
		NPBinding.MediaLibrary.PlayVideoFromURL(_url, PlayVideoFinished);
		#endif
	}

	private void PlayVideoFinished (ePlayVideoFinishReason reason)
	{
		//Debug.Log("Request to play video finished. Reason for finish is " + reason + ".");

		if (OnVideoFinished != null) 
		{
			OnVideoFinished (reason);
		}
	}
}