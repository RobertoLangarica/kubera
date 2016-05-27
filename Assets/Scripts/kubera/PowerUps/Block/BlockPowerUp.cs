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

		inputBlockPowerUp.enabled = false;
	}

	public override void activate ()
	{
		inputBlockPowerUp.enabled = true;

		inputBlockPowerUp.createBlock (powerUpBlock,buttonPowerUpBlock.position);
	}

	protected void completePowerUp()
	{
		inputBlockPowerUp.enabled = false;
		OnComplete ();
	}

	public override void cancel()
	{
		inputBlockPowerUp.enabled = false;
		OnCancel ();
	}
}
