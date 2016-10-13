using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookNews : PopUpBase {

	public MapManager mapManager;
	public Button newsButton;
	public Text messageCount;
	public Image messageCountImage;
	public GridLayoutGroup panelMessageGridLayout;
	public RectTransform panelMessageGridRectTransform;
	public Text title;

	void Start()
	{
		panelMessageGridLayout.cellSize = new Vector2 (panelMessageGridRectTransform.rect.width, Screen.height *0.175f);
		panelMessageGridLayout.spacing = new Vector2 (0,panelMessageGridRectTransform.rect.width*0.2f);
		actualizeMessageNumber ();

		//TODO hardcoding
		title.text = "MENSAJES";
	}

	public void actualizeMessageNumber(int messageCount = 0)
	{
		if(messageCount == 0)
		{
			messageCountImage.gameObject.SetActive(false);
		}
		else
		{
			messageCountImage.gameObject.SetActive (true);
			if(messageCount > 9)
			{				
				this.messageCount.text = "+9";
			}
			else
			{
				this.messageCount.text = messageCount.ToString();
			}
		}
	}

	public void openMessages()
	{
		activate ();
		mapManager.openPopUp ("facebookNews");
		actualizeMessageNumber ();
	}

	public void exit()
	{
		OnComplete ();
	}

	public void toWorlds()
	{
		OnComplete ("toWorldTraveler");
	}
}
