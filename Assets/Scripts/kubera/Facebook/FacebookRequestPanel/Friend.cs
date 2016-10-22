using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Friend : MonoBehaviour {

	public string id;
	public Image friendImage;
	public Image requestFriend;

	public Text userName;
	public bool selected;
	public bool imageSetted;

	public delegate void DOnActivateToggle(bool activated);
	public DOnActivateToggle OnActivated;

	public Sprite selectedImage;
	public Sprite notSelectedImage;

	public void activateSelected(bool activated)
	{
		selected = activated;
		changeSprite ();
		OnActivated (selected);
	}

	public void activateSelected()
	{
		selected = !selected;
		changeSprite ();
		OnActivated (selected);
	}

	public void changeSprite()
	{
		if(selected)
		{
			requestFriend.sprite = selectedImage;
		}
		else
		{
			requestFriend.sprite = notSelectedImage;
		}
	}

	public void setFriendImage(Texture image)
	{	
		Texture2D tPicture = image as Texture2D;
		setFriendImage (tPicture);
	}

	public void setFriendImage(Texture2D picture)
	{
		Sprite sPicture = Sprite.Create (picture, new Rect (0, 0, picture.width, picture.height), new Vector2 (0, 0));
		setFriendImage (sPicture);
	}

	public void setFriendImage(Sprite picture)
	{
		friendImage.sprite = picture;
	}

	public void getTextureFromURL (string url)
	{
		GraphUtil.LoadImgFromURL(url, delegate(Texture pictureTexture)
			{
				if (pictureTexture != null)
				{
					FacebookPersistentData.GetInstance().addFriendImage(id,pictureTexture);
					setFriendImage(pictureTexture);
					imageSetted = true;
				}
			});
	}
}
