using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using utils.gems;

public class Gems : MonoBehaviour {

	public Text gems;
	public KuberaWebView webView;

	public void goToStore()
	{
		//mandamos a la tienda de kubera
		webView.showShopikaAndRegisterForEvents();
	}

	void Start()
	{
		GemsManager.GetCastedInstance<GemsManager>().OnGemsUpdated += actualizeGems;

		actualizeGems(GemsManager.GetCastedInstance<GemsManager>().currentGems);
	}

	void OnDestroy()
	{
		GemsManager.GetCastedInstance<GemsManager>().OnGemsUpdated -= actualizeGems;
	}

	public void actualizeGems(int gems)
	{
		this.gems.text = gems.ToString ();
	}
}
