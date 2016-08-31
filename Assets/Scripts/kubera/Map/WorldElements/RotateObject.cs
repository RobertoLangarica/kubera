using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour {
	protected bool rotate;
	protected float randomSpeed;

	void Update()
	{
		if(rotate)
		{
			this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x,this.transform.eulerAngles.y,this.transform.eulerAngles.z+randomSpeed*Time.smoothDeltaTime);
		}
	}

	public void startRotate ()
	{
		rotate = true;
		randomSpeed = Random.Range (50, 100);

		if(Random.Range(0,2) == 0)
		{
			randomSpeed *= -1;
		}
	}
}
