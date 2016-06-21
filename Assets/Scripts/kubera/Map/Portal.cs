using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

	protected MapManager mapManager;

	void Start()
	{
		mapManager = FindObjectOfType<MapManager> ();
	}

	public void OnClick(int world)
	{
		mapManager.changeCurrentWorld (world);
	}
}
