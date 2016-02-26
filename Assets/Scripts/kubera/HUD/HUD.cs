using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	public Text points;
	public Text scoreText;

	public Button Music;
	public Button Exit;
	public Button Sounds;

	public Text movementsText;
	public Text gemsText;
	public Text levelText;
	public Image pointsMeter;
	public Image[] Stars;
	public Text winConditionText;

	public GameObject GemsChargeGO;
	public GameObject secondChanceLock;
	public GameObject uiLetter;

	public GameObject objectivePopUp;

	public delegate void DPopUpNotification();
	public DPopUpNotification OnObjectivePopUpClose;

	protected float[] scoreToStar;
	protected ScoreTextPool scorePool;

	void Start () 
	{
		scorePool = FindObjectOfType<ScoreTextPool>();
	}

	/**
	 * Setea los puntos en la hud
	 **/
	public void setPoints(int pointsCount)
	{
		points.text = pointsCount.ToString();
		setMeterPoints (pointsCount);
	}

	/**
	 * Setea los movimientos en la hud
	 **/
	public void setMovments(int movments)
	{
		movementsText.text = movments.ToString();
	}

	/**
	 * Setea el dinero en la hud
	 **/
	public void setGems (int gems)
	{
		gemsText.text = gems.ToString();
	}

	/**
	 * Setea cuanto se cobrara en la hud
	 **/
	public void setChargeGems(int chargeGems)
	{
		if(GemsChargeGO != null)
		{
			if (chargeGems == 0) 
			{
				GemsChargeGO.GetComponentInChildren<Text> ().text = " " + chargeGems.ToString ();
			}
			else
			{
				GemsChargeGO.GetComponentInChildren<Text> ().text = "-" + chargeGems.ToString ();
			}
		}
	}

	/**
	 * Activa o desactiva el gemsCharge
	 **/
	public void activateChargeGems(bool activate)
	{
		GemsChargeGO.SetActive (activate);
	}

	/**
	 * setea los datos para obtener estrellas
	 **/
	public void setMeterData(float[] scoreToStarData)
	{
		scoreToStar = scoreToStarData;
		setPositionOfStars ();
		setPoints (0);
	}


	/**
	 * setea el medidor de puntos
	 **/
	public void setMeterPoints(int points)
	{
		pointsMeter.fillAmount = (float) points / scoreToStar [2];
		actualizeStars (points);
	}

	/**
	 * actualiza las estrellas
	 **/
	protected void actualizeStars(int points)
	{
		if(points >= scoreToStar[0])
		{
			Stars [0].color = Color.yellow;
		}
		if(points >= scoreToStar[1])
		{
			Stars [1].color = Color.yellow;
		}
		if(points >= scoreToStar[2])
		{
			Stars [2].color = Color.yellow;
		}
	}

	/**
	 * setea las posiciones de las estrellas
	 **/
	protected void setPositionOfStars()
	{
		Stars[0].rectTransform.localPosition = new Vector3(Stars[0].rectTransform.localPosition.x, scoreToStar[0] / scoreToStar [2] * pointsMeter.rectTransform.rect.height );
		Stars[1].rectTransform.localPosition = new Vector3(Stars[1].rectTransform.localPosition.x, scoreToStar[1] / scoreToStar [2] * pointsMeter.rectTransform.rect.height);
		Stars[2].rectTransform.localPosition = new Vector3(Stars[2].rectTransform.localPosition.x, scoreToStar[2] / scoreToStar [2] * pointsMeter.rectTransform.rect.height );
	}
		
	public void setLevelName(string name)
	{
		levelText.text = name;
	}	

	/**
	 * Setea la condicion de victoria
	 **/
	public void setWinCondition(string winCondition)
	{
		
	}	

	/**
	 * Setea la condicion de victoria
	 **/
	public void setSecondChanceLock(bool activate)
	{
		secondChanceLock.SetActive(activate);
	}

	/**
	 * setea la condicion de victoria
	 **/
	public void setWinConditionOnHud(string winCondition, string[] words, int value=0,List<string> letters = null)
	{
		//[TODO] Jalar textos del xml de idiomas
		switch (winCondition) {
		case "points":
			winConditionText.text = "Obten: " + value +" puntos.";
			break;
		case "words":
			winConditionText.text = "Forma: " + value +" palabras.";
			break;
		case "letters":
		case "blackLetters":
			winConditionText.text = "Usa: ";

			break;
		case "word":
			winConditionText.text = "Forma: " + words;
			break;

		default:
			break;
		}
	}

	public void activateSettings()
	{
		if (points.IsActive ()) 
		{
			points.enabled = false;
			scoreText.enabled = false;
			//Activar los otros botones
			Music.gameObject.SetActive(true);
			Exit.gameObject.SetActive(true);
			Sounds.gameObject.SetActive(true);
		}
		else 
		{
			scoreText.enabled = true;
			points.enabled = true;
			//Desactivar los otros botones
			Music.gameObject.SetActive(false);
			Exit.gameObject.SetActive (false);
			Sounds.gameObject.SetActive(false);
		}
	}

	public void showScoreTextAt(Vector3 scorePosition,int score)
	{
		Vector3 finish = scorePosition;
		Text poolText = scorePool.getFreeText();
		ScoreText bText = poolText.gameObject.GetComponent<ScoreText>();

		poolText.text = score.ToString();

		scorePosition.z = 0;
		finish.y += 2;// HACK: poolText.rectTransform.rect.height;

		//Se inicia la animacion del texto
		bText.startAnim(scorePosition,finish);
	}

	public void showObjectivePopUp(string objectiveType,string objective)
	{
		Text objectiveTypeText = objectivePopUp.transform.FindChild("Type").GetComponent<Text>();
		Text objectiveText = objectivePopUp.transform.FindChild("Objective").GetComponent<Text>();

		objectiveTypeText.text = objectiveType;
		objectiveText.text = objective;

		objectivePopUp.SetActive(true);
	}

	public void hideObjectivePopUp()
	{
		objectivePopUp.SetActive(false);

		if(OnObjectivePopUpClose != null)
		{
			OnObjectivePopUpClose();
		}
	}
}