using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startGamePopUp : PopUpBase {
	
	public Text text;
	public Text flavorText;
	public RectTransform winContent;
	public float speed =1;

	void Start()
	{
		flavorText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FLAVOR_TEXT);
	}

	public void initText(string goal)
	{
		switch (goal) {
		case "points":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_POINTS_TEXT);
			break;
		case "letters":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_LETTERS_TEXT);
			break;
		case "obstacles":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_BLACK_TEXT);
			break;
		case "word":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_WORD_TEXT);
			break;
		case "words":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_WORDS_TEXT);
			break;
		case "ant":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_ANTONYM_TEXT);
			break;
		case "sin":
			text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.START_GAME_OBJECTIVE_SYNONYM_TEXT);
			break;
		default:
			break;
		}
	}

	public override void activate()
	{
		popUp.SetActive (true);

		Vector3 v3 = new Vector3 ();
		v3 = winContent.anchoredPosition;

		winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed + speed).SetEase(Ease.OutBack).OnComplete(()=>
			{
				winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).OnComplete(()=>
					{
						winContent.DOAnchorPos (-v3, speed).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel
								popUpCompleted("startGame");
							});
					});
			});
	}

	protected void popUpCompleted(string action ="")
	{
		popUp.SetActive (false);
		OnComplete (action);
	}
}