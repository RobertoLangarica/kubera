using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class HUDManager : MonoBehaviour 
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

	public Text goalText;
	public Text goalLettersText;
	public GameObject goalLettersContainer;

	public GameObject PointerOnScene;

	public GameObject exitGamePopUp;
	public Text exitText;
	public RectTransform exitContent;

	public Image[] musicImages;
	public Image[] soundsImages;

	public Text lettersPoints;
	public Text lettersPointsTitle;


	public Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);

	public GameObject goalPopUp;

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

		scoreText.text = lettersPointsTitle.text = GameTextManager.instance.getTextByID ("hudPuntaje");
	}

	/**
	 * Setea los movimientos en la hud
	 **/
	public void setMovements(int movments)
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
	 * Puntos para obtener estrellas
	 **/
	public void setStarsData(float[] scores)
	{
		scoreToStar = scores;
		setStarsPosition ();
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
	protected void setStarsPosition()
	{
		Stars[0].rectTransform.localPosition = new Vector3(Stars[0].rectTransform.localPosition.x, scoreToStar[0] / scoreToStar [2] * pointsMeter.rectTransform.rect.height );
		Stars[1].rectTransform.localPosition = new Vector3(Stars[1].rectTransform.localPosition.x, scoreToStar[1] / scoreToStar [2] * pointsMeter.rectTransform.rect.height);
		Stars[2].rectTransform.localPosition = new Vector3(Stars[2].rectTransform.localPosition.x, scoreToStar[2] / scoreToStar [2] * pointsMeter.rectTransform.rect.height );
	}
		
	public void setLevelName(string name)
	{
		levelText.text = name;
	}	

	public void showGoalAsLetters(bool isLetters)
	{
		if(isLetters)
		{
			goalText.gameObject.SetActive (false);
			goalLettersText.gameObject.SetActive (true);
		}
		else
		{
			goalText.gameObject.SetActive (true);
			goalLettersText.gameObject.SetActive (false);
		}
	}

	public void setSecondChanceLock(bool activate)
	{
		secondChanceLock.SetActive(activate);
	}

	/**
	 * setea la condicion de victoria
	 **/
	public void setPointsCondition(int pointsNeed, int pointsMade)
	{
		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalByPointsCondition"),new string[2]{"pointsMade","pointsNeed"},new string[2]{pointsMade.ToString(),pointsNeed.ToString()});
	}

	public void setWordsCondition(int wordsNeeded,int wordsMade)
	{
		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalByWordsCondition"),new string[2]{"wordsMade","wordNeed"},new string[2]{wordsMade.ToString(),wordsNeeded.ToString()});
	}

	public void setLettersCondition(List<string> letters = null)
	{
		goalLettersText.text = GameTextManager.instance.getTextByID("goalByLettersCondition");
		for (int i = 0; i < letters.Count; i++) 
		{
			GameObject letter =  Instantiate(uiLetter) as GameObject;
			letter.name = letters [i];
			lettersToFound.Add (letter);
			letter.GetComponentInChildren<Text> ().text = letters[i];
			letter.transform.SetParent (goalLettersContainer.transform,false);
		}
		setSizeOfContainer (letters.Count);
	}

	protected void setSizeOfContainer(int maxSize = 5)
	{
		GridLayoutGroup gridLayoutGroup = goalLettersContainer.GetComponent<GridLayoutGroup>();

		if(((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-gridLayoutGroup.padding.left) < goalLettersContainer.GetComponent<RectTransform> ().rect.height *.8f)
		{
			gridLayoutGroup.cellSize = new Vector2((goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-5
				,(goalLettersContainer.GetComponent<RectTransform> ().rect.width/maxSize )-gridLayoutGroup.padding.left);
		}
		else
		{
			gridLayoutGroup.cellSize = new Vector2(goalLettersContainer.GetComponent<RectTransform>().rect.height*.8f
				,goalLettersContainer.GetComponent<RectTransform>().rect.height*.8f);
		}
	}

	public void setObstaclesCondition(int value=0)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		//goalText.text = GameTextManager.instance.getTextByID("goalByObstaclesCondition").Replace("goalObstacleLetters",value.ToString())

		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalByObstaclesCondition"),new string[1]{"goalObstacleLetters"},new string[1]{value.ToString()});
	}

	public void setWordCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalBy1WordCondition"),new string[1]{"goalWord"},new string[1]{word});
	}

	public void setSynCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalBySynonymousCondition"),new string[1]{"goalSin"},new string[1]{word});
	}

	public void setAntCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = GameTextManager.instance.changeTextWords(
			GameTextManager.instance.getTextByID("goalByAntonymCondition"),new string[1]{"goalAnt"},new string[1]{word});
	}
		
	public void destroyLetterFound(string letterFound)
	{
		for (int i = 0; i < lettersToFound.Count; i++) 
		{
			if (letterFound == lettersToFound [i].name) 
			{
				Destroy (lettersToFound [i]);
				lettersToFound.RemoveAt (i);
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
		
	public void activateRotateImage(bool activate,int activePos)
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
		for(int i = 0; i < pieces.Count; i++)
		{
			pieces[i].transform.position= new Vector3(rotationImagePositions [i].position.x,rotationImagePositions [i].position.y,1);
			pieces[i].positionOnScene = pieces[i].transform.position;
			pieces[i].initialPieceScale = initialPieceScale;
			pieces[i].transform.localScale = new Vector3 (0, 0, 0);
			pieces[i].transform.DOScale(initialPieceScale, 0.25f);
			pieces[i].transform.SetParent (showingPiecesContainer,false);
		}
	}


	public void showGoalPopUp(string goalCondition, Object parameters)
	{
		Text goalText = goalPopUp.transform.FindChild("Objective").GetComponent<Text>();
		string textId;
		string textToReplace;
		string replacement;

		switch(goalCondition)
		{
		case GoalManager.LETTERS:
			textId = "goalByLettersObjectivePopUp";
			textToReplace = "goalLetters";

			replacement = "";
			List<Letter> letters = (List<Letter>)parameters;

			for (int i = 0; i < letters.Count; i++) 
			{
				replacement += letters[i];
				replacement += (i == letters.Count-1 ? ".":", ");
			}

			break;
		case GoalManager.OBSTACLES:
			textId = "goalByObstaclesObjectivePopUp";
			textToReplace = "goalObstacleLetters";
			replacement = ((int)parameters).ToString();
			break;
		case GoalManager.POINTS:
			textId = "goalByPointsObjectivePopUp";
			textToReplace = "goalPoints";
			replacement = ((int)parameters).ToString();
			break;
		case GoalManager.WORDS_COUNT:
			textId = "goalByWordsObjectivePopUp";
			textToReplace = "goalWords";
			replacement = ((int)parameters).ToString();
			break;
		case GoalManager.SYNONYMOUS:
			textId = "goalBySynonymousObjectivePopUp";
			textToReplace = "goalSin";
			replacement = (string) parameters;
			break;
		case GoalManager.WORD:
			textId = "goalBy1WordObjectivePopUp";
			textToReplace = "goalWord";
			replacement = (string) parameters;
			break;
		
		case GoalManager.ANTONYMS:
			textId = "goalByAntonymObjectivePopUp";
			textToReplace = "goalAnt";
			replacement = (string) parameters;
			break;
		}

		goalText.text = GameTextManager.instance.getTextByID(textId).Replace(textToReplace,replacement);

		goalPopUp.SetActive(true);
	}
		
	public void hideGoalPopUp()
	{
		goalPopUp.SetActive(false);
	}

	public void quitGamePopUp()
	{
		exitGamePopUp.SetActive (true);
		exitText.text = GameTextManager.instance.getTextByID ("exitText");
		Vector3 v3 = new Vector3 ();
		v3 = exitContent.anchoredPosition;

		exitContent.DOAnchorPos (new Vector3(exitContent.anchoredPosition.x,0), 1.5f).SetEase(Ease.OutBack).OnComplete(()=>
			{
				exitContent.DOAnchorPos (new Vector3(exitContent.anchoredPosition.x,0), 1.5f).OnComplete(()=>
					{
						exitContent.DOAnchorPos (-v3, 1.0f).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel y hacerle perder una vida, etc.
								print("perdio");
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

	public void setLettersPoints(int lettersPoints)
	{
		this.lettersPoints.text = lettersPoints.ToString();
	}

	public void activateLettersPoints(bool activate)
	{
		lettersPoints.enabled = activate;
	}
}