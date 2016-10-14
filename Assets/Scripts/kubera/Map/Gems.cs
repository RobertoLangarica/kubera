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
		ShopikaManager.GetCastedInstance<ShopikaManager>().OnGemsUpdated += actualizeGems;

		actualizeGems(ShopikaManager.GetCastedInstance<ShopikaManager>().currentGems);
	}

	void OnDestroy()
	{
		ShopikaManager.GetCastedInstance<ShopikaManager>().OnGemsUpdated -= actualizeGems;
	}

	public void actualizeGems(int gems)
	{
		this.gems.text = gems.ToString ();
	}
}
