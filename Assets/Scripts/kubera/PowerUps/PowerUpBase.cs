using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerUpBase : MonoBehaviour {

	public int uses;

	public GameObject powerOne;
	public Text numberUses;
	public GameObject imageUses;

	// Use this for initialization
	void Start () {
		numberUses.text = uses.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PowerUsed()
	{
		if(uses !=0)
		{
			uses--;
			numberUses.text = uses.ToString ();
			//hacerlo no interactuable
			if(uses==0)
			{
				imageUses.SetActive(false);
				gameObject.GetComponent<Button>().interactable = false;
			}
		}
	}

	public void oneTilePower()
	{
		if (uses != 0) 
		{
			Vector3 pos;

			GameObject go = (GameObject)Instantiate (powerOne,Camera.main.ScreenToWorldPoint(Input.mousePosition),Quaternion.identity);

			go.name = powerOne.name;
			go.GetComponent<Piece> ().powerUp = true;
			FindObjectOfType<InputGameController> ().activePowerUp (go);
		}
	}
}
