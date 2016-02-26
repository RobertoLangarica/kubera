using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatePowerUp : PowerupBase
{
	public GameObject powerUpBlock;
	public Transform powerUpButton;
	public RectTransform piecesPanel;

	protected InputPowerUpRotate inputPowerUpRotate;
	protected InputBombAndDestroy inputPowerUp;

	protected GameObject powerUpGO;

	void Start()
	{
		inputPowerUpRotate = FindObjectOfType<InputPowerUpRotate> ();
		inputPowerUp = FindObjectOfType<InputBombAndDestroy> ();

		inputPowerUpRotate.gameObject.SetActive (false);
		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		
		this.gameObject.SetActive( true);

		powerUpGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		powerUpGO.name = "RotatePowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		inputPowerUp.enabled = true;
		inputPowerUp.setCurrentSelected(powerUpGO);
		inputPowerUp.OnDrop += powerUpPositionated;
	}

	public void powerUpPositionated()
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
				powerUpActivateRotate ();
			}
		}
			
		inputPowerUp.OnDrop -= powerUpPositionated;
		if (!activated) 
		{
			powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("RotatePowerUP_Move");
			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("RotatePowerUP_Scale").OnComplete (() => {

				DestroyImmediate(powerUpGO);
			});
			cancelPowerUp ();
			return;
		}
		powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("RotatePowerUP_Scale").OnComplete (() => {

			DestroyImmediate(powerUpGO);
		});

	}

	public void powerUpActivateRotate()
	{
		inputPowerUpRotate.gameObject.SetActive (true);
		inputPowerUpRotate.OnPowerupRotateCompleted += completePowerUp;
		inputPowerUpRotate.OnPowerupRotateCanceled += cancelPowerUp;
		inputPowerUpRotate.startRotate ();
	}

	protected void completePowerUp()
	{
		OnComplete ();
		inputPowerUpRotate.enabled = true;
		this.gameObject.SetActive( false);
	}

	protected void cancelPowerUp()
	{
		OnCancel ();
		inputPowerUpRotate.enabled = true;
		this.gameObject.SetActive( false);
	}
}

