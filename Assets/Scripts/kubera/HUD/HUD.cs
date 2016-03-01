﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

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

	public Transform[] rotationImagePositions;
	public Transform showingPiecesContainer;

	public GameObject GemsChargeGO;
	public GameObject secondChanceLock;
	public GameObject uiLetter;

	public Text winConditionText;
	public Text winConditionLettersText;
	public GameObject winConditionLettersContainer;

	public GameObject PointerOnScene;

	public GameObject exitGamePopUp;

	public Image[] musicImages;
	public Image[] soundsImages;

	//DONE: Hardcoding
	public Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);

	public GameObject objectivePopUp;

	protected float[] scoreToStar;
	protected ScoreTextPool scorePool;
	protected List<GameObject> lettersToFound;

	void Start () 
	{
		lettersToFound = new List<GameObject>();
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
			winConditionText.gameObject.SetActive (true);
			winConditionLettersText.gameObject.SetActive (false);
			winConditionText.text = "Obten: " + value +" puntos.";
			break;
		case "words":
			winConditionText.gameObject.SetActive (true);
			winConditionLettersText.gameObject.SetActive (false);
			winConditionText.text = "Forma: " + value +" palabras.";
			break;
		case "letters":
			winConditionText.gameObject.SetActive (false);
			winConditionLettersText.gameObject.SetActive (true);
			winConditionLettersText.text = "Usa: ";
			for (int i = 0; i < letters.Count; i++) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = letters [i];
				lettersToFound.Add (letter);
				letter.GetComponentInChildren<Text> ().text = letters[i];
				letter.transform.SetParent (winConditionLettersContainer.transform,false);
			}

			break;
		case "obstacles":
			winConditionText.gameObject.SetActive (false);
			winConditionLettersText.gameObject.SetActive (true);
			winConditionLettersText.text = "Usa: ";
			print (letters.Count);
			for (int i = 0; i < letters.Count; i++) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = letters [i];
				lettersToFound.Add (letter);
				letter.GetComponentInChildren<Text> ().text = letters[i];
				letter.transform.SetParent (winConditionLettersContainer.transform,false);
				letter.GetComponent<Image>().color = Color.grey;
			}

			break;
		case "word":
			winConditionText.gameObject.SetActive (true);
			winConditionLettersText.gameObject.SetActive (false);
			string wordString = "";
			for (int i = 0; i<words.Length; i++) 
			{
				wordString += words [i];
				if (i + 1 < words.Length) 
				{
					wordString += ", ";
				}
			}
			winConditionText.text = "Forma: " + wordString;

			break;
		default:
			break;
		}
	}

	public void destroyLetterFound(string letterFound)
	{
		for (int i = 0; i < lettersToFound.Count; i++) 
		{
			if (letterFound == lettersToFound [i].name) 
			{
				lettersToFound.RemoveAt (i);
				Destroy (lettersToFound [i]);
				break;
			}
		}
	}

	public void activateSettings(bool activate)
	{
		if (points.IsActive () && activate) 
		{
			points.enabled = false;
			scoreText.enabled = false;
			//Activar los otros botones
			Music.gameObject.SetActive(true);
			Exit.gameObject.SetActive(true);
			Sounds.gameObject.SetActive(true);
			PointerOnScene.SetActive(true);
		}
		else 
		{
			scoreText.enabled = true;
			points.enabled = true;
			//Desactivar los otros botones
			Music.gameObject.SetActive(false);
			Exit.gameObject.SetActive (false);
			Sounds.gameObject.SetActive(false);
			PointerOnScene.SetActive(false);
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
		
	public void activateTransformImage(bool activate,int activePos)
	{
		if (activate) 
		{
			rotationImagePositions [activePos].GetComponent<Image> ().enabled = true;
		}
		else 
		{
			rotationImagePositions [activePos].GetComponent<Image> ().enabled = false;
		}
	}

	public void showPieces(List<Piece> pieces)
	{
		List<Piece> newListPieces = new List<Piece> (pieces);
		int i = 0;
		while(newListPieces.Count >0)
		{
			Piece go = Instantiate (newListPieces [0]);

			go.name = newListPieces[0].name;
			newListPieces.RemoveAt(0);

			go.transform.position= new Vector3(rotationImagePositions [i].position.x,rotationImagePositions [i].position.y,1);
			go.transform.localScale = new Vector3 (0, 0, 0);
			go.transform.DOScale(initialPieceScale, 0.25f);
			go.transform.SetParent (showingPiecesContainer);

			i++;
		}
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
	}

	public void quitGamePopUp()
	{
		exitGamePopUp.SetActive (true);
		RectTransform content = exitGamePopUp.transform.FindChild ("Content").GetComponent<RectTransform>();
		Vector3 v3 = new Vector3 ();
		v3 = content.anchoredPosition;

		content.DOAnchorPos (new Vector3(content.anchoredPosition.x,0), 1.5f).SetEase(Ease.OutBack).OnComplete(()=>
			{
				content.DOAnchorPos (new Vector3(content.anchoredPosition.x,0), 1.5f).OnComplete(()=>
					{
						content.DOAnchorPos (-v3, 1.0f).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel y hacerle perder una vida, etc.
							});
					});
			});
	}

	public void setStateMusic(bool activate)
	{
		if (activate) 
		{
			Music.image = musicImages [0];
		}
		else
		{
			Music.image = musicImages [1];
		}
	}

	public void setStateSounds(bool activate)
	{
		if (activate) 
		{
			Sounds.image = soundsImages [0];
		}
		else
		{
			Sounds.image = soundsImages [1];
		}
	}
}