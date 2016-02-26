using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatePowerUp : PowerupBase
{
	InputPowerUpRotate inputPowerUpRotate;

	void Start()
	{
		inputPowerUpRotate = FindObjectOfType<InputPowerUpRotate> ();

		inputPowerUpRotate.OnPowerupRotateCompleted += completePowerUp;
		inputPowerUpRotate.OnPowerupRotateCanceled += cancelPowerUp;

		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		inputPowerUpRotate.startRotate ();
		this.gameObject.SetActive( true);
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

