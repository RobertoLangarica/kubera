using UnityEngine;
using UnityEngine.UI;

public class HUDMetterAndStars : MonoBehaviour
{
	public Image pointsMeter;
	public Image[] Stars;
	public Image[] Lines;

	[HideInInspector]
	public float[] scoreToReachStar;

	bool star1Reached;
	bool star2Reached;
	bool star3Reached;

	void Start()
	{
		pointsMeter.rectTransform.anchorMax = new Vector3 (0, 1, 0);//pointsMeter.rectTransform.rect.width;
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
		pointsMeter.rectTransform.anchorMax = new Vector3 (size,1,0);
		actualizeStars (points);
	}

	/**
	 * Puntos para obtener estrellas
	 **/
	public void setStarsData(float[] scores)
	{
		scoreToReachStar = scores;
		setStarsPosition ();
	}		

	/**
	 * setea las posiciones de las estrellas
	 **/
	public void setStarsPosition()
	{
		float pointMetterwidth = pointsMeter.rectTransform.rect.width;

		Lines[0].rectTransform.anchoredPosition = new Vector3(scoreToReachStar[0] / scoreToReachStar [2] * pointMetterwidth, 0);
		Lines[1].rectTransform.anchoredPosition = new Vector3(scoreToReachStar[1] / scoreToReachStar [2] * pointMetterwidth, 0);
		Stars [2].rectTransform.anchoredPosition =  new Vector2(pointMetterwidth,0);
	}

	/**
	 * actualiza las estrellas
	 **/
	protected void actualizeStars(int points)
	{
		if(points >= scoreToReachStar[0] && !star1Reached)
		{
			star1Reached = true;
			Stars [0].color = Color.yellow;
		}
		else if(points >= scoreToReachStar[1] && !star2Reached)
		{
			star2Reached = true;
			Stars [1].color = Color.yellow;
		}
		else if(points >= scoreToReachStar[2] && !star3Reached)
		{
			star3Reached = true;
			Stars [2].color = Color.yellow;
		}
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


