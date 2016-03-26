using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour {

	public GameObject playerInfo;
	public Image image;
	public Text text;

	public void showPlayerName(string name)
	{
		text.text = name;
	}

	public void showPlayerPicture(Texture pictureTexture)
	{	
		Texture2D picture = pictureTexture as Texture2D;
		showPlayerPicture (picture);
	}

	public void showPlayerPicture(Texture2D pictureTexture2D)
	{
		Sprite picture = Sprite.Create (pictureTexture2D, new Rect (0, 0, pictureTexture2D.width, pictureTexture2D.height), new Vector2 (0, 0));
		showPlayerPicture (picture);
	}

	public void showPlayerPicture(Sprite picture)
	{
		image.sprite = picture;
	}

	public void showPlayerInfo(string name, Texture picture)
	{
		showPlayerName (name);
		showPlayerPicture (picture);
	}
}
