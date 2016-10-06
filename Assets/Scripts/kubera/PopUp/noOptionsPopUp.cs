using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class noOptionsPopUp : PopUpBase {

	public GameObject initial;
	public GameObject toChose;

	public Text FirstText;
	public Text flavorText;
	public Text toChoseText;
	public Text buttonContinue;
	public Text buttonExit;

	public RectTransform winContent;
	public float speed =1;
	public RectTransform LetterContainer;
	public Vector2 startPosition;

	void Start()
	{
		FirstText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_PIECES_POPUP_ID);
		flavorText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FLAVOR_TEXT);
		startPosition = new Vector2(winContent.localPosition.x, Screen.height * 2);

		winContent.localPosition = startPosition;

		toChoseText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GAME_NO_OPTION_TITLE);
		buttonContinue.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GAME_NO_OPTION_CONTINUE);
		buttonExit.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.GAME_NO_OPTION_GIVEUP);
	}

	public override void activate()
	{
		popUp.SetActive (true);
		initial.SetActive (true);
		toChose.SetActive(false);
		winContent.localPosition = startPosition;

		winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).SetEase(Ease.OutBack).OnComplete(()=>
			{
				winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).OnComplete(()=>
					{
						winContent.DOLocalMoveY(LetterContainer.transform.localPosition.y,speed).SetEase(Ease.InBack).OnComplete(()=>
							{
								initial.SetActive(false);
								toChose.SetActive(true);

							});
					});
			});
	}

	public void continuePlaying()
	{
		winContent.DOLocalMoveY(startPosition.y,speed).SetEase(Ease.InBack).OnComplete(()=>
			{
				popUpCompleted("checkInSeconds");
			});
		
	}

	public void exitPlaying()
	{
		popUpCompleted("loose");
	}

	protected void popUpCompleted(string action ="")
	{
		popUp.SetActive (false);
		OnComplete (action);
	}
}
