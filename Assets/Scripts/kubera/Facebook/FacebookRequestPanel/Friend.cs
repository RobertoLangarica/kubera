using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Friend : MonoBehaviour {

	public string id;
	public Image friendImage;
	public Text userName;
	public Toggle selected;

	public delegate void DOnActivateToggle(bool activated);
	public DOnActivateToggle OnActivated;

	public void activateSelected(bool activate)
	{
		selected.isOn = activate;
	}

	public void activateSelected()
	{
		bool activated = selected.isOn;

		selected.isOn = activated;
		OnActivated (selected.isOn);
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
				}
			});
	}
}
