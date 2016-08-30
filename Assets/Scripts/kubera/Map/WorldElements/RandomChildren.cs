using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomChildren : MonoBehaviour {

	public List<GameObject> Children = new List<GameObject>();
	public List<GameObject> randomList = new List<GameObject>();

	public int maxElements = 0;
	public bool allChildren;
	void Start()
	{
		foreach (Transform child in transform)
		{
			Children.Add(child.gameObject);
		}

		if(allChildren)
		{
			maxElements = Children.Count;
		}

		int random;
		for(int i=0; i<maxElements; i++)
		{
			random = Random.Range (0, Children.Count);
			randomList.Add(Children [random]);
			randomList[i].AddComponent<RotateObject> ().startRotate ();
			//randomList [i].AddComponent<MoveInfinityObjects> ();
			Children.Remove (Children[random]);
		}
	}
}
