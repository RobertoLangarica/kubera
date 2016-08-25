using UnityEngine;
using System.Collections;

public class Stairs : MonoBehaviour {

	protected bool active;
	protected MapManager mapManager;
	public int toWorld;

	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();
	}

	public void animateStairs()
	{
		//TODO animate
		active = true;
	}

	public void onClick()
	{
		if(active)
		{			
			mapManager.changeCurrentWorld (toWorld,true,false);
		}
	}
}
