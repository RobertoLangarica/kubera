using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class WorldsPopUp : PopUpBase {

	public GridLayoutGroup grid;

	public MapManager mapManager;

	public RectTransform mapButton;
	public RectTransform facebookMessagesButton;
	public RectTransform popUpRect;

	public ScrollRect scrollRect;

	public MiniWorld[] worlds;

	protected Transform previousParent;

	void Start()
	{
		grid.cellSize = new Vector2 (Screen.width * 0.75f, Screen.height * 0.8f);
		grid.spacing = new Vector2(0,Screen.height * 0.2f);
	}

	public override void activate()
	{
		if (mapButton.parent != transform.parent) 
		{
			popUpRect.anchoredPosition = new Vector2 (-Screen.width * 0.85f, 0);

			//animateWorldsLights (true);
			popUp.SetActive (true);

			popUpRect.DOAnchorPos (Vector2.zero, 0.5f, true);

			previousParent = mapButton.parent;

			mapButton.DOAnchorPos (new Vector2 (Screen.width * 0.85f, 0), 0.5f, true);
			mapButton.SetParent (transform.parent);

			facebookMessagesButton.DOAnchorPos (new Vector2 (Screen.width * 0.78f, -Screen.height * 0.5f), 0.5f, true);
			facebookMessagesButton.SetParent (transform.parent);

			transform.SetSiblingIndex (transform.parent.childCount - 1);
		} 
		else 
		{
			popUpRect.anchoredPosition = Vector2.zero;

			popUp.SetActive (true);

			previousParent = mapButton.parent;

			mapButton.DOAnchorPos (new Vector2 (Screen.width * 0.85f, 0), 0.5f, true);
			facebookMessagesButton.DOAnchorPos (new Vector2 (Screen.width * 0.78f, -Screen.height * 0.5f), 0.5f, true);

			transform.SetSiblingIndex (transform.parent.childCount - 1);
		}
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
		if(world >= worlds.Length)
		{
			return;
		}

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
		CompletePopUp ("toFacebookMessages",false);
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

			facebookMessagesButton.DOAnchorPos (new Vector2(Screen.width * 0.85f,0),0.5f,true);
		}
	}
}
