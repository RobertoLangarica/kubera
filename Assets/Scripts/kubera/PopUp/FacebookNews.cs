using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kubera.Data.Sync;

public class FacebookNews : PopUpBase {

	public MapManager mapManager;
	public Button newsButton;
	public Text messageCount;
	public Image messageCountImage;
	public GridLayoutGroup panelMessageGridLayout;
	public RectTransform panelMessageGridRectTransform;
	public Text title;
	public GameObject noMessagesMessage;

	void Start()
	{
		panelMessageGridLayout.cellSize = new Vector2 (panelMessageGridRectTransform.rect.width, Screen.height *0.175f);
		panelMessageGridLayout.spacing = new Vector2 (0,panelMessageGridRectTransform.rect.width*0.2f);

		panelMessageGridLayout.padding.top = (int)(panelMessageGridLayout.cellSize.y *.5f);
		actualizeMessageNumber ();

		//TODO hardcoding
		title.text = "MENSAJES";
	}

	public void actualizeMessageNumber(int messageCount = 0)
	{
		if(messageCount == 0)
		{
			messageCountImage.gameObject.SetActive(false);
			if(KuberaSyncManger.GetCastedInstance<KuberaSyncManger>().facebookProvider.isLoggedIn)
			{
				noMessagesMessage.SetActive (true);
			}
			else
			{
				noMessagesMessage.SetActive (false);
			}
		}
		else
		{
			messageCountImage.gameObject.SetActive (true);
			noMessagesMessage.SetActive (false);
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
		messageCountImage.gameObject.SetActive(false);
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
