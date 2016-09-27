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
	void Start()
	{
		lastPosition = stairs [stairs.Length - 1].rectTransform.anchoredPosition.x;
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
			mapManager.changeCurrentWorld (toWorld,true,false);
		}
	}
}
