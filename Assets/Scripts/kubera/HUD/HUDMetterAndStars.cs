using UnityEngine;
using UnityEngine.UI;

public class HUDMetterAndStars : MonoBehaviour
{
	public Image pointsMeter;
	public Image[] Stars;
	public Sprite StarFilled;

	[HideInInspector]
	public float[] scoreToReachStar;

	bool star1Reached;
	bool star2Reached;
	bool star3Reached;

	float Star1 = 0.3f;
	float Star2 = 0.564f;
	float Star3 = 0.919f;

	void Start()
	{
		//pointsMeter.rectTransform.anchorMax = new Vector3 (0, 1, 0);//pointsMeter.rectTransform.rect.width;
	}

	/**
	 * setea el medidor de puntos
	 **/
	public void setMeterPoints(int points)
	{
		float size = (float)points / scoreToReachStar [2];
		if (size >= 1)
		{
			size = 1;
		}
		//pointsMeter.rectTransform.anchorMax = new Vector3 (size,1,0);
		actualizeStars (points);
	}

	/**
	 * Puntos para obtener estrellas
	 **/
	public void setStarsData(float[] scores)
	{
		scoreToReachStar = scores;
		//setStarsPosition ();
	}		

	/**
	 * setea las posiciones de las estrellas
	 **/
	public void setStarsPosition()
	{
		float pointMetterwidth = pointsMeter.rectTransform.rect.width;
		Stars[0].rectTransform.anchoredPosition = new Vector3(scoreToReachStar[0] / scoreToReachStar [2] * pointMetterwidth, 0);
		Stars[1].rectTransform.anchoredPosition = new Vector3(scoreToReachStar[1] / scoreToReachStar [2] * pointMetterwidth, 0);
		Stars[2].rectTransform.anchoredPosition =  new Vector2(pointMetterwidth,0);
	}

	/**
	 * actualiza las estrellas
	 **/
	protected void actualizeStars(int points)
	{
		float size = 0;

		if(!star1Reached)
		{
			size = scoreToReachStar [0] / Star1;

			size = (float)points / size;

			if(size >= Star1)
			{
				//Stars [0].sprite = StarFilled;
				star1Reached = true;
				star2Reached = true;
			}
		}

		if(star2Reached)
		{
			size = scoreToReachStar [1] / Star2;

			size = (float)points / size;

			if(size >= Star2)
			{
				//Stars [1].sprite = StarFilled;
				star2Reached = false;
				star3Reached = true;
			}
		}

		if(star3Reached)
		{
			size = scoreToReachStar [2] / Star3;

			size = (float)points / size;
		}
		pointsMeter.fillAmount = size;
	}

	public int getStarsAmount()
	{
		int result = 0;

		if (star3Reached) 
		{
			result = 3;
		}
		else if (star2Reached) 
		{
			result = 2;
		}
		else if (star3Reached) 
		{
			result = 1;
		}

		return result;
	}
}


