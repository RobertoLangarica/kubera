using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WildCardPowerUp : PowerupBase
{
	public string powerUpScore = "x3";
	public RectTransform wordsContainer;

	public KeyBoardManager keyBoard;

	protected InputBombAndDestroy inputPowerUp;

	protected GameObject powerUpGO;

	protected WordManager wordManager;
	protected bool canUse;
	void Start()
	{
		wordManager = FindObjectOfType<WordManager> ();

		inputPowerUp = FindObjectOfType<InputBombAndDestroy> ();

		this.gameObject.SetActive( false);
	}

	public override void activate (bool canUse)
	{
		this.gameObject.SetActive( true);
		if (powerUpGO != null) 
		{
			DestroyImmediate (powerUpGO);
		}
		powerUpGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		powerUpGO.name = "WildPowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		inputPowerUp.enabled = true;
		inputPowerUp.setCurrentSelected(powerUpGO);
		inputPowerUp.OnDrop += powerUpPositioned;
		this.canUse = canUse;

		updateDragableObjectImage (powerUpGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_POWERUP);
	}

	public void powerUpPositioned()
	{
		bool activated = false;
		Vector3 v3;

		v3 = powerUpGO.transform.position;
		v3 = Camera.main.WorldToScreenPoint (v3);

		v3.x = v3.x/Screen.width;

		if (v3.x > wordsContainer.anchorMin.x && v3.x < wordsContainer.anchorMax.x) 
		{
			v3.y = v3.y/Screen.height;
			if (v3.y > wordsContainer.anchorMin.y && v3.y < wordsContainer.anchorMax.y) 
			{
				activated = true;
				addWildcard ();
			}
		}

		inputPowerUp.OnDrop -= powerUpPositioned;
		if (!activated) 
		{
			powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("WildCardPowerUP_Move");
			cancel ();
		}
		powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("WildCardPowerUP_Scale").OnComplete (() => {

			DestroyImmediate(powerUpGO);
		});
	}

	public void addWildcard()
	{
		if(!canUse)
		{
			completePowerUpNoGems ();
			return;
		}
		if (wordManager.isAddLetterAllowed ()) 
		{
			wordManager.addLetter (wordManager.getWildcard (powerUpScore),false,true);
			OnComplete ();
		}
		else
		{
			cancel ();
		}
	}

	protected void completePowerUp()
	{
		HighLightManager.GetInstance ().turnOffHighLights ();
		OnComplete ();
		this.gameObject.SetActive( false);
	}

	protected void completePowerUpNoGems()
	{
		powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("WildCardPowerUP_Move");
		HighLightManager.GetInstance ().turnOffHighLights ();
		OnCompletedNoGems ();
		this.gameObject.SetActive( false);
	}

	public override void cancel ()
	{
		HighLightManager.GetInstance ().turnOffHighLights ();
		OnCancel ();
		this.gameObject.SetActive( false);
	}

}
