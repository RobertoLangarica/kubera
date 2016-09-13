using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

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

	public GameObject discountDisplay;

	protected int secondChanceTimes = 0;
	protected int price;

	public override void activate()
	{
		popUp.SetActive (true);

		movementsText.text = "+" + movements.ToString ();
		bombsText.text = bombs.ToString ();

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

	protected void checkDiscount()
	{
		//Debug.Log (UserDataManager.instance.playerGems);
		if (!TransactionManager.GetInstance().checkIfExistEnoughGems (price)) 
		{
			price = (int)(price * 0.5f);
			discountDisplay.SetActive (true);
		}
		priceText.text = price.ToString ();
	}

	public void buyASecondChance()
	{
		if(TransactionManager.GetInstance().tryToUseGems(price))
		{
			secondChanceTimes++;

			thisObject.DOAnchorPos (new Vector3(thisObject.anchoredPosition.x,0), speed).OnComplete(()=>
				{
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
						});
				});
		}
		else
		{
			OnComplete ("NoGemsPopUp");
		}

		Debug.Log("Fondos insuficientes");
	}

	public void giveUp()
	{
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

						OnComplete ("loose");
						popUp.SetActive (false);
					});
			});
		

		//popUpManager.activatePopUp ("RetryPopUp");
	}
}
