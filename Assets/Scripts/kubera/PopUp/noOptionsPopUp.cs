using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class noOptionsPopUp : PopUpBase {

	public Text text;
	public RectTransform winContent;
	public float speed =1;

	void Start()
	{
		text.text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.NO_PIECES_POPUP_ID);
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
								print("perdio");
								popUpCompleted("loose");
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
