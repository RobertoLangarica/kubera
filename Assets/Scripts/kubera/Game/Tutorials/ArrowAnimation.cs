using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ArrowAnimation : MonoBehaviour 
{

	public float minAlpha = 0.5f;
	public float animStep = 0.02f;

	public float beatTime = 1;
	public float beatStrength = 0.1f;
	public int beatVibrato = 1;
	public float beatRandom = 0;

	public GameObject arrowHead;
	public GameObject arrowBody;
	public GameObject arrowGlow;

	protected bool isDescending;
	protected bool onAnim;

	public void startAnimation()
	{
		onAnim = true;

		Beat ();
	}

	public void stopAnimation()
	{
		onAnim = false;
	}

	void Update()
	{
		if (onAnim) 
		{
			if (isDescending) 
			{
				if (getModifiedAlpha(-animStep) <= minAlpha) 
				{
					isDescending = false;
				}
			} 
			else 
			{

				if (getModifiedAlpha(animStep) >= 1) 
				{
					isDescending = true;
				}
			}
		}
		
	}

	protected float getModifiedAlpha(float value,bool reset = false)
	{
		Image tempImg = arrowGlow.GetComponent<Image>();

		Color tempCol = Color.white;

		if (!reset) 
		{
			tempCol = new Color (tempImg.color.r, tempImg.color.g, tempImg.color.b, tempImg.color.a + value);
		} 		

		tempImg.color = tempCol;

		return tempCol.a;
	}

	protected void Beat()
	{
		arrowHead.transform.DOShakeScale (beatTime, beatStrength, beatVibrato, beatRandom);
		arrowBody.transform.DOShakeScale (beatTime, beatStrength, beatVibrato, beatRandom).OnComplete (
			() => {
				if (onAnim) {
					Invoke ("Beat", 1);
				}
			}
		);
	}
}
