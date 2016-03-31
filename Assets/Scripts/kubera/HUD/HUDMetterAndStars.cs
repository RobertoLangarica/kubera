using UnityEngine;
using UnityEngine.UI;

public class HUDMetterAndStars : MonoBehaviour
{
	public Image pointsMeter;
	public Image[] Stars;

	[HideInInspector]
	public float[] scoreToReachStar;

	bool star1Reached;
	bool star2Reached;
	bool star3Reached;

	/**
	 * setea el medidor de puntos
	 **/
	public void setMeterPoints(int points)
	{
		pointsMeter.fillAmount = (float) points / scoreToReachStar [2];
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
		Stars[0].rectTransform.localPosition = new Vector3(scoreToReachStar[0] / scoreToReachStar [2] * pointMetterwidth, Stars[0].rectTransform.localPosition.y);
		Stars[1].rectTransform.localPosition = new Vector3(scoreToReachStar[1] / scoreToReachStar [2] * pointMetterwidth, Stars[1].rectTransform.localPosition.y);
		Stars[2].rectTransform.localPosition = new Vector3(scoreToReachStar[2] / scoreToReachStar [2] * pointMetterwidth, Stars[2].rectTransform.localPosition.y);
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
}


