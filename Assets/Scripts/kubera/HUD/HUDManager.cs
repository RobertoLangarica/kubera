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
	public GameObject settingsBackground;

	public Text movementsText;
	public Text movementsText1;
	public Text movementsText2;
	public Text gemsText;
	public Text levelText;
	public Text levelNumber;
	public GameObject lvlGo;
	protected Button lvlButton;

	public Transform[] rotationImagePositions;
	public Transform showingPiecesContainer;

	public GameObject GemsChargeGO;
	public GameObject uiLetter;

	public Text goalText;
	public Text goalTextUP;
	public Text goalLettersText;
	public GameObject goalLettersContainer;

	public GameObject PointerOnScene;



	public Text lettersPoints;
	public Text lettersPointsTitle;

	public Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);

	public GameObject modal;
	public RectTransform vacum;
	protected Vector2 vacumStartPos;

	protected FloatingTextPool scorePool;
	protected List<GameObject> lettersToFound = new List<GameObject>();
	protected HUDMetterAndStars hudStars;
	protected PopUpManager popUpManager;

	public delegate void DPopUpNotification(string action ="");
	public DPopUpNotification OnPopUpCompleted;

	public delegate void DNotifyEvent ();
	public DNotifyEvent OnPiecesScaled;
	public GameObject wordsHighlight;

	public Button[] powerUps;

	void Start () 
	{
		hudStars = FindObjectOfType<HUDMetterAndStars> ();
		scorePool = FindObjectOfType<FloatingTextPool>();
		popUpManager = FindObjectOfType <PopUpManager> ();

		popUpManager.OnPopUpCompleted += popUpCompleted;
		lvlButton = lvlGo.GetComponent<Button> ();		

		setLevelGoFinalPosition ();
		setText ();
	}

	protected void setLevelGoFinalPosition()
	{
		DistanceJoint2D lvlGoJoint2D = lvlGo.GetComponent<DistanceJoint2D> ();
		lvlGoJoint2D.connectedAnchor = new Vector2 (Camera.main.aspect * -5 * .75f, lvlGoJoint2D.connectedAnchor.y);
	}

	protected void setText()
	{
		scoreText.text = lettersPointsTitle.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SCORE_HUD_TITLE_ID);
		levelText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.LVL_HUD_TITLE_ID);
	}

	public int getEarnedStars()
	{
		return hudStars.getStarsAmount ();
	}

	public void updateTextPoints(int pointsCount)
	{
		points.text = pointsCount.ToString();
		hudStars.setMeterPoints (pointsCount);

	}
		
	public void updateTextMovements(int movments)
	{
		movementsText.text = movementsText1.text = movementsText2.text = movments.ToString();
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
		for (int i = 0; i < name.Length; i++) 
		{
			if (name [i] == '0') 
			{
				name = name.Remove(i,1);
				i--;
			} 
			else 
			{
				break;
			}
		}
		levelNumber.text = name;
	}

	public void animateLvlGo(bool drop = true)
	{
		lvlGo.GetComponent<Rigidbody2D> ().isKinematic = false;
		lvlGo.GetComponent<DistanceJoint2D> ().enabled = true;
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
		string textUpId = string.Empty;
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
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_LETTERS_ID;
			break;
		case GoalManager.OBSTACLES:
			textId = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.GOAL_CONDITION_BY_OBSTACLES_ID);
			goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
				new string[2]{ "{{lettersUsed}}", "{{lettersNeed}}" }, new string[2] {
					"0",
					(Convert.ToInt32 (parameters)).ToString ()
				});
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_OBSTACLES_UP_ID;
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);
			break;
		case GoalManager.POINTS:
			textId = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.GOAL_CONDITION_BY_POINT_ID);
			goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
				new string[2]{ "{{pointsMade}}", "{{pointsNeed}}" }, new string[2] {
				"0",
				(Convert.ToInt32 (parameters)).ToString ()
			});
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.GOAL_CONDITION_BY_POINT_UP_ID);
			break;
		case GoalManager.WORDS_COUNT:
			textId = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.GOAL_CONDITION_BY_WORDS_ID);
			goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
				new string[2]{ "{{wordsMade}}", "{{wordsNeed}}" }, new string[2] {
					"0",
					(Convert.ToInt32 (parameters)).ToString ()
				});
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_WORDS_UP_ID;
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);
			break;
		case GoalManager.SYNONYMOUS:
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_SYNONYMOUS_ID;
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);

			word = (List<string>)parameters;
			replacement = word [0];
			goalText.text = replacement;
			break;
		case GoalManager.WORD:
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_1_WORD_ID;
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);

			word = (List<string>)parameters;
			replacement = word [0];
			goalText.text = replacement;
			break;
		case GoalManager.ANTONYMS:
			textUpId = MultiLanguageTextManager.GOAL_CONDITION_BY_ANTONYM_ID;
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);

			word = (List<string>)parameters;
			replacement = word [0];
			goalText.text = replacement;
			break;
		}

		if (goalText.text == "") 
		{
			goalText.text = MultiLanguageTextManager.instance.getTextByID (textId).Replace (textToReplace, replacement);
			goalTextUP.text = MultiLanguageTextManager.instance.getTextByID (textUpId);
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

	public void actualizePointsOnObstacleLetters(string obstacleLettersMade, string obstacleLettersNeed)
	{
		string textId = string.Empty;

		textId = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GOAL_CONDITION_BY_OBSTACLES_ID);
		goalText.text = MultiLanguageTextManager.instance.multipleReplace (textId,
			new string[2]{ "{{pointsMade}}", "{{pointsNeed}}" }, new string[2]{ obstacleLettersMade, obstacleLettersNeed });
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
		if (!settingsBackground.activeSelf) 
		{
			DOTween.Kill(settingsBackground,true);

			Exit.enabled = false;
			Music.enabled = false;
			Sounds.enabled = false;
			Exit.transform.localScale = Vector2.zero;
			Music.transform.localScale = Vector2.zero;
			Sounds.transform.localScale = Vector2.zero;

			settingsBackground.transform.localScale = Vector3.one;
			settingsBackground.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -180));
			settingsBackground.SetActive(true);

			settingsBackground.GetComponent<RectTransform> ().pivot = new Vector2(0.5f,0.5f);

			//Activar los otros botones
			settingsBackground.transform.DOLocalRotate (Vector3.zero,0.3f).SetId(settingsBackground).OnComplete(()=>
				{
					

					Exit.enabled = true;
					Music.enabled = true;
					Sounds.enabled = true;

					Exit.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
					Music.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
					Sounds.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.OutBack);
				});
			PointerOnScene.SetActive(true);
		}
		else 
		{
			DOTween.Kill(settingsBackground,true);

			Exit.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack);
			Music.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack);
			Sounds.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack).OnComplete(()=>
				{
					settingsBackground.GetComponent<RectTransform> ().pivot = Vector2.one;

					settingsBackground.transform.DOScale (new Vector3 (0, 0,0),0.3f).SetId(settingsBackground).OnComplete(()=>
						{
							settingsBackground.SetActive(false);

						});
				});

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
		finish.y += 1;// HACK: poolText.rectTransform.rect.height;

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
			pieces[i].transform.localScale = initialPieceScale;
			pieces [i].transform.position = new Vector2 (pieces [i].transform.position.x, pieces [i].transform.position.y+pieces [i].transform.position.y*0.5f);
			if (i == (pieces.Count - 1)) 
			{
				pieces [i].transform.DOMove (pieces[i].positionOnScene, 0.5f).SetEase(Ease.OutBack).OnComplete (()=>{OnPiecesScaled();});
				//pieces [i].transform.DOScale (initialPieceScale, 0.25f).OnComplete (()=>{OnPiecesScaled();});
			} 
			else 
			{
				pieces [i].transform.DOMove (pieces[i].positionOnScene, 0.5f).SetEase(Ease.OutBack);
				//pieces[i].transform.DOScale(initialPieceScale, 0.25f);
			}
			pieces[i].transform.SetParent (showingPiecesContainer,false);
		}
	}

	public void showVacum(float closeTime)
	{
		vacumStartPos = vacum.transform.position;

		DOTween.Kill (vacum);
		vacum.DOAnchorPos (Vector2.zero,0.2f).SetEase (Ease.OutBack).SetId(vacum);

		Invoke ("hideVacum", closeTime);
	}

	public void hideVacum()
	{
		DOTween.Kill (vacum);
		vacum.DOMove (vacumStartPos,0.2f).SetEase (Ease.InBack).SetId(vacum);
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

	public void activatePowerUpButtont(bool activate)
	{
		if(activate)
		{
			for(int i=0; i<powerUps.Length; i++)
			{
				powerUps [i].transition = Selectable.Transition.ColorTint;
				powerUps [i].interactable = true;
			}
		}
		else
		{
			for(int i=0; i<powerUps.Length; i++)
			{
				powerUps [i].transition = Selectable.Transition.None;
				powerUps [i].interactable = false;

			}
		}
	}
}