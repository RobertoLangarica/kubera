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

	protected ScoreTextPool scorePool;
	protected List<GameObject> lettersToFound = new List<GameObject>();
	protected HUDMetterAndStars hudStars;

	void Start () 
	{
		hudStars = FindObjectOfType<HUDMetterAndStars> ();
		scorePool = FindObjectOfType<ScoreTextPool>();
	}
		
	public void actualizePoints(int pointsCount)
	{
		points.text = pointsCount.ToString();
		hudStars.setMeterPoints (pointsCount);

		scoreText.text = lettersPointsTitle.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SCORE_HUD_TITLE_ID);
	}
		
	public void actualizeMovements(int movments)
	{
		movementsText.text = movments.ToString();
	}

	public void actualizeGems (int gems)
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
		
	public void setStarsData(float[] scores)
	{
		hudStars.setStarsData (scores);
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
		goalText.text = MultiLanguageTextManager.instance.multipleReplace(
			MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_POINT_ID),new string[2]{"{{pointsMade}}","{{pointsNeed}}"},new string[2]{pointsMade.ToString(),pointsNeed.ToString()});
	}

	public void setWordsCondition(int wordsNeeded,int wordsMade)
	{
		goalText.text = MultiLanguageTextManager.instance.multipleReplace(
			MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_WORDS_ID),new string[2]{"{{wordsMade}}","{{wordNeed}}"},new string[2]{wordsMade.ToString(),wordsNeeded.ToString()});
	}

	public void setLettersCondition(List<string> letters = null)
	{
		goalLettersText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_LETTERS_ID);
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

		goalText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_OBSTACLES_ID).Replace("{{goalObstacleLetters}}",value.ToString());
	}

	public void setWordCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_1_WORD_ID).Replace("{{goalWord}}",word);
	}

	public void setSynCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_SYNONYMOUS_ID).Replace("{{goalSin}}",word);
	}

	public void setAntCondition(string word)
	{
		//TODO: Los textos que se usan como codigo para reemplazar deben ser menos coloquiales para evitar que se reesccriban
		//TODO: goalSin != {{goalSin}} o [[goalSin]] o <<goalSin>> o <code>goalSin</code> Algo que indique a quien edite estas cadenas que no debe de modificarse
		//TODO: Usar un replace que no necesite estar creando arreglos
		goalText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_ANTONYM_ID).Replace("{{goalAnt}}",word);
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
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_LETTERS_ID;
			textToReplace = "{{goalLetters}}";

			replacement = "";
			List<Letter> letters = parameters as List<Letter>;

			for (int i = 0; i < letters.Count; i++) 
			{
				replacement += letters[i];
				replacement += (i == letters.Count-1 ? ".":", ");
			}

			break;
		case GoalManager.OBSTACLES:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_OBSTACLES_ID;
			textToReplace = "{{goalObstacleLetters}}";
			replacement = (int.Parse(parameters)).ToString();
			break;
		case GoalManager.POINTS:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_POINTS_ID;
			textToReplace = "{{goalPoints}}";
			replacement = (int.Parse(parameters)).ToString();
			break;
		case GoalManager.WORDS_COUNT:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_WORDS_ID;
			textToReplace = "{{goalWords}}";
			replacement = (int.Parse(parameters)).ToString();
			break;
		case GoalManager.SYNONYMOUS:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_SYNONYMOUS_ID;
			textToReplace = "{{goalSin}}";
			replacement = parameters.ToString();
			break;
		case GoalManager.WORD:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_1_WORD_ID;
			textToReplace = "{{goalWord}}";
			replacement = parameters.ToString();
			break;
		
		case GoalManager.ANTONYMS:
			textId = MultiLanguageTextManager.OBJECTIVE_POPUP_BY_ANTONYM_ID;
			textToReplace = "{{goalAnt}}";
			replacement = parameters.ToString();
			break;
		}

		goalText.text = MultiLanguageTextManager.instance.getTextByID(textId).Replace(textToReplace,replacement);

		goalPopUp.SetActive(true);
	}
		
	public void hideGoalPopUp()
	{
		goalPopUp.SetActive(false);
	}

	public void quitGamePopUp()
	{
		exitGamePopUp.SetActive (true);
		exitText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.EXIT_POPUP_ID);
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