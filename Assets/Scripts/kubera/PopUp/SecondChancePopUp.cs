using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using utils.gems;

public class SecondChancePopUp : PopUpBase 
{
	public delegate void DSecondChanceNotification();

	public DSecondChanceNotification OnSecondChanceAquired;
	public DSecondChanceNotification OnGiveUp;

	public RectTransform thisObject;
	public float speed =0.5f;
	protected Vector3 v3;

	public int movements = 5;
	public int bombs = 2;
	public Text priceText;
	public Text movementsText;
	public Text bombsText;

	public Text title;
	public Text acceptText;
	public Text cancelText;

	public GameObject discountDisplay;

	protected int secondChanceTimes = 0;
	protected int price;
	protected bool pressed;

	public override void activate()
	{
		popUp.SetActive (true);

		movementsText.text = "+" + movements.ToString ();
		bombsText.text = bombs.ToString ();

		title.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SECONDCHANCE_TITLE);
		acceptText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SECONDCHANCE_RETRY);
		cancelText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.SECONDCHANCE_GIVEUP);

		setPrice ();
		

		v3 = thisObject.anchoredPosition;
		thisObject.DOAnchorPos (new Vector3(thisObject.anchoredPosition.x,0), speed).SetEase(Ease.OutBack);
	}

	protected void setPrice()
	{
		switch(secondChanceTimes)
		{
		case(0):
			price = 10;
			break;
		case(1):
			price = 15;
			break;
		case(2):
			price = 20;
			break;
		default:
			price = 30;
			break;
		}

		checkDiscount ();
	}

	public int getCurrentPrice()
	{
		return price;
	}

	protected void checkDiscount()
	{
		//Debug.Log (UserDataManager.instance.playerGems);
		if (!ShopikaManager.GetCastedInstance<ShopikaManager>().isPossibleToConsumeGems(price)) 
		{
			price = (int)(price * 0.5f);
			discountDisplay.SetActive (true);
		}
		priceText.text = price.ToString ();
	}

	public void buyASecondChance()
	{
		if(pressed)
		{
			return;
		}

		pressed = true;

		if(ShopikaManager.GetCastedInstance<ShopikaManager>().isPossibleToConsumeGems(price))
		{
			ShopikaManager.GetCastedInstance<ShopikaManager>().tryToConsumeGems(price);
			secondChanceTimes++;

			thisObject.DOAnchorPos (-v3, speed).SetEase(Ease.InBack).OnComplete(()=>
				{
					//TODO: salirnos del nivel
					//print("gano");
					if (OnSecondChanceAquired != null) 
					{
						OnSecondChanceAquired ();
					}
					popUp.SetActive (false);
					OnComplete ();
					pressed = false;
				});
		}
		else
		{
			OnComplete ("NoGemsPopUp",false);
		}

		Debug.Log("Fondos insuficientes");
	}

	public void giveUp()
	{
		if(pressed)
		{
			return;
		}
		pressed = true;
		thisObject.DOAnchorPos (new Vector3(thisObject.anchoredPosition.x,0), speed).OnComplete(()=>
			{
				thisObject.DOAnchorPos (-v3, speed).SetEase(Ease.InBack).OnComplete(()=>
					{
						//TODO: salirnos del nivel
						//print("gano");
						if (OnGiveUp != null) 
						{
							OnGiveUp ();
						}

						pressed = false;
						OnComplete ("loose");
						popUp.SetActive (false);
					});
			});
		

		//popUpManager.activatePopUp ("RetryPopUp");
	}
}
