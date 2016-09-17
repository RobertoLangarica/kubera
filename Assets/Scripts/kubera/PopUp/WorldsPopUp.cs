﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldsPopUp : PopUpBase {

	public GridLayoutGroup grid;

	protected MapManager mapManager;

	public GameObject rightArrow;
	public GameObject leftArrow;

	public ScrollRect scrollRect;

	public MiniWorld[] worlds;
	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();

		grid.cellSize = new Vector2 (Screen.width * 0.9f, Screen.height * 0.8f);
		grid.padding.left = (int)(Screen.width * 0.06f);
		grid.padding.right = (int)(Screen.width * 0.06f);
		grid.spacing = new Vector2(Screen.height * 0.08f,0);
	}

	public override void activate()
	{
		animateWorldsLights (true);
		popUp.SetActive (true);
	}

	public void goToWorld(int world)
	{
		mapManager.changeCurrentWorld (world, true, false);
		CompletePopUp ();
	}

	public void exit()
	{
		CompletePopUp ();
	}

	public void initializeMiniWorld(int world, bool unLocked, int starsObtained, int worldStars)
	{
		worlds [world].worldPopUp = this;
		if(unLocked)
		{
			worlds [world].setStars (starsObtained, worldStars);
		}
		else
		{
			worlds [world].blocked ();
		}
	}

	public void toMessages()
	{
		CompletePopUp ("toFacebookMessages");
	}

	protected void animateWorldsLights(bool animate)
	{
		for(int i=0; i<worlds.Length; i++)
		{
			if(animate)
			{
				worlds [i].animateLights ();
			}
			else
			{
				worlds [i].killAnimateLights ();
			}
		}
	}

	protected void CompletePopUp(string action= "")
	{
		animateWorldsLights (false);
		OnComplete (action);
	}

	public void checkPosition()
	{
		if(scrollRect.normalizedPosition.x < 0)
		{
			leftArrow.SetActive (false);
		}
		else
		{
			leftArrow.SetActive (true);
		}

		if(scrollRect.normalizedPosition.x > 1)
		{
			rightArrow.SetActive (false);
		}
		else
		{
			rightArrow.SetActive (true);
		}
	}
}
