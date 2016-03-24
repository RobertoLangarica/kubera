﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WildCardPowerUp : PowerupBase
{
	public string powerUpScore = "x3";
	public GameObject powerUpWildCard;
	public Transform powerUpButton;
	public RectTransform wordsContainer;

	protected InputBombAndDestroy inputPowerUp;

	protected GameObject powerUpGO;

	protected ABC.WordManager wordManager;

	void Start()
	{
		wordManager = FindObjectOfType<ABC.WordManager> ();

		inputPowerUp = FindObjectOfType<InputBombAndDestroy> ();

		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		this.gameObject.SetActive( true);
		if (powerUpGO != null) 
		{
			DestroyImmediate (powerUpGO);
		}
		powerUpGO = Instantiate (powerUpWildCard,powerUpButton.position,Quaternion.identity) as GameObject;
		powerUpGO.name = "WildPowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		inputPowerUp.enabled = true;
		inputPowerUp.setCurrentSelected(powerUpGO);
		inputPowerUp.OnDrop += powerUpPositioned;
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
			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("WildCardPowerUP_Scale").OnComplete (() => {

				DestroyImmediate(powerUpGO);
			});
			cancelPowerUp ();
			return;
		}
		powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("WildCardPowerUP_Scale").OnComplete (() => {

			DestroyImmediate(powerUpGO);
		});

	}

	public void addWildcard()
	{
		wordManager.addLetter(wordManager.getWildcard (powerUpScore));
		OnComplete ();
	}

	protected void completePowerUp()
	{
		OnComplete ();
		this.gameObject.SetActive( false);
	}

	protected void cancelPowerUp()
	{
		OnCancel ();
		this.gameObject.SetActive( false);
	}

}
