using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BlockPowerUp : PowerupBase
{
	public  InputBlockPowerUp inputBlockPowerUp;

	public GameManager gameManager;
	void Start()
	{
		//inputBlockPowerUp = FindObjectOfType<InputBlockPowerUp> ();
		inputBlockPowerUp.OnPowerupCanceled += cancel;
		inputBlockPowerUp.OnPowerupCompleted += completePowerUp;
		inputBlockPowerUp.OnPowerupCompletedNoGems += completePowerUpNoGems;
			
		if(!inputBlockPowerUp.rayCastersRegistered)
		{
			inputBlockPowerUp.registerRayCasters();
		}
		inputBlockPowerUp.enabled = false;
		//inputBlockPowerUp.gameObject.SetActive(false);
	}

	public override void activate (bool canUse)
	{
		inputBlockPowerUp.enabled = true;
		//inputBlockPowerUp.gameObject.SetActive(true);

		inputBlockPowerUp.createBlock (powerUpBlock,powerUpButton.position,canUse);

		updateDragableObjectImage (inputBlockPowerUp.getCurrentSelected());
		inputBlockPowerUp.setPieceColor ();

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.SQUARE_POWERUP);
	}

	protected void completePowerUp()
	{
		inputBlockPowerUp.enabled = false;
		//inputBlockPowerUp.gameObject.SetActive(false);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);

		gameManager.updatePiecesLightAndUpdateLetterState ();
		OnComplete ();
	}

	protected void completePowerUpNoGems()
	{
		inputBlockPowerUp.enabled = false;
		//inputBlockPowerUp.gameObject.SetActive(false);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);
		OnCompletedNoGems ();
	}

	public override void cancel()
	{
		inputBlockPowerUp.enabled = false;
		//inputBlockPowerUp.gameObject.SetActive(false);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.SQUARE_POWERUP);
		OnCancel ();
	}
}
