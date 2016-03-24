using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gems : MonoBehaviour {

	public Text gems;

	public void goToStore()
	{
		//mandamos a la tienda de kubera
	}

	public void actualizeGems(int gems)
	{
		this.gems.text = gems.ToString ();
	}
}
