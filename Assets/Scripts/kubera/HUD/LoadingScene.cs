using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LoadingScene : MonoBehaviour 
{
	public Image backGround;

	public void showLoading(float duration,TweenCallback callBack)
	{
		backGround.DOFade (1,duration).SetId("backGround").OnComplete(callBack);
	}

	public void hideLoading(float duration)
	{
		backGround.DOFade (0,duration);
	}
}