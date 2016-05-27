using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FacebookNews : MonoBehaviour {

	public Button newsButton;
	public Text MessageCount;
	public GameObject panelMessages;
	public GridLayoutGroup panellMessageGrid;
	void Start()
	{
		panellMessageGrid.cellSize = new Vector2 (Screen.width * .4f, Screen.height / 8);
	}

	public void actualizeMessageNumber(string messageCount)
	{
		MessageCount.text = messageCount.ToString();
	}

	public void openMessages()
	{
		panelMessages.SetActive (!panelMessages.activeSelf);
	}
}
