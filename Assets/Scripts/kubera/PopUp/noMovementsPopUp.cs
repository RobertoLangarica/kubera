using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class noMovementsPopUp : PopUpBase {

	public Text winText;
	public Text flavorText;
	public RectTransform winContent;

	public float speed =1;

	public override void activate()
	{
		popUp.SetActive (true);

		winText.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_MOVEMENTS_POPUP_ID);
		flavorText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FLAVOR_TEXT);
		Vector3 v3 = new Vector3 ();
		v3 = winContent.anchoredPosition;

		winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).SetEase(Ease.OutBack).OnComplete(()=>
			{
				winContent.DOAnchorPos (new Vector3(winContent.anchoredPosition.x,0), speed).SetEase(Ease.InBack).OnComplete(()=>
					{
						winContent.DOAnchorPos (-v3, speed).SetEase(Ease.InBack).OnComplete(()=>
							{
								//TODO: salirnos del nivel
								popUpCompleted("looseNoMovements");
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
