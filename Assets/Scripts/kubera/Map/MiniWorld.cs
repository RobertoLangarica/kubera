using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniWorld : MonoBehaviour {

	public Text name;
	public Text starsCount;
	public GameObject obtained;
	public GameObject imageBloqued;
	public Image starImage;

	public int world;

	public bool bloqued;

	[HideInInspector]public WorldsPopUp worldPopUp;

	public Transform[] lights;

	void Start()
	{
	}

	public void setStars(int obtainedStars, int worldStars)
	{
		starsCount.text = obtainedStars.ToString () + "/" + worldStars.ToString ();

		name.text = MultiLanguageTextManager.instance.getTextByID ("World" + world + "_name");
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
		starImage.enabled = false;
	}

	public void animateLights()
	{
		float randomRotation = Random.Range (180f, 360f);
		for(int i=0; i<lights.Length; i++)
		{
			lights [i].DORotate (new Vector3 (0, 0, lights [i].rotation.z * randomRotation), Random.Range (1, 3), RotateMode.FastBeyond360).SetId(lights[i]);
		}
		Invoke("animateLights",Random.Range(3,5));
	}

	public void killAnimateLights()
	{
		CancelInvoke ();
		for(int i=0; i<lights.Length; i++)
		{
			DOTween.Kill (lights [i], true);
		}
	}
}
