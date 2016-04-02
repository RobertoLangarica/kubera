using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager : MonoBehaviour {

	public ScrollRect scrollRect;
	public GameObject panelPopUp;
	protected LifesHUDManager lifesHUDManager;

	void Start()
	{
		lifesHUDManager = FindObjectOfType<LifesHUDManager> ();
		lifesHUDManager.OnStopInput += stopInput;

	}

	protected void stopInput(bool stopScrollRect)
	{
		panelPopUp.SetActive (stopScrollRect);
	}

}
