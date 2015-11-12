using UnityEngine;
using System.Collections;

public class FlashColor : MonoBehaviour {

	[HideInInspector]
	public delegate void OnFinish();
	[HideInInspector]
	public OnFinish onFinish;

	public Color colorToFlash = new Color(1,1,1,1);

	protected bool coloring;
	protected Color currentColor;
	protected Color initialColor;
	protected float inverseColorTime;
	protected float colorElapsed;
	protected Color destinationColor;
	protected SpriteRenderer spriteRenderer;
	protected float percent;

	void Start()
	{
		onFinish = foo;
	}
	
	void foo(){}
	
	void Update () 
	{
		if(coloring)
		{
			percent = colorElapsed*inverseColorTime;

			currentColor = Color.Lerp(currentColor,destinationColor,percent);

			if(currentColor == destinationColor)
			{
				//if(currentColor == initialColor)
				//{
					coloring = false;
					onFinish();
				//}
				/*else
				{
					destinationColor = initialColor;
					colorElapsed = 0;
				}*/
			}

			spriteRenderer.color = currentColor;
			colorElapsed += Time.deltaTime;
		}	
	}

	public void startFlash(SpriteRenderer renderer, float delay = 0.2f)
	{
		if(coloring)
		{
			return;
		}

		inverseColorTime = (1.0f/delay)*0.5f;
		spriteRenderer = renderer;
		currentColor = colorToFlash;
		initialColor = spriteRenderer.color;
		destinationColor = spriteRenderer.color;
		spriteRenderer.color = colorToFlash;

		colorElapsed = 0;
		coloring = true;
	}

	public void startFlash(SpriteRenderer renderer, Color color, float delay = 0.2f)
	{
		colorToFlash = color;
		startFlash(renderer,delay);
	}
}
