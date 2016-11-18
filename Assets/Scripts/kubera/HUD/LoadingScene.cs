using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LoadingScene : MonoBehaviour 
{
	public Image backGround;

	public Image[] kuberaLoading;
	public Text[] textKuberaLoading;
	protected int current = 0;
	protected float size;

	public HintManager hints;

	void Start()
	{
		size = kuberaLoading [0].rectTransform.rect.size.x;

		for(int i=0; i<kuberaLoading.Length; i++)
		{
			textKuberaLoading [i] = kuberaLoading [i].transform.GetComponentInChildren<Text> ();
		}
	}

	public void showLoading(float duration,TweenCallback callBack = null)
	{
		backGround.gameObject.SetActive(true);
		animateKuberaLoading ();
		Tweener tween = backGround.DOFade (1,duration).SetId("backGround");

		if(callBack != null)
		{
			tween.OnComplete(callBack);
		}

		for(int i=0; i<kuberaLoading.Length; i++)
		{
			kuberaLoading[i].DOFade (1,duration);
			textKuberaLoading[i].DOFade (1,duration);
		}
		backGround.gameObject.SetActive (true);

		hints.changeText();
	}

	public void hideLoading(float duration)
	{
		backGround.DOFade (0,duration).OnComplete(()=>{backGround.gameObject.SetActive(false);});
		killAnimation (duration);
		current = 0;
		backGround.gameObject.SetActive (false);
	}

	protected void killAnimation(float duration)
	{
		DOTween.Kill ("kuberaLoading");
		for(int i=0; i<kuberaLoading.Length; i++)
		{
			kuberaLoading[i].DOFade (0,duration);
			textKuberaLoading[i].DOFade (0,duration);
			kuberaLoading [i].rectTransform.DOAnchorPos(Vector3.zero,0);
		}
	}

	public void animateKuberaLoading()
	{
		float temp = 0;

		for (int i = 0; i < kuberaLoading.Length; i++) 
		{
			temp = Random.Range (-45,45);

			kuberaLoading [i].transform.localRotation = Quaternion.Euler(new Vector3(0,0,temp));
		}
		/*if(current == kuberaLoading.Length)
		{
			current =0;
		}


		kuberaLoading[current].rectTransform.DOAnchorPos(new Vector2(0,size),0.25f).SetId("kuberaLoading").OnComplete(()=>
			{
				kuberaLoading[current].rectTransform.DOAnchorPos(new Vector2(0,0),0.25f).SetId("kuberaLoading");
				current++;
				animateKuberaLoading();
			});*/
	}
}