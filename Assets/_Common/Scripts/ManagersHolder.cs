using UnityEngine;
using System.Collections;

public class ManagersHolder : MonoBehaviour 
{
	private bool checkDone = false;
	private float timeToCheckForLife = 3.0f;

	void Update () 
	{
		if(!checkDone)
		{
			timeToCheckForLife -= Time.deltaTime;
			if(timeToCheckForLife <= 0)
			{
				timeToCheckForLife = 0.0f;
				checkDone = true;
				if(transform.childCount == 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
