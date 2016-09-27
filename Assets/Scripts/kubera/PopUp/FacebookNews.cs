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

	void Start()
	{
		panelMessageGridLayout.cellSize = new Vector2 (panelMessageGridRectTransform.rect.width, Screen.height *0.175f);
		panelMessageGridLayout.spacing = new Vector2 (0,panelMessageGridRectTransform.rect.width*0.2f);
		actualizeMessageNumber ("0");
	}

	public void actualizeMessageNumber(string messageCount)
	{
		if(messageCount == "0")
		{
			messageCountImage.gameObject.SetActive(false);
		}
		else
		{			
			messageCountImage.gameObject.SetActive (true);
			this.messageCount.text = messageCount;
		}
	}

	public void openMessages()
	{
		activate ();
		mapManager.openPopUp ("facebookNews");
		actualizeMessageNumber ("0");
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
