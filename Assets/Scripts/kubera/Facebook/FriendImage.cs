using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FriendImage : MonoBehaviour {

	public Image friendImage;
	//public string id;

	public void setFriendPicture(Texture picture)
	{	
		Texture2D tPicture = picture as Texture2D;
		setFriendPicture (tPicture);
	}

	public void setFriendPicture(Texture2D picture)
	{
		Sprite sPicture = Sprite.Create (picture, new Rect (0, 0, picture.width, picture.height), new Vector2 (0, 0));
		setFriendPicture (sPicture);
	}

	public void setFriendPicture(Sprite picture)
	{
		friendImage.sprite = picture;
	}

	public void setFriendImage(Texture picture)
	{
		setFriendPicture (picture);
	} 
}
