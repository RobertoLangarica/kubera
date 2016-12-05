using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class PowerupBase : MonoBehaviour 
{
	public EType type;

	public enum EType
	{
		HINT_WORD,BOMB,BLOCK,ROTATE,WILDCARD,DESTROY
	}

	public delegate void DPowerUpNotification();
	public DPowerUpNotification OnPowerupCanceled;
	public DPowerUpNotification OnPowerupCompleted;
	public DPowerUpNotification OnPowerupUsed;
	public DPowerUpNotification OnPowerupCompletedNoGems;

	public Transform powerUpButton;
	public GameObject powerUpBlock;
	public GameObject stockLeftGO;
	public Text stockText;
	public Sprite powerUpImage;
	public Text priceText;
	public Text freeText;

	public bool isFree;

	public int stock;
	protected int maxValue = 9;

	public virtual void activate(bool canUse)
	{
	}

	public virtual void cancel()
	{
		
	}
		
	protected void OnComplete()
	{
		if(OnPowerupCompleted != null)
		{
			OnPowerupCompleted();
		}	
	}

	protected void OnCancel()
	{
		if(OnPowerupCanceled != null)
		{
			OnPowerupCanceled();
		}	
	}

	protected void OnCompletedNoGems()
	{
		if(OnPowerupCompletedNoGems != null)
		{
			OnPowerupCompletedNoGems();
		}	
	}

	protected void updateDragableObjectImage(GameObject powerUpObject)
	{
		Image temp = powerUpObject.GetComponentInChildren<Image> ();
		SpriteRenderer tempS = powerUpObject.GetComponentInChildren<SpriteRenderer> ();
	
		if (powerUpImage != null) 
		{
			if(powerUpObject.GetComponent<Piece>())
			{
				powerUpObject.GetComponent<Piece> ().currentColor = Piece.EColor.NONE;
			}
			
			if (temp != null) 
			{
				temp.sprite = Sprite.Create (powerUpImage.texture, powerUpImage.textureRect, new Vector2 (0.5f, 0.5f));
				temp.color = Color.white;
			} 
			else if (tempS != null) 
			{
				tempS.sprite = Sprite.Create (powerUpImage.texture, powerUpImage.textureRect, new Vector2 (0.5f, 0.5f));
				tempS.color = Color.white;
				powerUpImage = tempS.sprite;
			}
		}
	}

	public void makePowerUpFree(bool makeFree)
	{
		if (makeFree) 
		{
			isFree = true;

			priceText.gameObject.SetActive (false);
			freeText.gameObject.SetActive (true);

			freeText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.FREE_POWERUP_PRICE);
		}
		else
		{
			isFree = false;

			priceText.gameObject.SetActive (true);
			freeText.gameObject.SetActive (false);
		}
	}

	public void updateStock(int newStock)
	{
		stock += newStock;
		if(stock >0)
		{
			if(stock > maxValue)
			{
				stockText.text = "9+";
			}
			else
			{
				stockText.text = stock.ToString ();
			}
			stockLeftGO.SetActive (true);
		}
		else
		{
			stockText.text = stock.ToString ();
			stockLeftGO.SetActive (false);
		}
	}
}
