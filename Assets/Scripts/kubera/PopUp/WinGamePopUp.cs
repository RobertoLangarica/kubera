using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinGamePopUp : PopUpBase {

	public Text winText;
	public RectTransform winContent;
	public float speed =1;

	void Start()
	{
		winText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.WIN_TEXT_POPUP_ID);
	}

	public override void activate()
	{
		popUp.SetActive (true);

		Vector3 v3 = new Vector3 ();
		v3 = winContent.anchoredPosition;

		winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).SetEase(Ease.OutBack).OnComplete(()=>
			{
				winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).OnComplete(()=>
					{
						winContent.DOAnchorPos (-v3, speed).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel
								print("gano");
								popUpCompleted("winPopUpEnd");
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
