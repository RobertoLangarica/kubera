using UnityEngine;
using System.Collections;

public class Stairs : MonoBehaviour {

	protected MapManager mapManager;
	public int toWorld;

	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();
	}

	public void animateStairs()
	{
		//TODO animate
	}

	public void onClick()
	{
		mapManager.changeCurrentWorld (toWorld,true,false);
	}
}
