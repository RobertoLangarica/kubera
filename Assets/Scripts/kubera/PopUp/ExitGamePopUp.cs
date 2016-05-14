using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ExitGamePopUp : PopUpBase {

	public Text exitText;
	public RectTransform exitContent;

	public override void activate()
	{
		popUp.SetActive (true);
		
		exitText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.EXIT_POPUP_ID);
		Vector3 v3 = new Vector3 ();
		v3 = exitContent.anchoredPosition;

		exitContent.DOAnchorPos (new Vector3(exitContent.anchoredPosition.x,0), 1.5f).SetEase(Ease.OutBack).OnComplete(()=>
			{
				exitContent.DOAnchorPos (new Vector3(exitContent.anchoredPosition.x,0), 1.5f).OnComplete(()=>
					{
						exitContent.DOAnchorPos (-v3, 1.0f).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel
								print("perdio");
								popUpCompleted("endGame");
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
