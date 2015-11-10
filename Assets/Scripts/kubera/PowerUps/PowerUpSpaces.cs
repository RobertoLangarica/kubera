using UnityEngine;
using System.Collections;

public class PowerUpSpaces : MonoBehaviour {

	public Transform[] powerUpPosses;

	public GameObject powerOne;

	public int powerUses = 3;
	// Use this for initialization
	void Start () {
		for(int i = 0; i<3; i++)
		{
			GameObject go = Instantiate(powerOne);
			go.name = powerOne.name;
			go.transform.position= new Vector3(powerUpPosses [i].position.x,powerUpPosses [i].position.y,1);
			go.GetComponent<Piece>().powerUp = true;
			go.GetComponent<Piece>().myFirstPos = powerUpPosses[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
