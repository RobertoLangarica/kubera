using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	protected MapManager mapManager;
	public int toWorld;

	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();
	}

	public void onClick()
	{
		mapManager.changeCurrentWorld (toWorld,false,true);
	}
}
