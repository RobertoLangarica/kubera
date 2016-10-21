using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kubera.Data.Sync;
using DG.Tweening;

public class FacebookNews : PopUpBase {

	public MapManager mapManager;
	public Button newsButton;
	public Text messageCount;
	public Image messageCountImage;
	public GridLayoutGroup panelMessageGridLayout;
	public RectTransform panelMessageGridRectTransform;
	public Text title;
	public GameObject noMessagesMessage;
	public Transform previousParent;

	public RectTransform mapButton;
	public RectTransform facebookMessagesButton;
	public RectTransform popUpRect;

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
		if (popUpRect.gameObject.activeSelf) 
		{
			exit ();
		} 
		else 
		{
			if (mapManager.worldsPopUp.gameObject.activeSelf) 
			{
				mapManager.worldsPopUp.toMessages ();
			} 
			else 
			{
				activate ();
				mapManager.openPopUp ("facebookNews");
				messageCountImage.gameObject.SetActive (false);
			}
		}
	}

	public void exit()
	{
		CompletePopUp ();
	}

	public void toWorlds()
	{
		CompletePopUp ("toWorldTraveler");
	}

	public override void activate()
	{
		popUpRect.anchoredPosition = new Vector2 (-Screen.width*0.85f,0);

		popUp.SetActive (true);

		popUpRect.DOAnchorPos (Vector2.zero,0.5f,true);

		mapButton.DOAnchorPos (new Vector2 (Screen.width * 0.78f, 0), 0.5f, true);
		mapButton.SetParent (popUpRect.parent);

		facebookMessagesButton.DOAnchorPos (new Vector2(Screen.width * 0.85f,0),0.5f,true);
		facebookMessagesButton.SetParent (popUpRect.parent);

		popUpRect.SetSiblingIndex(popUpRect.parent.childCount-1);
	}

	protected void CompletePopUp(string action= "")
	{
		popUpRect.DOAnchorPos (new Vector2 (-Screen.width * 0.85f, 0), 0.5f, true).OnComplete (()=>{OnComplete (action);});

		mapButton.DOAnchorPos (Vector2.zero, 0.5f, true);
		mapButton.SetParent (previousParent);

		facebookMessagesButton.DOAnchorPos (Vector2.zero,0.5f,true);
		facebookMessagesButton.SetParent (previousParent);
	}
}