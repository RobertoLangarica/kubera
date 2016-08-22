using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BlockPowerUp : PowerupBase
{
	protected InputBlockPowerUp inputBlockPowerUp;

	void Start()
	{
		inputBlockPowerUp = FindObjectOfType<InputBlockPowerUp> ();
		inputBlockPowerUp.OnPowerupCanceled += cancel;
		inputBlockPowerUp.OnPowerupCompleted += completePowerUp;
		inputBlockPowerUp.OnPowerupCompletedNoGems += completePowerUpNoGems;
			
		inputBlockPowerUp.enabled = false;
	}

	public override void activate (bool canUse)
	{
		inputBlockPowerUp.enabled = true;

		inputBlockPowerUp.createBlock (powerUpBlock,powerUpButton.position,canUse);

		updateDragableObjectImage (inputBlockPowerUp.getCurrentSelected());
		inputBlockPowerUp.setPieceColor ();

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SQUARE_POWERUP);
	}

	protected void completePowerUp()
	{
		inputBlockPowerUp.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);
		OnComplete ();
	}

	protected void completePowerUpNoGems()
	{
		inputBlockPowerUp.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);
		OnCompletedNoGems ();
	}

	public override void cancel()
	{
		inputBlockPowerUp.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);
		OnCancel ();
	}
}
