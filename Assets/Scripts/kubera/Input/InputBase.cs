using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputBase : MonoBehaviour 
{
	private static List<GameObject> AllRayCasters;


	public static void registerRayCasters(GameObject[] casters)
	{
		if(AllRayCasters == null)
		{
			AllRayCasters = new List<GameObject>();
		}	

		AllRayCasters.AddRange(casters);
	}

	public static void clearRaycasters()
	{
		if(AllRayCasters != null)
		{
			AllRayCasters.Clear();
		}
	}

	public static void activateAllRayCasters(bool activate)
	{
		if(AllRayCasters == null){return;}
		int c = AllRayCasters.Count;

		for(int i = 0; i < c; i++)
		{
			AllRayCasters[i].SetActive(activate);
		}
	}
}