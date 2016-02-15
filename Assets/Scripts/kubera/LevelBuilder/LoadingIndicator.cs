using UnityEngine;
using System.Collections;

public class LoadingIndicator : MonoBehaviour {

	public float rotationSpeed = 40.0f;
	public GameObject indicator;

	void Update () 
	{
		Vector3 rot = new Vector3(0,0,rotationSpeed*Time.deltaTime);
		indicator.transform.Rotate(rot);
	}
}
