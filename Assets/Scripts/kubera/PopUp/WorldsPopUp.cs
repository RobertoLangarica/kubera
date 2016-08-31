using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldsPopUp : PopUpBase {

	public GridLayoutGroup grid;

	protected MapManager mapManager;
	protected MiniWorld[] worlds;
	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();
		worlds = FindObjectsOfType<MiniWorld> ();

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
	}

	public void initializeMiniWorlds(bool unLocked, string worldName, int starsObtained, int worldStars)
	{
		for(int i=0; i<worlds.Length; i++)
		{
			worlds [i].worldPopUp = this;
			if(unLocked)
			{
				worlds [i].name.text = worldName;
				worlds [i].setStars (starsObtained, worldStars);
			}
			else
			{
				worlds [i].blocked ();
			}
		}
	}
}
