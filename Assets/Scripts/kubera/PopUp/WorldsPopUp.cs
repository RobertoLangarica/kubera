using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldsPopUp : PopUpBase {

	public GridLayoutGroup grid;

	protected MapManager mapManager;
	public MiniWorld[] worlds;
	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();

		grid.cellSize = new Vector2 (Screen.width * 0.8f, Screen.height * 0.8f);
		grid.padding.left = (int)(Screen.height * 0.08f);
		grid.padding.right = (int)(Screen.width * 0.08f);
		grid.spacing = new Vector2(Screen.height * 0.08f,0);
	}

	public override void activate()
	{
		popUp.SetActive (true);
	}

	public void goToWorld(int world)
	{
		mapManager.changeCurrentWorld (world, true, false);
		OnComplete ();
	}

	public void exit()
	{
		OnComplete ();
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
		OnComplete ("toFacebookMessages");
	}
}
