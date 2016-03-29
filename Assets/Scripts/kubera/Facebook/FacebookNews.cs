using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookNews : MonoBehaviour {

	public Button newsButton;
	public Text MessageCount;
	public GameObject panelMessages;

	public void actualizeMessageNumber(string messageCount)
	{
		MessageCount.text = messageCount.ToString();
	}

	public void openMessages()
	{
		panelMessages.SetActive (!panelMessages.activeSelf);
	}
}
