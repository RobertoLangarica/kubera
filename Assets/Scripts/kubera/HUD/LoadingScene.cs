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

	void Start()
	{
		size = kuberaLoading [0].rectTransform.rect.size.x;

		for(int i=0; i<kuberaLoading.Length; i++)
		{
			print (kuberaLoading [i].GetComponent<Text> ());
			textKuberaLoading [i] = kuberaLoading [i].transform.GetComponentInChildren<Text> ();
		}
	}

	public void showLoading(float duration,TweenCallback callBack)
	{
		animateKuberaLoading ();
		backGround.DOFade (1,duration).SetId("backGround").OnComplete(callBack);
		for(int i=0; i<kuberaLoading.Length; i++)
		{
			kuberaLoading[i].DOFade (1,duration);
			textKuberaLoading[i].DOFade (1,duration);
		}
	}

	public void hideLoading(float duration)
	{
		backGround.DOFade (0,duration);
		killAnimation (duration);
		current = 0;
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
		if(current == kuberaLoading.Length)
		{
			current =0;
		}


		kuberaLoading[current].rectTransform.DOAnchorPos(new Vector2(0,size),0.25f).SetId("kuberaLoading").OnComplete(()=>
			{
				kuberaLoading[current].rectTransform.DOAnchorPos(new Vector2(0,0),0.25f).SetId("kuberaLoading");
				current++;
				animateKuberaLoading();
			});
	}
}