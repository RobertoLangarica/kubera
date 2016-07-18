using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
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
	public GameObject uiLetter;

	public Text goalText;
	public Text goalLettersText;
	public GameObject goalLettersContainer;

	public GameObject PointerOnScene;

	public Image[] musicImages;
	public Image[] soundsImages;

	public Text lettersPoints;
	public Text lettersPointsTitle;

	public Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);

	public GameObject modal;

	protected FloatingTextPool scorePool;
	protected List<GameObject> lettersToFound = new List<GameObject>();
	protected HUDMetterAndStars hudStars;
	protected PopUpManager popUpManager;

	public delegate void DPopUpNotification(string action ="");
	public DPopUpNotification OnPopUpCompleted;

	public delegate void DNotifyEvent ();
	public DNotifyEvent OnPiecesScaled;

		void Start () 
	{
		hudStars = FindObjectOfType<HUDMetterAndStars> ();
		scorePool = FindObjectOfType<FloatingTextPool>();
		popUpManager = FindObjectOfType <PopUpManager> ();

		popUpManager.OnPopUpCompleted += popUpCompleted;

		
	}

	public int getEarnedStars()
	{
		return hudStars.getStarsAmount ();
	}

	public void updateTextPoints(int pointsCount)
	{
		points.text = pointsCount.ToString();
		hudStars.setMeterPoints (pointsCount);

		scoreText.text = lettersPointsTitle.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SCORE_HUD_TITLE_ID);
	}
		
	public void updateTextMovements(int movments)
	{
		movementsText.text = movments.ToString();
	}

	public void updateTextGems (int gems)
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
		modal.SetActive(activate);
	}

	/**
	 * setea la condicion de victoria
	 **/
	public void setWinCondition (string goalCondition, System.Object parameters)
	{
		string textId = string.Empty;
		string textToReplace = string.Empty;
		string replacement = string.Empty;
		List<string> word = new List<string>();

		goalText.text = "";

		switch (goalCondition)
		{
		case GoalManager.LETTERS:

			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_LETTERS_ID;
			textToReplace = "{{goalLetters}}";

			IEnumerable letters = parameters as IEnumerable;

			foreach (object val in letters) 
			{
				GameObject letter =  Instantiate(uiLetter) as GameObject;
				letter.name = val.ToString();
				lettersToFound.Add (letter);
				letter.GetComponentInChildren<Text> ().text = val.ToString();
				letter.transform.SetParent (goalLettersContainer.transform,false);
			}
			setSizeOfContainer (lettersToFound.Count);

			break;
		case GoalManager.OBSTACLES:
			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_OBSTACLES_ID;
			textToReplace = "{{goalObstacleLetters}}";
			replacement = (Convert.ToInt32(parameters)).ToString();
			break;
		case GoalManager.POINTS:
			textId = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.GOAL_CONDITION_BY_POINT_ID);
			goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
				new string[2]{ "{{pointsMade}}", "{{pointsNeed}}" }, new string[2] {
				"0",
				(Convert.ToInt32 (parameters)).ToString ()
			});
			break;
		case GoalManager.WORDS_COUNT:
			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_WORDS_ID;
			textToReplace = "{{goalWords}}";
			replacement = "0 / "+(Convert.ToInt32(parameters)).ToString();
			break;
		case GoalManager.SYNONYMOUS:
			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_SYNONYMOUS_ID;
			textToReplace = "{{goalSin}}";
			word= (List<string>)parameters;
			replacement = word[0];
			break;
		case GoalManager.WORD:
			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_1_WORD_ID;
			textToReplace = "{{goalWord}}";
			word= (List<string>)parameters;
			replacement = word[0];
			break;
		case GoalManager.ANTONYMS:
			textId = MultiLanguageTextManager.GOAL_CONDITION_BY_ANTONYM_ID;
			textToReplace = "{{goalAnt}}";
			word= (List<string>)parameters;
			replacement = word[0];
			break;
		}

		if (goalText.text == "") 
		{
			goalText.text = MultiLanguageTextManager.instance.getTextByID (textId).Replace (textToReplace, replacement);
		}
	}

	public void actualizePointsOnWinCondition(string pointsMade, string pointsNeed)
	{
		string textId = string.Empty;

		textId = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_POINT_ID);
		goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
			new string[2]{ "{{pointsMade}}", "{{pointsNeed}}" }, new string[2]{ pointsMade, pointsNeed });
	}

	public void actualizeWordsMadeOnWinCondition(string wordsMade, string wordsNeed)
	{
		string textId = string.Empty;

		textId = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_WORDS_ID);
		goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
			new string[2]{ "{{wordsMade}}", "{{wordNeed}}" }, new string[2]{ wordsMade, wordsNeed });
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
		/*print (activate);
		print (Sounds.gameObject.activeSelf);*/
		if (!Sounds.gameObject.activeSelf) 
		{
			/*points.enabled = false;
			scoreText.enabled = false;*/
			//Activar los otros botones
			Music.gameObject.SetActive(true);
			Exit.gameObject.SetActive(true);
			Sounds.gameObject.SetActive(true);
			PointerOnScene.SetActive(true);
		}
		else 
		{
			/*scoreText.enabled = true;
			points.enabled = true;*/
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
		FloatingText bText = poolText.gameObject.GetComponent<FloatingText>();

		poolText.text = "+" + score.ToString();

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
			if (i == (pieces.Count - 1)) 
			{
				pieces [i].transform.DOScale (initialPieceScale, 0.25f).OnComplete (()=>{OnPiecesScaled();});
			} 
			else 
			{
				pieces[i].transform.DOScale(initialPieceScale, 0.25f);
			}
			pieces[i].transform.SetParent (showingPiecesContainer,false);
		}
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

	public void activatePopUp(string popUpName)
	{
		popUpManager.activatePopUp (popUpName);
		modal.SetActive (true);
	}

	private void popUpCompleted(string action ="")
	{
		OnPopUpCompleted (action);
		if(popUpManager.openPopUps.Count == 0)
		{
			modal.SetActive (false);
		}  
	}
}