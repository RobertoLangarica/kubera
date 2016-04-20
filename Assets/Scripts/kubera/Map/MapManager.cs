﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager : MonoBehaviour {

	public ScrollRect scrollRect;
	public GameObject modal;
	protected LifesManager lifesHUDManager;
	protected PopUpManager popUpManager;

	void Start()
	{
		popUpManager = FindObjectOfType<PopUpManager> ();
		lifesHUDManager = FindObjectOfType<LifesManager> ();


		popUpManager.OnPopUpCompleted = OnPopupCompleted;
	}

	protected void stopInput(bool stopInput)
	{
		modal.SetActive (stopInput);
	}

	public void openPopUp(string popUpName)
	{
		popUpManager.activatePopUp (popUpName);
		stopInput (true);
	}

	private void OnPopupCompleted(string action ="")
	{
		stopInput(false);
	}
}
