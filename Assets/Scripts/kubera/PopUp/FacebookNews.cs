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

	public int currentMessages;
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
			currentMessages = messageCount;
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
				//print (11);
				mapManager.worldsPopUp.toMessages ();
			} 
			else 
			{
				//print (22);
				mapManager.openPopUp ("facebookNews");
				messageCountImage.gameObject.SetActive (false);
			}
		}
	}

	public void exit()
	{
		mapManager.OnClosePopUp -= exit;
		CompletePopUp ();
	}

	public void toWorlds()
	{
		mapManager.OnClosePopUp -= exit;
		CompletePopUp ("toWorldTraveler",false);
	}

	public override void activate()
	{
		mapManager.OnClosePopUp += exit;
		if (mapButton.parent != popUpRect.parent) 
		{
			popUpRect.anchoredPosition = new Vector2 (-Screen.width * 0.85f, 0);

			popUp.SetActive (true);

			popUpRect.DOAnchorPos (Vector2.zero, 0.5f, true);

			mapButton.DOAnchorPos (new Vector2 (Screen.width * 0.78f, 0), 0.5f, true);
			mapButton.SetParent (popUpRect.parent);

			facebookMessagesButton.DOAnchorPos (new Vector2 (Screen.width * 0.85f, 0), 0.5f, true);
			facebookMessagesButton.SetParent (popUpRect.parent);

			popUpRect.SetSiblingIndex (popUpRect.parent.childCount - 1);
		} 
		else 
		{
			popUpRect.anchoredPosition = Vector2.zero;

			popUp.SetActive (true);

			mapButton.DOAnchorPos (new Vector2 (Screen.width * 0.78f, 0), 0.5f, true);
			facebookMessagesButton.DOAnchorPos (new Vector2 (Screen.width * 0.85f, 0), 0.5f, true);

			popUpRect.SetSiblingIndex (popUpRect.parent.childCount - 1);
		}
	}

	protected void CompletePopUp(string action= "",bool withAnim = true)
	{
		if (withAnim) 
		{
			popUpRect.DOAnchorPos (new Vector2 (-Screen.width * 0.85f, 0), 0.5f, true).OnComplete (() => {
				OnComplete (action);
			});

			mapButton.DOAnchorPos (Vector2.zero, 0.5f, true);
			mapButton.SetParent (previousParent);

			facebookMessagesButton.DOAnchorPos (Vector2.zero, 0.5f, true);
			facebookMessagesButton.SetParent (previousParent);
		} 
		else 
		{
			OnComplete (action);
			facebookMessagesButton.DOAnchorPos (new Vector2(Screen.width * 0.78f,0),0.5f,true);
		}
	}

	public void updateCurrentMessages(int messagesProcessed)
	{
		currentMessages -= messagesProcessed;

		if(currentMessages == 0)
		{
			messageCountImage.gameObject.SetActive(false);
			noMessagesMessage.SetActive (true);
		}
	}
}