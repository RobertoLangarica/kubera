using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniWorld : MonoBehaviour {

	public Text name;
	public Text starsCount;
	public GameObject obtained;
	public GameObject imageBloqued;
	public Image starImage;

	public int world;

	public bool bloqued;

	[HideInInspector]public WorldsPopUp worldPopUp;

	void Start()
	{
		//name.text = MultiLanguageTextManager.instance.getTextByID ("World" + world + "_Name");
	}

	public void setStars(int obtainedStars, int worldStars)
	{
		print (world);
		starsCount.text = obtainedStars.ToString () + "/" + worldStars.ToString ();
	}

	public void toWorld()
	{
		worldPopUp.goToWorld (world);
	}

	public void blocked()
	{
		print (world);
		starsCount.text = "";
		name.text = "???";
		obtained.SetActive (false);
		imageBloqued.SetActive (true);
		starImage.enabled = false;
	}
}
