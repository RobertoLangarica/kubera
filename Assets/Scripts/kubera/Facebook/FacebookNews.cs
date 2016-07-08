using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookNews : MonoBehaviour {

	public Button newsButton;
	public Text messageCount;
	public Image messageCountImage;
	public GameObject messagesNewsBackground;
	public GridLayoutGroup panelMessageGridLayout;
	public RectTransform panelMessageGridRectTransform;

	void Start()
	{
		panelMessageGridLayout.cellSize = new Vector2 (panelMessageGridRectTransform.rect.width, Screen.height / 8);
		actualizeMessageNumber ("0");
	}

	public void actualizeMessageNumber(string messageCount)
	{
		print ("NADA");
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
		messagesNewsBackground.SetActive (!messagesNewsBackground.activeSelf);
		actualizeMessageNumber ("0");
	}
}
