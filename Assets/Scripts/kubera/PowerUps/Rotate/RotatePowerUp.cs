﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatePowerUp : PowerupBase
{
	public GameObject powerUpBlock;
	public RectTransform piecesPanel;

	protected InputPowerUpRotate inputPowerUpRotate;
	protected InputBombAndDestroy inputPowerUp;
	protected InputWords inputWords;

	protected GameObject powerUpGO;
	protected bool canUse;

	void Start()
	{
		inputWords = FindObjectOfType<InputWords> ();
		inputPowerUpRotate = FindObjectOfType<InputPowerUpRotate> ();
		inputPowerUp = FindObjectOfType<InputBombAndDestroy> ();

		inputPowerUpRotate.gameObject.SetActive (false);
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
		powerUpGO.name = "RotatePowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		inputPowerUp.enabled = true;
		inputPowerUp.setCurrentSelected(powerUpGO);
		inputPowerUp.OnDrop += powerUpPositioned;

		inputWords.allowInput = true;
		this.canUse = canUse;
	}

	public void powerUpPositioned()
	{
		bool activated = false;
		Vector3 v3 = new Vector3();
		v3 = powerUpGO.transform.position;
		v3 = Camera.main.WorldToScreenPoint (v3);

		v3.x = v3.x/Screen.width;

		if (v3.x > piecesPanel.anchorMin.x && v3.x < piecesPanel.anchorMax.x) 
		{
			v3.y = v3.y/Screen.height;
			if (v3.y > piecesPanel.anchorMin.y && v3.y < piecesPanel.anchorMax.y) 
			{
				activated = true;
			}
		}
		//print ("powerUpPositioned "+ activated);
		if (!activated) 
		{
			powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId ("RotatePowerUP_Move");
			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("RotatePowerUP_Scale").OnComplete (() => {

				DestroyImmediate (powerUpGO);
				cancel ();
			});
		}
		else 
		{
			if(!canUse)
			{
				powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId ("RotatePowerUP_Move");
			}

			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("RotatePowerUP_Scale").OnComplete (() => {

				DestroyImmediate (powerUpGO);
				powerUpActivateRotate (canUse);
			});

		}
		inputPowerUp.OnDrop -= powerUpPositioned;

	}

	public void powerUpActivateRotate(bool canUse)
	{
		inputPowerUp.OnDrop -= powerUpPositioned;
		if(canUse)
		{
			inputPowerUpRotate.gameObject.SetActive (true);
			inputPowerUpRotate.enabled = true;
			inputPowerUpRotate.startRotate ();
			inputPowerUpRotate.OnPowerupRotateCompleted += completePowerUp;
		}
		else
		{
			OnCompletedNoGems ();
		}
	}

	protected void completePowerUp()
	{
		inputPowerUpRotate.OnPowerupRotateCompleted -= completePowerUp;
		Debug.Log ("Completado");
		OnComplete ();
		inputPowerUpRotate.enabled = false;
		this.gameObject.SetActive( false);
	}

	public override void cancel()
	{
		//print ("cancelPowerUp");
		OnCancel ();
		inputPowerUpRotate.enabled = false;
		this.gameObject.SetActive( false);
	}
}

