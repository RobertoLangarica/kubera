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
	public Sprite powerUpImage;
	public Text priceText;

	public bool isFree;

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
}
