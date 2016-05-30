using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BlockPowerUp : PowerupBase
{
	protected InputBlockPowerUp inputBlockPowerUp;

	public GameObject powerUpBlock;
	public Transform buttonPowerUpBlock;

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

		inputBlockPowerUp.createBlock (powerUpBlock,buttonPowerUpBlock.position,canUse);
	}

	protected void completePowerUp()
	{
		inputBlockPowerUp.enabled = false;
		OnComplete ();
	}

	protected void completePowerUpNoGems()
	{
		inputBlockPowerUp.enabled = false;
		OnCompletedNoGems ();
	}

	public override void cancel()
	{
		inputBlockPowerUp.enabled = false;
		OnCancel ();
	}
}
