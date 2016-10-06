using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Stairs : MonoBehaviour {

	protected bool active;
	public MapManager mapManager;
	public int toWorld;

	public Image[] stairs;
	public float lastPosition;

	public Transform friendFirstPosition;
	public Transform friendLastPosition;
	public Image outOfOrder;
	public ParticleSystem starsParticleSystem;

	void Start()
	{
		lastPosition = stairs [stairs.Length - 1].rectTransform.anchoredPosition.x;
		outOfOrder.gameObject.SetActive (false);

		Invoke ("activateOutOfOrder", 0.6f);
	}

	void Update()
	{
		if (Input.GetKey (KeyCode.C))
		{
			animateStairs ();
		}

		if(!active)
		{
			return;
		}
		
		for(int i=0; i<stairs.Length; i++)
		{
			stairs [i].rectTransform.anchoredPosition = new Vector2 (stairs [i].rectTransform.anchoredPosition.x + 1, stairs [i].rectTransform.anchoredPosition.y + 1);
			if(stairs[i].rectTransform.anchoredPosition.x >= lastPosition)
			{
				stairs [i].rectTransform.anchoredPosition = Vector2.zero;
			}
		}
	}

	public void activateOutOfOrder()
	{
		if(!active)
		{
			outOfOrder.gameObject.SetActive (true);
		}
	}

	public void animateStairs()
	{
		active = true;
	}

	protected void animate(Image stair)
	{
		stair.rectTransform.anchoredPosition = Vector2.zero;

		stair.rectTransform.DOAnchorPos (new Vector2(320,320),1);

	}

	public void onClick()
	{
		if(active)
		{			
			if(friendLastPosition != null && mapManager.isInLastLevelWorld)
			{
				DOTween.Kill ("animateWaiting");
				mapManager.getCurrentLevel ().facebookBackground.transform.DOLocalMove (friendLastPosition.localPosition, 3.0f).OnComplete(()=>
					{
						mapManager.changeCurrentWorld (toWorld,true,false);
					}).SetId("onClick").SetEase(Ease.Linear);
			}
			else
			{
				mapManager.changeCurrentWorld (toWorld,true,false);
			}
		}
	}

	public void animateToWait()
	{
		if(friendLastPosition == null)
		{
			return;
		}

		starsParticleSystem.Play ();
		outOfOrder.GetComponentInChildren<Text> ().DOFade (0,1);
		outOfOrder.DOFade (0, 1).OnComplete(()=>
			{
				mapManager.getCurrentLevel ().facebookBackground.transform.DOLocalMove (friendFirstPosition.localPosition, 1.0f).OnComplete(()=>
					{
						active = true;
						onClick();
					}).SetId("animateToWait");
			}
		);
	}

	public void animateWaiting()
	{
		if(friendLastPosition == null)
		{
			return;
		}

		mapManager.getCurrentLevel ().facebookBackground.transform.DOShakePosition (1,4).OnComplete(()=>
			{
				animateWaiting();
			}).SetId("animateWaiting");

	}
}
