using System.Collections;
using UnityEngine;

public class FriendPicture :MonoBehaviour {

	public string fbId;
	protected Sprite picture;
	float requestAgainTime = 0.5f;

	public delegate void DOnPictureFound(Sprite picture);
	public DOnPictureFound OnFound;

	protected int attempts = 5;
	public Sprite getPicture()
	{
		picture =  FacebookPersistentData.GetInstance ().getSpritePictureById (fbId);
		if (picture == null) 
		{
			StartCoroutine ("findPicture");
		}
		return picture;
	}

	IEnumerator findPicture()
	{
		yield return new WaitForSeconds (requestAgainTime);
		picture = FacebookPersistentData.GetInstance ().getSpritePictureById (fbId);
		if(picture == null && attempts >0)
		{
			attempts--;
			findPicture ();
		}
		else if(attempts >0)
		{
			if(OnFound != null)
			{
				OnFound (picture);
			}
		}
	}
}

