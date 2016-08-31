using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniWorld : MonoBehaviour {

	public Text name;
	public Text starsCount;
	public GameObject obtained;
	public GameObject imageBloqued;

	public int world;

	public bool bloqued;

	[HideInInspector]public WorldsPopUp worldPopUp;

	void Start()
	{
		//name.text = MultiLanguageTextManager.instance.getTextByID ("World" + world + "_Name");
	}

	public void setStars(int obtainedStars, int worldStars)
	{
		starsCount.text = obtainedStars.ToString () + "/" + worldStars.ToString ();
	}

	public void toWorld()
	{
		worldPopUp.goToWorld (world);
	}

	public void blocked()
	{
		starsCount.text = "";
		name.text = "???";
		obtained.SetActive (false);
		imageBloqued.SetActive (true);
	}
}
